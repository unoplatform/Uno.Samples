namespace QuoteCraft.Services;

public interface INotificationService
{
    int UnreadCount { get; }
    event Action<int>? UnreadCountChanged;

    Task<List<NotificationEntity>> GetAllAsync();
    Task<int> GetUnreadCountAsync();
    Task CreateAsync(NotificationType type, string title, string message, string? quoteId = null);
    Task MarkAsReadAsync(string notificationId);
    Task MarkAllAsReadAsync();
}
