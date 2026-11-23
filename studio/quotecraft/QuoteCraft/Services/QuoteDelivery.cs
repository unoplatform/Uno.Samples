namespace QuoteCraft.Services;

/// <summary>
/// A quote as it goes to the client, for the open quote on the Quotes page and the editor: the quote with its items,
/// its client and the business (the detail, the print preview and the Send dialog read it), its PDF, and sending it by
/// email. The status rules are <see cref="QuoteStatusService"/>'s.
/// </summary>
public sealed class QuoteDelivery
{
    private readonly IQuoteRepository _quoteRepo;
    private readonly IClientRepository _clientRepo;
    private readonly IBusinessProfileRepository _profileRepo;
    private readonly IShareService _shareService;
    private readonly IEmailLauncherService _email;
    private readonly QuoteStatusService _status;

    public QuoteDelivery(
        IQuoteRepository quoteRepo,
        IClientRepository clientRepo,
        IBusinessProfileRepository profileRepo,
        IShareService shareService,
        IEmailLauncherService email,
        QuoteStatusService status)
    {
        _quoteRepo = quoteRepo;
        _clientRepo = clientRepo;
        _profileRepo = profileRepo;
        _shareService = shareService;
        _email = email;
        _status = status;
    }

    /// <summary>The quote, its items, its client and the business; null when the quote is gone.</summary>
    public async Task<QuotePreviewData?> ReadAsync(string quoteId)
    {
        if (await _quoteRepo.GetByIdAsync(quoteId) is not { } quote)
        {
            return null;
        }
        var client = string.IsNullOrEmpty(quote.ClientId) ? null : await _clientRepo.GetByIdAsync(quote.ClientId);
        var profile = await _profileRepo.GetAsync();
        return new QuotePreviewData(quote, quote.LineItems.ToImmutableList(), profile, client);
    }

    /// <summary>The quote as the Send dialog fills it in, or null when it is gone.</summary>
    public async Task<QuoteSendDetails?> GetSendDetailsAsync(string quoteId) =>
        await ReadAsync(quoteId) is { } data
            ? new QuoteSendDetails(data.Quote.Id, data.Quote.QuoteNumber, data.Quote.Title, data.Quote.ClientName,
                data.Client?.Email ?? string.Empty, data.Profile.BusinessName ?? "Our Company", data.Quote.ValidUntil)
            : null;

    /// <summary>
    /// Saves the quote's PDF (<see cref="IShareService"/>); the quote's status does not change.
    /// </summary>
    /// <returns>Where the PDF is when no app opened it, for the page's message line; otherwise null.</returns>
    public async Task<string?> ExportPdfAsync(string quoteId) =>
        await _quoteRepo.GetByIdAsync(quoteId) is { } quote
            ? await _shareService.GenerateAndDownloadPdfAsync(quote)
            : null;

    /// <summary>
    /// Send by email: the PDF, then the email, then the quote marked Sent (<see cref="QuoteStatusService.MarkSentAsync"/>:
    /// not when the client has already answered). Without an email app it throws, and the quote stays as it was.
    /// </summary>
    /// <returns>The PDF's note (<see cref="ExportPdfAsync"/>).</returns>
    public async Task<string?> SendByEmailAsync(string quoteId, string recipient, string subject, string body)
    {
        var note = await ExportPdfAsync(quoteId);
        if (!await _email.ComposeEmailAsync(recipient, subject, body))
        {
            throw new InvalidOperationException("No email app opened, so the quote is not marked as sent.");
        }
        await _status.MarkSentAsync(quoteId);
        return note;
    }
}
