namespace QuoteCraft.Models;

public enum QuoteStatus
{
    Draft,
    Sent,
    /// <summary>Set by share links, which are gone (D11): kept so a quote stored with it still reads, as a Sent one.</summary>
    Viewed,
    Accepted,
    Declined,
    Expired
}
