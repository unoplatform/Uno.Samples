namespace QuoteCraft.Services;

public interface INotificationService
{
    int UnreadCount { get; }
    event Action<int>? UnreadCountChanged;

    Task<List<NotificationEntity>> GetAllAsync();
    Task<int> GetUnreadCountAsync();
    Task CreateAsync(NotificationType type, string title, string message, string? quoteId = null);

    /// <summary>True when a notification of this type for this quote was created at or after <paramref name="since"/>.</summary>
    Task<bool> ExistsAsync(NotificationType type, string quoteId, DateTimeOffset since);
    Task MarkAllAsReadAsync();
}
