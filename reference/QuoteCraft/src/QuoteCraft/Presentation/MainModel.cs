using QuoteCraft.Helpers;
using QuoteCraft.Services;

namespace QuoteCraft.Presentation;

public partial record MainModel
{
    private readonly INotificationService _notificationService;
    private readonly ConnectivityHelper _connectivity;

    public MainModel(INotificationService notificationService, ConnectivityHelper connectivity)
    {
        _notificationService = notificationService;
        _connectivity = connectivity;
    }

    public IState<int> NotificationVersion => State<int>.Value(this, () => 0);

    public IFeed<int> UnreadNotificationCount => NotificationVersion
        .SelectAsync(async (_, ct) => await _notificationService.GetUnreadCountAsync());

    // ── Connectivity ───────────────────────────────────────────────────────
    // Exposed so MainPage can wire its OfflineBanner without service-locator lookups.

    public ConnectivityHelper Connectivity => _connectivity;

    // ── Notification Overlay Support ───────────────────────────────────────

    public Task<List<NotificationEntity>> GetAllNotificationsAsync() => _notificationService.GetAllAsync();

    public async ValueTask MarkAllNotificationsReadAsync(CancellationToken ct)
    {
        await _notificationService.MarkAllAsReadAsync();
        await NotificationVersion.UpdateAsync(v => v + 1, ct);
    }
}
