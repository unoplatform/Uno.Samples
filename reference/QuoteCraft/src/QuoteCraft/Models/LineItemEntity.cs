namespace QuoteCraft.Models;

public class LineItemEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string QuoteId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; } = 1;
    public int SortOrder { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? SyncedAt { get; set; }

    public decimal LineTotal => UnitPrice * Quantity;
}
