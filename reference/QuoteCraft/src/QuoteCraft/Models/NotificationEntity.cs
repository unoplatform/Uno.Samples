namespace QuoteCraft.Models;

public enum NotificationType
{
    QuoteViewed,
    QuoteAccepted,
    QuoteDeclined,
    QuoteExpiring
}

public class NotificationEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? QuoteId { get; set; }
    public bool IsRead { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? SyncedAt { get; set; }
}
