namespace QuoteCraft.Presentation;

/// <summary>What the user chose in Send Quote: an email to compose (with the PDF saved first), or the PDF only.</summary>
public record SendQuoteChoice(bool PdfOnly, string Recipient = "", string Subject = "", string Body = "");

/// <summary>Send Quote (route <c>!SendQuote</c>), filled from the quote, its client and the business.</summary>
public partial record SendQuoteModel
{
    private readonly INavigator _navigator;

    public SendQuoteModel(INavigator navigator, QuoteSendDetails quote)
    {
        _navigator = navigator;
        Heading = $"Send {quote.QuoteNumber} to {quote.ClientName ?? "client"}";
        Recipient = State<string>.Value(this, () => quote.ClientEmail);
        Subject = State<string>.Value(this, () => $"Quote {quote.QuoteNumber} - {quote.Title}");
        Body = State<string>.Value(this, () =>
            $"Hi {quote.ClientName ?? "there"},\n\nPlease find attached quote {quote.QuoteNumber} for \"{quote.Title}\".\n\n" +
            $"The quote is valid until {quote.ValidUntil?.ToString("MMMM d, yyyy") ?? "further notice"}.\n\n" +
            $"Please don't hesitate to reach out with any questions.\n\nBest regards,\n{quote.BusinessName}");
    }

    public string Heading { get; }

    public IState<string> Recipient { get; }
    public IState<string> Subject { get; }
    public IState<string> Body { get; }

    public async ValueTask SendByEmail(CancellationToken ct) =>
        await _navigator.NavigateBackWithResultAsync(this, data: new SendQuoteChoice(
            PdfOnly: false,
            (await Recipient)?.Trim() ?? string.Empty,
            (await Subject)?.Trim() ?? string.Empty,
            (await Body)?.Trim() ?? string.Empty), cancellation: ct);

    public async ValueTask DownloadPdfOnly(CancellationToken ct) =>
        await _navigator.NavigateBackWithResultAsync(this, data: new SendQuoteChoice(PdfOnly: true), cancellation: ct);
}
