namespace QuoteCraft.Services;

public interface ISyncService
{
    bool IsSyncing { get; }
    DateTimeOffset? LastSyncedAt { get; }
    event Action<bool>? SyncStatusChanged;

    Task StartAsync();
    Task StopAsync();
    Task SyncNowAsync();
}
