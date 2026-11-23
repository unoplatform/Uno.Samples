namespace QuoteCraft.Models;

public class BusinessProfileEntity
{
    public string Id { get; set; } = "default";
    public string? BusinessName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Website { get; set; }
    public string? BusinessNumber { get; set; }
    public string? LogoPath { get; set; }
    public decimal DefaultTaxRate { get; set; }
    public decimal DefaultMarkup { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public int QuoteValidDays { get; set; } = 14;
    public string QuoteNumberPrefix { get; set; } = "QC-";
    public string? CustomFooter { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? SyncedAt { get; set; }
}
