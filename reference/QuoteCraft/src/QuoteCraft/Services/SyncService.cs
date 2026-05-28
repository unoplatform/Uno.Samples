using QuoteCraft.Data.Remote;

namespace QuoteCraft.Services;

/// <summary>
/// Background sync service that pushes local changes to Supabase
/// and pulls remote changes for Pro+ users.
/// Uses change tracking: records where updated_at > synced_at are dirty.
/// Conflict resolution: last-write-wins based on updated_at.
/// </summary>
public class SyncService : ISyncService, IDisposable
{
    private readonly IAuthService _authService;
    private readonly IFeatureGateService _featureGate;
    private readonly SupabaseClient _supabase;
    private readonly IQuoteRepository _quoteRepo;
    private readonly IClientRepository _clientRepo;
    private readonly IBusinessProfileRepository _profileRepo;
    private readonly Helpers.ConnectivityHelper _connectivity;
    private readonly ILogger<SyncService> _logger;
    private Timer? _syncTimer;
    private int _isSyncing; // 0 = not syncing, 1 = syncing (Interlocked)

    public bool IsSyncing => _isSyncing == 1;
    public DateTimeOffset? LastSyncedAt { get; private set; }
    public event Action<bool>? SyncStatusChanged;

    public SyncService(
        IAuthService authService,
        IFeatureGateService featureGate,
        SupabaseClient supabase,
        IQuoteRepository quoteRepo,
        IClientRepository clientRepo,
        IBusinessProfileRepository profileRepo,
        Helpers.ConnectivityHelper connectivity,
        ILogger<SyncService> logger)
    {
        _authService = authService;
        _featureGate = featureGate;
        _supabase = supabase;
        _quoteRepo = quoteRepo;
        _clientRepo = clientRepo;
        _profileRepo = profileRepo;
        _connectivity = connectivity;
        _logger = logger;
    }

    public async Task StartAsync()
    {
        if (!_featureGate.HasCloudSync || !_authService.CurrentState.IsAuthenticated)
        {
            _logger.LogInformation("Sync not enabled: CloudSync={HasSync}, Auth={IsAuth}",
                _featureGate.HasCloudSync, _authService.CurrentState.IsAuthenticated);
            return;
        }

        _connectivity.StartMonitoring();

        // Initial sync on startup
        await SyncNowAsync();

        // Periodic sync every 30 seconds
        _syncTimer?.Dispose();
        _syncTimer = new Timer(async _ => await SyncNowAsync(), null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
    }

    public Task StopAsync()
    {
        _syncTimer?.Dispose();
        _syncTimer = null;
        _connectivity.StopMonitoring();
        return Task.CompletedTask;
    }

    public async Task SyncNowAsync()
    {
        if (!_connectivity.IsOnline || !_authService.CurrentState.IsAuthenticated)
            return;

        // Atomic check-and-set to prevent concurrent sync
        if (Interlocked.CompareExchange(ref _isSyncing, 1, 0) != 0)
            return;

        SyncStatusChanged?.Invoke(true);

        try
        {
            await PushLocalChangesAsync();
            await PullRemoteChangesAsync();
            LastSyncedAt = DateTimeOffset.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Sync failed");
        }
        finally
        {
            Interlocked.Exchange(ref _isSyncing, 0);
            SyncStatusChanged?.Invoke(false);
        }
    }

    private async Task PushLocalChangesAsync()
    {
        var userId = _authService.CurrentState.UserId;
        if (userId is null) return;

        // Push dirty quotes (updated_at > synced_at or synced_at is null)
        var allQuotes = await _quoteRepo.GetAllAsync();
        var dirtyQuotes = allQuotes.Where(q => q.SyncedAt is null || q.UpdatedAt > q.SyncedAt).ToList();

        if (dirtyQuotes.Count > 0)
        {
            var remoteQuotes = dirtyQuotes.Select(q => new
            {
                q.Id,
                user_id = userId,
                q.Title,
                client_id = q.ClientId,
                client_name = q.ClientName,
                q.Notes,
                tax_rate = q.TaxRate,
                status = q.Status.ToString(),
                quote_number = q.QuoteNumber,
                created_at = q.CreatedAt.ToString("O"),
                sent_at = q.SentAt?.ToString("O"),
                valid_until = q.ValidUntil?.ToString("O"),
                updated_at = q.UpdatedAt.ToString("O"),
                share_token = q.ShareToken,
                is_deleted = q.IsDeleted,
            }).ToList();

            await _supabase.UpsertBatchAsync("quotes", remoteQuotes);

            // Mark as synced locally
            foreach (var q in dirtyQuotes)
            {
                q.SyncedAt = DateTimeOffset.UtcNow;
                await _quoteRepo.SaveAsync(q);
            }

            _logger.LogInformation("Pushed {Count} dirty quotes", dirtyQuotes.Count);
        }

        // Push dirty clients
        var allClients = await _clientRepo.GetAllAsync();
        var dirtyClients = allClients.Where(c => c.SyncedAt is null || c.UpdatedAt > c.SyncedAt).ToList();

        if (dirtyClients.Count > 0)
        {
            var remoteClients = dirtyClients.Select(c => new
            {
                c.Id,
                user_id = userId,
                c.Name,
                c.Email,
                c.Phone,
                c.Address,
                updated_at = c.UpdatedAt.ToString("O"),
                is_deleted = c.IsDeleted,
            }).ToList();

            await _supabase.UpsertBatchAsync("clients", remoteClients);

            foreach (var c in dirtyClients)
            {
                c.SyncedAt = DateTimeOffset.UtcNow;
                await _clientRepo.SaveAsync(c);
            }

            _logger.LogInformation("Pushed {Count} dirty clients", dirtyClients.Count);
        }
    }

    private async Task PullRemoteChangesAsync()
    {
        // Pull quotes updated after our last sync
        var sinceParam = LastSyncedAt?.ToString("O") ?? DateTimeOffset.MinValue.ToString("O");

        try
        {
            var remoteQuotes = await _supabase.GetAsync<RemoteQuote>(
                "quotes", $"select=*&updated_at=gt.{sinceParam}&order=updated_at.asc");

            foreach (var rq in remoteQuotes)
            {
                var local = await _quoteRepo.GetByIdAsync(rq.Id ?? "");
                var remoteUpdated = DateTimeOffset.TryParse(rq.UpdatedAt, out var ru) ? ru : DateTimeOffset.MinValue;

                // Last-write-wins: only apply if remote is newer
                if (local is null || remoteUpdated > local.UpdatedAt)
                {
                    var entity = new QuoteEntity
                    {
                        Id = rq.Id ?? Guid.NewGuid().ToString(),
                        Title = rq.Title ?? "Untitled",
                        ClientId = rq.ClientId,
                        ClientName = rq.ClientName,
                        Notes = rq.Notes,
                        TaxRate = rq.TaxRate,
                        Status = Enum.TryParse<QuoteStatus>(rq.Status, out var s) ? s : QuoteStatus.Draft,
                        QuoteNumber = rq.QuoteNumber ?? "",
                        CreatedAt = DateTimeOffset.TryParse(rq.CreatedAt, out var ca) ? ca : DateTimeOffset.UtcNow,
                        SentAt = DateTimeOffset.TryParse(rq.SentAt, out var sa) ? sa : null,
                        ValidUntil = DateTimeOffset.TryParse(rq.ValidUntil, out var vu) ? vu : null,
                        UpdatedAt = remoteUpdated,
                        ShareToken = rq.ShareToken,
                        IsDeleted = rq.IsDeleted,
                        SyncedAt = DateTimeOffset.UtcNow,
                    };
                    await _quoteRepo.SaveAsync(entity);
                }
            }

            var remoteClients = await _supabase.GetAsync<RemoteClient>(
                "clients", $"select=*&updated_at=gt.{sinceParam}&order=updated_at.asc");

            foreach (var rc in remoteClients)
            {
                var local = await _clientRepo.GetByIdAsync(rc.Id ?? "");
                var remoteUpdated = DateTimeOffset.TryParse(rc.UpdatedAt, out var ru) ? ru : DateTimeOffset.MinValue;

                if (local is null || remoteUpdated > local.UpdatedAt)
                {
                    var entity = new ClientEntity
                    {
                        Id = rc.Id ?? Guid.NewGuid().ToString(),
                        Name = rc.Name ?? "",
                        Email = rc.Email,
                        Phone = rc.Phone,
                        Address = rc.Address,
                        UpdatedAt = remoteUpdated,
                        IsDeleted = rc.IsDeleted,
                        SyncedAt = DateTimeOffset.UtcNow,
                    };
                    await _clientRepo.SaveAsync(entity);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Pull remote changes failed");
        }
    }

    public void Dispose()
    {
        _syncTimer?.Dispose();
        _connectivity.StopMonitoring();
    }

    // DTOs for deserialization from Supabase REST API
    private class RemoteQuote
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? Notes { get; set; }
        public decimal TaxRate { get; set; }
        public string? Status { get; set; }
        public string? QuoteNumber { get; set; }
        public string? CreatedAt { get; set; }
        public string? SentAt { get; set; }
        public string? ValidUntil { get; set; }
        public string? UpdatedAt { get; set; }
        public string? ShareToken { get; set; }
        public bool IsDeleted { get; set; }
    }

    private class RemoteClient
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
