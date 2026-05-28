using Pens.Models;

namespace Pens.Services;

/// <summary>
/// In-memory mock implementation of ISupabaseService for development and demos.
/// Provides realistic hockey team data without requiring a Supabase backend.
/// </summary>
public class MockSupabaseService : ISupabaseService, IDisposable
{
    private readonly List<DbPlayer> _players;
    private readonly List<DbGame> _games;
    private readonly List<DbAttendance> _attendance;
    private readonly List<DbChatMessage> _chatMessages;
    private readonly List<DbDuty> _duties;
    private DbBeerTracker _beerTracker;
    private int _nextChatId;
    private int _nextAttendanceId = 100;
    private int _nextDutyId = 100;

    private Action<DbChatMessage>? _onNewMessage;
    private CancellationTokenSource? _pollingCts;

    public MockSupabaseService()
    {
        _players = GeneratePlayers();
        _games = GenerateGames();
        _attendance = GenerateAttendance();
        _chatMessages = GenerateChatMessages();
        _duties = GenerateDuties();
        _beerTracker = new DbBeerTracker
        {
            Id = 1,
            Season = "2024-2025",
            ConsumedCases = 17,
            UpdatedAt = DateTime.UtcNow
        };
        _nextChatId = _chatMessages.Max(m => m.Id) + 1;
    }

    public Task<List<DbPlayer>> GetPlayersAsync()
    {
        return Task.FromResult(_players.OrderBy(p => p.Number).ToList());
    }

    public Task<List<DbGame>> GetUpcomingGamesAsync()
    {
        var today = DateTime.Today;
        var upcoming = _games
            .Where(g => g.GameDate.Date >= today)
            .OrderBy(g => g.GameDate)
            .Take(10)
            .ToList();
        return Task.FromResult(upcoming);
    }

    public Task<List<DbGame>> GetPastGamesAsync()
    {
        var past = _games
            .Where(g => g.GameDate.Date < DateTime.Today && g.HomeScore != null)
            .OrderByDescending(g => g.GameDate)
            .Take(4)
            .ToList();
        return Task.FromResult(past);
    }

    public Task<List<DbAttendance>> GetAllAttendanceForWeekAsync(DateTime weekOf)
    {
        var records = _attendance
            .Where(a => a.WeekOf.Date == weekOf.Date)
            .ToList();
        return Task.FromResult(records);
    }

    public Task<DbAttendance?> GetAttendanceAsync(int playerId, DateTime weekOf)
    {
        var record = _attendance.FirstOrDefault(a => a.PlayerId == playerId && a.WeekOf.Date == weekOf.Date);
        return Task.FromResult(record);
    }

    public Task UpsertAttendanceAsync(int playerId, DateTime weekOf, string status)
    {
        var existing = _attendance.FirstOrDefault(a => a.PlayerId == playerId && a.WeekOf.Date == weekOf.Date);
        if (existing != null)
        {
            existing.Status = status;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            _attendance.Add(new DbAttendance
            {
                Id = _nextAttendanceId++,
                PlayerId = playerId,
                WeekOf = weekOf.Date,
                Status = status,
                UpdatedAt = DateTime.UtcNow
            });
        }
        return Task.CompletedTask;
    }

    public Task<List<DbChatMessage>> GetChatMessagesAsync(int limit = 50)
    {
        var messages = _chatMessages
            .OrderByDescending(m => m.CreatedAt)
            .Take(limit)
            .OrderBy(m => m.CreatedAt)
            .ToList();
        return Task.FromResult(messages);
    }

    public Task<DbChatMessage> SendChatMessageAsync(int? playerId, string playerName, string message)
    {
        var msg = new DbChatMessage
        {
            Id = _nextChatId++,
            PlayerId = playerId,
            PlayerName = playerName,
            Message = message,
            CreatedAt = DateTime.UtcNow
        };
        _chatMessages.Add(msg);
        return Task.FromResult(msg);
    }

    public Task<DbBeerTracker?> GetBeerTrackerAsync(string season = "2024-2025")
    {
        var tracker = _beerTracker.Season == season ? _beerTracker : null;
        return Task.FromResult<DbBeerTracker?>(tracker);
    }

    public Task UpdateBeerCountAsync(int consumedCases, string season = "2024-2025")
    {
        if (_beerTracker.Season == season)
        {
            _beerTracker.ConsumedCases = consumedCases;
            _beerTracker.UpdatedAt = DateTime.UtcNow;
        }
        return Task.CompletedTask;
    }

    public Task<List<DbDuty>> GetDutiesForGameAsync(int gameId)
    {
        var duties = _duties.Where(d => d.GameId == gameId).ToList();
        return Task.FromResult(duties);
    }

    public Task AssignDutiesAsync(int gameId, Dictionary<string, int> dutyAssignments)
    {
        _duties.RemoveAll(d => d.GameId == gameId);
        foreach (var kv in dutyAssignments)
        {
            _duties.Add(new DbDuty
            {
                Id = _nextDutyId++,
                GameId = gameId,
                PlayerId = kv.Value,
                DutyType = kv.Key,
                CreatedAt = DateTime.UtcNow
            });
        }
        return Task.CompletedTask;
    }

    public Task SubscribeToChatMessagesAsync(Action<DbChatMessage> onNewMessage, CancellationToken cancellationToken = default)
    {
        // Mock polling does nothing - messages appear instantly via SendChatMessageAsync
        UnsubscribeFromChat();
        _pollingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _onNewMessage = onNewMessage;
        return Task.CompletedTask;
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

    // ─── Data Generators ────────────────────────────────────────

    private static List<DbPlayer> GeneratePlayers()
    {
        return
        [
            new() { Id = 1,  Name = "Marc Tremblay",     Number = 7,  Position = "C",  IsInjured = false, CreatedAt = DateTime.UtcNow },
            new() { Id = 2,  Name = "Patrick Bouchard",   Number = 11, Position = "LW", IsInjured = false, CreatedAt = DateTime.UtcNow },
            new() { Id = 3,  Name = "Steve Lapointe",     Number = 14, Position = "RW", IsInjured = false, CreatedAt = DateTime.UtcNow },
            new() { Id = 4,  Name = "Jean-Luc Martin",    Number = 19, Position = "C",  IsInjured = false, CreatedAt = DateTime.UtcNow },
            new() { Id = 5,  Name = "Mike Gagnon",        Number = 22, Position = "D",  IsInjured = false, CreatedAt = DateTime.UtcNow },
            new() { Id = 6,  Name = "Dave Caron",         Number = 24, Position = "D",  IsInjured = false, CreatedAt = DateTime.UtcNow },
            new() { Id = 7,  Name = "Phil Bergeron",      Number = 27, Position = "LW", IsInjured = false, CreatedAt = DateTime.UtcNow },
            new() { Id = 8,  Name = "Eric Pelletier",     Number = 31, Position = "G",  IsInjured = false, CreatedAt = DateTime.UtcNow },
            new() { Id = 9,  Name = "Ryan Dubois",        Number = 33, Position = "RW", IsInjured = false, CreatedAt = DateTime.UtcNow },
            new() { Id = 10, Name = "Luc Belanger",       Number = 37, Position = "D",  IsInjured = true,  CreatedAt = DateTime.UtcNow },
            new() { Id = 11, Name = "Kevin Roy",          Number = 41, Position = "C",  IsInjured = false, CreatedAt = DateTime.UtcNow },
            new() { Id = 12, Name = "Dan Fournier",       Number = 44, Position = "LW", IsInjured = false, CreatedAt = DateTime.UtcNow },
            new() { Id = 13, Name = "Chris Levesque",     Number = 55, Position = "D",  IsInjured = false, CreatedAt = DateTime.UtcNow },
            new() { Id = 14, Name = "Alex Nadeau",        Number = 61, Position = "RW", IsInjured = false, CreatedAt = DateTime.UtcNow },
            new() { Id = 15, Name = "Ben Gauthier",       Number = 77, Position = "G",  IsInjured = false, CreatedAt = DateTime.UtcNow },
        ];
    }

    private static List<DbGame> GenerateGames()
    {
        var today = DateTime.Today;
        // Find the next Thursday from today (or today if it is Thursday)
        var daysUntilThursday = ((int)DayOfWeek.Thursday - (int)today.DayOfWeek + 7) % 7;
        var nextThursday = today.AddDays(daysUntilThursday == 0 ? 0 : daysUntilThursday);

        var opponents = new[] { "Lachine Hawks", "DDO Storm", "Kirkland Kings", "Pointe-Claire Blades", "Beaconsfield Bears", "St-Lazare Wolves" };
        var rinks = new[] { "Dorval Arena", "Lachine Arena", "DDO Civic Centre", "Kirkland Arena", "Pointe-Claire Aquatic Centre", "Beaconsfield Arena" };

        var games = new List<DbGame>();
        var id = 1;

        // Past games (last 6 Thursdays with scores)
        for (var i = 6; i >= 1; i--)
        {
            var pastDate = nextThursday.AddDays(-7 * i);
            var oppIdx = (i - 1) % opponents.Length;
            var isHome = i % 2 == 0;
            var rink = isHome ? "Dorval Arena" : rinks[oppIdx];
            var pScore = Random.Shared.Next(1, 8);
            var oScore = Random.Shared.Next(0, 7);

            games.Add(new DbGame
            {
                Id = id++,
                Opponent = opponents[oppIdx],
                GameDate = pastDate,
                GameTime = "21:15",
                Rink = rink,
                IsHome = isHome,
                HomeScore = isHome ? pScore : oScore,
                AwayScore = isHome ? oScore : pScore,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Upcoming games (next 8 Thursdays)
        for (var i = 0; i < 8; i++)
        {
            var futureDate = nextThursday.AddDays(7 * i);
            var oppIdx = i % opponents.Length;
            var isHome = i % 2 == 0;
            var rink = isHome ? "Dorval Arena" : rinks[oppIdx];

            games.Add(new DbGame
            {
                Id = id++,
                Opponent = opponents[oppIdx],
                GameDate = futureDate,
                GameTime = "21:15",
                Rink = rink,
                IsHome = isHome,
                HomeScore = null,
                AwayScore = null,
                CreatedAt = DateTime.UtcNow
            });
        }

        return games;
    }

    private List<DbAttendance> GenerateAttendance()
    {
        var today = DateTime.Today;
        var weekOf = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

        var statuses = new[] { "in", "in", "in", "in", "in", "in", "in", "in", "out", "out", "pending", "pending", "in", "in", "in" };
        var records = new List<DbAttendance>();

        for (var i = 0; i < _players.Count && i < statuses.Length; i++)
        {
            records.Add(new DbAttendance
            {
                Id = i + 1,
                PlayerId = _players[i].Id,
                WeekOf = weekOf,
                Status = statuses[i],
                UpdatedAt = DateTime.UtcNow
            });
        }

        return records;
    }

    private List<DbChatMessage> GenerateChatMessages()
    {
        var now = DateTime.UtcNow;
        return
        [
            new() { Id = 1,  PlayerId = 1,  PlayerName = "Marc Tremblay",     Message = "Who's bringing beer this week?",                    CreatedAt = now.AddHours(-48) },
            new() { Id = 2,  PlayerId = 7,  PlayerName = "Phil Bergeron",      Message = "I got it. Grabbing two flats of Molson Ex.",         CreatedAt = now.AddHours(-47) },
            new() { Id = 3,  PlayerId = 5,  PlayerName = "Mike Gagnon",        Message = "Beauty. I'll bring the cooler.",                     CreatedAt = now.AddHours(-46) },
            new() { Id = 4,  PlayerId = 3,  PlayerName = "Steve Lapointe",     Message = "Luc, how's the knee? You playing Thursday?",         CreatedAt = now.AddHours(-30) },
            new() { Id = 5,  PlayerId = 10, PlayerName = "Luc Belanger",       Message = "Still sore. Gonna sit this one out boys.",           CreatedAt = now.AddHours(-29) },
            new() { Id = 6,  PlayerId = 4,  PlayerName = "Jean-Luc Martin",    Message = "Rest up buddy. We need you for playoffs.",           CreatedAt = now.AddHours(-28) },
            new() { Id = 7,  PlayerId = 6,  PlayerName = "Dave Caron",         Message = "Anyone know what time the rink opens?",              CreatedAt = now.AddHours(-24) },
            new() { Id = 8,  PlayerId = 8,  PlayerName = "Eric Pelletier",     Message = "Doors open at 8:45. Game at 9:15.",                  CreatedAt = now.AddHours(-23) },
            new() { Id = 9,  PlayerId = 11, PlayerName = "Kevin Roy",          Message = "Let's go boys! Gotta bounce back from last week.",   CreatedAt = now.AddHours(-12) },
            new() { Id = 10, PlayerId = 12, PlayerName = "Dan Fournier",       Message = "Their goalie is weak glove side. Shoot high.",       CreatedAt = now.AddHours(-10) },
            new() { Id = 11, PlayerId = 9,  PlayerName = "Ryan Dubois",        Message = "Who owes ice money? I'm collecting Thursday.",       CreatedAt = now.AddHours(-8) },
            new() { Id = 12, PlayerId = 2,  PlayerName = "Patrick Bouchard",   Message = "I paid last week. Should be good.",                  CreatedAt = now.AddHours(-7) },
            new() { Id = 13, PlayerId = 13, PlayerName = "Chris Levesque",     Message = "Can someone tape my stick before the game?",         CreatedAt = now.AddHours(-5) },
            new() { Id = 14, PlayerId = 14, PlayerName = "Alex Nadeau",        Message = "Bring your own tape this time Chris lol",            CreatedAt = now.AddHours(-4) },
            new() { Id = 15, PlayerId = 1,  PlayerName = "Marc Tremblay",      Message = "Alright everyone mark yourselves in or out. Need a headcount.", CreatedAt = now.AddHours(-2) },
            new() { Id = 16, PlayerId = 15, PlayerName = "Ben Gauthier",       Message = "I'm in. Eric, you starting or am I?",               CreatedAt = now.AddHours(-1) },
            new() { Id = 17, PlayerId = 8,  PlayerName = "Eric Pelletier",     Message = "You start, I'll take the second half.",              CreatedAt = now.AddMinutes(-30) },
        ];
    }

    private List<DbDuty> GenerateDuties()
    {
        // The first upcoming game ID - past games take IDs 1-6, so first upcoming is 7
        var nextGameId = 7;

        return
        [
            new() { Id = 1, GameId = nextGameId, PlayerId = 9,  DutyType = "ice",    CreatedAt = DateTime.UtcNow },
            new() { Id = 2, GameId = nextGameId, PlayerId = 7,  DutyType = "beer",   CreatedAt = DateTime.UtcNow },
            new() { Id = 3, GameId = nextGameId, PlayerId = 5,  DutyType = "cooler", CreatedAt = DateTime.UtcNow },
            new() { Id = 4, GameId = nextGameId, PlayerId = 12, DutyType = "food",   CreatedAt = DateTime.UtcNow },
        ];
    }
}
