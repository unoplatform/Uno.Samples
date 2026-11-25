namespace QuoteCraft.Models;

public partial record ClientDisplayItem(
    string Id,
    string Name,
    string Initials,
    int QuoteCount,
    string City,
    decimal TotalValue,
    ClientEntity Entity)
{
    /// <summary>"2 quotes · Riverton", or "1 quote" when the client has no city.</summary>
    public string Summary => string.IsNullOrWhiteSpace(City)
        ? $"{QuoteCount} {(QuoteCount == 1 ? "quote" : "quotes")}"
        : $"{QuoteCount} {(QuoteCount == 1 ? "quote" : "quotes")} · {City}";
}
