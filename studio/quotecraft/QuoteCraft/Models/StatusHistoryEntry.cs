namespace QuoteCraft.Models;

public class StatusHistoryEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string QuoteId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset ChangedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? ChangedBy { get; set; }
}
