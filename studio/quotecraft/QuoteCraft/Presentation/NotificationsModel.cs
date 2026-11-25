namespace QuoteCraft.Presentation;

/// <summary>
/// The notifications drawer (route <c>!Notifications</c>), opened by <see cref="MainModel.OpenNotifications"/>: the
/// rail's bell from 800 px and the bottom tab bar's Alerts below (D18).
/// </summary>
public partial record NotificationsModel
{
    private readonly INotificationService _notificationService;

    public NotificationsModel(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public IState<int> Version => State<int>.Value(this, () => 0);

    public IListFeed<NotificationEntity> Notifications => Version
        .SelectAsync(async (_, ct) =>
            (IImmutableList<NotificationEntity>)(await _notificationService.GetAllAsync()).ToImmutableList())
        .AsListFeed();

    /// <summary>What Mark all read could not do; shown under the bar.</summary>
    public IState<string> ActionError => State<string>.Value(this, () => string.Empty);

    public async ValueTask MarkAllRead(CancellationToken ct)
    {
        await ActionError.UpdateAsync(_ => string.Empty, ct);
        try
        {
            await _notificationService.MarkAllAsReadAsync();
        }
        catch (Exception ex)
        {
            ErrorReporter.Log(ex, "Mark all notifications read");
            await ActionError.UpdateAsync(_ => ErrorReporter.Describe("The notifications could not be marked as read.", ex), ct);
            return;
        }
        await Version.UpdateAsync(v => v + 1, ct);
    }
}
