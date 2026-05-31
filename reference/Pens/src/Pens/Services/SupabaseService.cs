using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pens.Models;
using Supabase;

namespace Pens.Services;

public interface ISupabaseService
{
    Task<List<DbPlayer>> GetPlayersAsync();
    Task<List<DbAttendance>> GetAllAttendanceForWeekAsync(DateTime weekOf);
    Task<List<DbChatMessage>> GetChatMessagesAsync(int limit = 50);
    Task<DbChatMessage> SendChatMessageAsync(int? playerId, string playerName, string message);
    Task<DbBeerTracker?> GetBeerTrackerAsync(string season = "2024-2025");
    Task UpdateBeerCountAsync(int consumedCases, string season = "2024-2025");
    Task<DbAttendance?> GetAttendanceAsync(int playerId, DateTime weekOf);
    Task UpsertAttendanceAsync(int playerId, DateTime weekOf, string status);
    Task<List<DbGame>> GetUpcomingGamesAsync();
    Task<List<DbGame>> GetPastGamesAsync();
    Task<List<DbDuty>> GetDutiesForGameAsync(int gameId);
    Task AssignDutiesAsync(int gameId, Dictionary<string, int> dutyAssignments);
    Task SubscribeToChatMessagesAsync(Action<DbChatMessage> onNewMessage, CancellationToken cancellationToken = default);
    void UnsubscribeFromChat();
}

public class SupabaseService : ISupabaseService, IDisposable
{
    // Fail fast on an unreachable/paused backend instead of hanging forever.
    private static readonly TimeSpan NetworkTimeout = TimeSpan.FromSeconds(12);

    private readonly Supabase.Client _client;
    private readonly ILogger<SupabaseService> _logger;
    private Task? _initTask;
    private Action<DbChatMessage>? _onNewMessage;
    private CancellationTokenSource? _pollingCts;

    public SupabaseService(IConfiguration configuration, ILogger<SupabaseService> logger)
    {
        _logger = logger;
        var url = configuration["Supabase:Url"] ?? throw new InvalidOperationException("Supabase:Url not configured");
        var key = configuration["Supabase:AnonKey"] ?? throw new InvalidOperationException("Supabase:AnonKey not configured");

        _client = new Supabase.Client(url, key);
    }

    // Initialize the client once, with a timeout so a dead backend surfaces an
    // error quickly rather than stalling the first call indefinitely.
    private async Task EnsureReadyAsync()
    {
        _initTask ??= _client.InitializeAsync();
        await _initTask.WaitAsync(NetworkTimeout);
    }

    public async Task<List<DbPlayer>> GetPlayersAsync()
    {
        await EnsureReadyAsync();
        var response = await _client.From<DbPlayer>()
            .Order("number", Supabase.Postgrest.Constants.Ordering.Ascending)
            .Get()
            .WaitAsync(NetworkTimeout);
        return response.Models;
    }

    public async Task<List<DbChatMessage>> GetChatMessagesAsync(int limit = 50)
    {
        await EnsureReadyAsync();
        var response = await _client.From<DbChatMessage>()
            .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
            .Limit(limit)
            .Get()
            .WaitAsync(NetworkTimeout);
        return response.Models.OrderBy(m => m.CreatedAt).ToList();
    }

    public async Task<DbChatMessage> SendChatMessageAsync(int? playerId, string playerName, string message)
    {
        await EnsureReadyAsync();
        var chatMessage = new DbChatMessage
        {
            PlayerId = playerId,
            PlayerName = playerName,
            Message = message,
            CreatedAt = DateTime.UtcNow
        };

        var response = await _client.From<DbChatMessage>().Insert(chatMessage).WaitAsync(NetworkTimeout);
        return response.Models.First();
    }

    public async Task<DbBeerTracker?> GetBeerTrackerAsync(string season = "2024-2025")
    {
        await EnsureReadyAsync();
        var response = await _client.From<DbBeerTracker>()
            .Where(b => b.Season == season)
            .Single()
            .WaitAsync(NetworkTimeout);
        return response;
    }

    public async Task UpdateBeerCountAsync(int consumedCases, string season = "2024-2025")
    {
        await EnsureReadyAsync();
        await _client.From<DbBeerTracker>()
            .Where(b => b.Season == season)
            .Set(b => b.ConsumedCases, consumedCases)
            .Set(b => b.UpdatedAt, DateTime.UtcNow)
            .Update()
            .WaitAsync(NetworkTimeout);
    }

    public async Task<DbAttendance?> GetAttendanceAsync(int playerId, DateTime weekOf)
    {
        try
        {
            await EnsureReadyAsync();
            var response = await _client.From<DbAttendance>()
                .Where(a => a.PlayerId == playerId)
                .Where(a => a.WeekOf == weekOf.Date)
                .Single()
                .WaitAsync(NetworkTimeout);
            return response;
        }
        catch (Exception ex)
        {
            // Single() throws if no record found - this is expected behavior
            _logger.LogDebug(ex, "No attendance record found for player {PlayerId} on {WeekOf}", playerId, weekOf.Date);
            return null;
        }
    }

    public async Task<List<DbAttendance>> GetAllAttendanceForWeekAsync(DateTime weekOf)
    {
        await EnsureReadyAsync();
        var response = await _client.From<DbAttendance>()
            .Where(a => a.WeekOf == weekOf.Date)
            .Get()
            .WaitAsync(NetworkTimeout);
        return response.Models;
    }

    public async Task UpsertAttendanceAsync(int playerId, DateTime weekOf, string status)
    {
        await EnsureReadyAsync();
        var attendance = new DbAttendance
        {
            PlayerId = playerId,
            WeekOf = weekOf.Date,
            Status = status,
            UpdatedAt = DateTime.UtcNow
        };

        // Conflict on (player_id, week_of) so re-toggling status updates the row
        // instead of inserting duplicates (matches the table's unique constraint).
        var options = new Supabase.Postgrest.QueryOptions { OnConflict = "player_id,week_of" };
        await _client.From<DbAttendance>().Upsert(attendance, options).WaitAsync(NetworkTimeout);
    }

    public async Task<List<DbGame>> GetUpcomingGamesAsync()
    {
        await EnsureReadyAsync();
        // Include today's games - they should show until the next day
        var today = DateTime.Today;
        var response = await _client.From<DbGame>()
            .Filter("game_date", Supabase.Postgrest.Constants.Operator.GreaterThanOrEqual, today.ToString("yyyy-MM-dd"))
            .Order("game_date", Supabase.Postgrest.Constants.Ordering.Ascending)
            .Limit(10)
            .Get()
            .WaitAsync(NetworkTimeout);
        return response.Models;
    }

    public async Task<List<DbGame>> GetPastGamesAsync()
    {
        await EnsureReadyAsync();
        var response = await _client.From<DbGame>()
            .Filter("game_date", Supabase.Postgrest.Constants.Operator.LessThan, DateTime.Today.ToString("yyyy-MM-dd"))
            .Not("home_score", Supabase.Postgrest.Constants.Operator.Is, "null")
            .Order("game_date", Supabase.Postgrest.Constants.Ordering.Descending)
            .Limit(4)
            .Get()
            .WaitAsync(NetworkTimeout);
        return response.Models;
    }

    public async Task<List<DbDuty>> GetDutiesForGameAsync(int gameId)
    {
        try
        {
            await EnsureReadyAsync();
            var response = await _client.From<DbDuty>()
                .Filter("game_id", Supabase.Postgrest.Constants.Operator.Equals, gameId.ToString())
                .Get()
                .WaitAsync(NetworkTimeout);
            return response.Models;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error getting duties for game {GameId}", gameId);
            return [];
        }
    }

    public async Task AssignDutiesAsync(int gameId, Dictionary<string, int> dutyAssignments)
    {
        try
        {
            await EnsureReadyAsync();

            // Delete existing duties for this game
            await _client.From<DbDuty>()
                .Filter("game_id", Supabase.Postgrest.Constants.Operator.Equals, gameId.ToString())
                .Delete()
                .WaitAsync(NetworkTimeout);

            // Insert new duties
            var duties = dutyAssignments.Select(kv => new DbDuty
            {
                GameId = gameId,
                PlayerId = kv.Value,
                DutyType = kv.Key,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            foreach (var duty in duties)
            {
                await _client.From<DbDuty>().Insert(duty).WaitAsync(NetworkTimeout);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning duties for game {GameId}", gameId);
            throw;
        }
    }

    public async Task SubscribeToChatMessagesAsync(Action<DbChatMessage> onNewMessage, CancellationToken cancellationToken = default)
    {
        UnsubscribeFromChat(); // Cancel any existing polling

        _pollingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _onNewMessage = onNewMessage;

        // Use polling for chat updates (Realtime requires Alpha access or Read Replica)
        var messages = await GetChatMessagesAsync(1);
        var lastMessageId = messages.LastOrDefault()?.Id ?? 0;
        var token = _pollingCts.Token;

        _ = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested && _onNewMessage != null)
            {
                try
                {
                    await Task.Delay(3000, token);
                    if (token.IsCancellationRequested) break;

                    var response = await _client.From<DbChatMessage>()
                        .Filter("id", Supabase.Postgrest.Constants.Operator.GreaterThan, lastMessageId.ToString())
                        .Order("id", Supabase.Postgrest.Constants.Ordering.Ascending)
                        .Get()
                        .WaitAsync(NetworkTimeout);

                    foreach (var msg in response.Models)
                    {
                        if (token.IsCancellationRequested) break;
                        lastMessageId = msg.Id;
                        _onNewMessage?.Invoke(msg);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Chat polling error, retrying...");
                }
            }
        }, token);
    }

    public void UnsubscribeFromChat()
    {
        _onNewMessage = null;
        _pollingCts?.Cancel();
        _pollingCts?.Dispose();
        _pollingCts = null;
    }

    public void Dispose()
    {
        UnsubscribeFromChat();
    }
}
