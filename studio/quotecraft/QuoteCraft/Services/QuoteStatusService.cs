namespace QuoteCraft.Services;

/// <summary>
/// Moves a quote between statuses and records each change in its history (PRD F3, F17). The rules live here:
/// <list type="bullet">
/// <item>Only a quote a client has (Sent or Viewed) expires. A Draft has not been offered to anyone, so its
/// valid-until date is a plan, not a deadline.</item>
/// <item>Sending keeps the quote's valid-until date (set in the editor, PRD F1.8) while it is ahead; a quote whose
/// date has passed gets the profile's number of days from today, so it does not expire at once.</item>
/// <item>Reopening an expired quote makes it a Draft again with a fresh validity period, ready to send again
/// (F17.3).</item>
/// </list>
/// </summary>
public class QuoteStatusService
{
    private readonly IQuoteRepository _quoteRepo;
    private readonly IStatusHistoryRepository _history;
    private readonly IBusinessProfileRepository _profileRepo;
    private readonly SemaphoreSlim _expiryGate = new(1, 1);

    public QuoteStatusService(IQuoteRepository quoteRepo, IStatusHistoryRepository history, IBusinessProfileRepository profileRepo)
    {
        _quoteRepo = quoteRepo;
        _history = history;
        _profileRepo = profileRepo;
    }

    /// <summary>When the quote was sent, accepted, declined, expired or reopened (PRD F3.4), oldest first.</summary>
    public Task<List<StatusHistoryEntry>> GetHistoryAsync(string quoteId) => _history.GetByQuoteIdAsync(quoteId);

    /// <summary>An explicit status change (the editor's Mark Sent, Accepted and Declined).</summary>
    public async Task<QuoteEntity?> SetStatusAsync(string quoteId, QuoteStatus status, string changedBy = "user")
    {
        var quote = await _quoteRepo.GetByIdAsync(quoteId);
        if (quote is null)
        {
            return null;
        }

        var now = DateTimeOffset.UtcNow;
        quote.Status = status;
        if ((status is QuoteStatus.Sent or QuoteStatus.Draft) && (quote.ValidUntil is not { } current || current <= now))
        {
            quote.ValidUntil = now.AddDays((await _profileRepo.GetAsync()).QuoteValidDays);
        }
        if (status == QuoteStatus.Sent)
        {
            quote.SentAt = now;
        }

        quote.UpdatedAt = now;
        await _quoteRepo.SaveAsync(quote);
        await _history.RecordAsync(quoteId, status.ToString(), changedBy);
        return quote;
    }

    /// <summary>
    /// The quote went out by email: Sent, unless the client has already answered (Accepted or Declined stay).
    /// </summary>
    public async Task<QuoteEntity?> MarkSentAsync(string quoteId)
    {
        var quote = await _quoteRepo.GetByIdAsync(quoteId);
        return quote is null or { Status: QuoteStatus.Accepted or QuoteStatus.Declined }
            ? quote
            : await SetStatusAsync(quoteId, QuoteStatus.Sent);
    }

    /// <summary>An expired quote back to Draft, valid again; any other status is left as it is.</summary>
    public async Task<QuoteEntity?> ReopenAsync(string quoteId)
    {
        var quote = await _quoteRepo.GetByIdAsync(quoteId);
        return quote is { Status: QuoteStatus.Expired }
            ? await SetStatusAsync(quoteId, QuoteStatus.Draft)
            : quote;
    }

    /// <summary>
    /// Sent and Viewed quotes past their valid-until date become Expired, each with one Expired line in its history.
    /// The Quotes list and <see cref="QuoteExpiryService"/> both ask for it, at times at once: one run goes at a time,
    /// and a quote another run has expired already is left alone, so the history never gets a second line.
    /// </summary>
    /// <returns>The quotes this run expired.</returns>
    public async Task<IReadOnlyList<QuoteEntity>> ExpireOverdueAsync(DateTimeOffset now)
    {
        await _expiryGate.WaitAsync();
        try
        {
            var expired = new List<QuoteEntity>();
            foreach (var quote in await _quoteRepo.GetExpirableAsync(now))
            {
                if (await _quoteRepo.MarkExpiredAsync(quote.Id))
                {
                    await _history.RecordAsync(quote.Id, nameof(QuoteStatus.Expired), "system");
                    quote.Status = QuoteStatus.Expired;
                    expired.Add(quote);
                }
            }
            return expired;
        }
        finally
        {
            _expiryGate.Release();
        }
    }
}
