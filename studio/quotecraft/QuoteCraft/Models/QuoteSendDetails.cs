namespace QuoteCraft.Models;

/// <summary>What the Send dialog starts from: the quote, its client's email and the business's name.</summary>
public record QuoteSendDetails(
    string QuoteId,
    string QuoteNumber,
    string Title,
    string? ClientName,
    string ClientEmail,
    string BusinessName,
    DateTimeOffset? ValidUntil);
