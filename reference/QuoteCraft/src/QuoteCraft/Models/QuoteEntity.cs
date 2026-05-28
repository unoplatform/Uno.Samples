namespace QuoteCraft.Models;

public class QuoteEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string? ClientId { get; set; }
    public string? ClientName { get; set; }
    public string? Notes { get; set; }
    public decimal TaxRate { get; set; }
    public QuoteStatus Status { get; set; } = QuoteStatus.Draft;
    public string QuoteNumber { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? SentAt { get; set; }
    public DateTimeOffset? ValidUntil { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? SyncedAt { get; set; }
    public string? ShareToken { get; set; }
    public bool IsDeleted { get; set; }

    // Populated by joins/queries, not stored directly
    public List<LineItemEntity> LineItems { get; set; } = [];

    public decimal Subtotal => LineItems.Sum(li => li.LineTotal);
    public decimal TaxAmount => Subtotal * (TaxRate / 100m);
    public decimal Total => Subtotal + TaxAmount;
}
