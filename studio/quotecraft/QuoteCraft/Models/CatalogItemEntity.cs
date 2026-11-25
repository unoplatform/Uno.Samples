namespace QuoteCraft.Models;

public class CatalogItemEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Description { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public string Category { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? SyncedAt { get; set; }
    public bool IsDeleted { get; set; }
}
