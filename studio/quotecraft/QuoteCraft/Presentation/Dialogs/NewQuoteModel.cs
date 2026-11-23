namespace QuoteCraft.Presentation;

/// <summary>New Quote (route <c>!NewQuote</c>): a title and an optional client; Create saves a Draft and returns it.</summary>
public partial record NewQuoteModel
{
    private readonly INavigator _navigator;
    private readonly IClientRepository _clientRepo;
    private readonly QuoteCreationService _creation;

    public NewQuoteModel(INavigator navigator, IClientRepository clientRepo, QuoteCreationService creation)
    {
        _navigator = navigator;
        _clientRepo = clientRepo;
        _creation = creation;
    }

    private const string TitleRequired = "Enter a title for the quote.";

    /// <summary>The title; "Enter a title" goes as soon as one is typed.</summary>
    public IState<string> QuoteTitle => State<string>.Value(this, () => string.Empty).ForEach(ClearTitleMessageAsync);

    private async ValueTask ClearTitleMessageAsync(string? title, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            await ValidationError.UpdateAsync(error => error == TitleRequired ? string.Empty : error, ct);
        }
    }

    public IState<string> ClientName => State<string>.Value(this, () => string.Empty);

    public IState<string> ValidationError => State<string>.Value(this, () => string.Empty);

    /// <summary>Clients whose name contains what is typed (<see cref="ClientLink.SuggestAsync"/>).</summary>
    public IListFeed<string> ClientSuggestions => ClientName
        .SelectAsync(async (typed, ct) => await ClientLink.SuggestAsync(typed, _clientRepo))
        .AsListFeed();

    /// <summary>A Draft linked to the named client (if there is one); the caller opens it in the editor.</summary>
    public async ValueTask Create(CancellationToken ct)
    {
        var title = (await QuoteTitle)?.Trim() ?? string.Empty;
        if (title.Length == 0)
        {
            await ValidationError.UpdateAsync(_ => TitleRequired, ct);
            return;
        }

        try
        {
            var quote = await _creation.CreateDraftAsync(title, await ClientName);
            await _navigator.NavigateBackWithResultAsync(this, data: quote, cancellation: ct);
        }
        catch (Exception ex)
        {
            ErrorReporter.Log(ex, "Create quote");
            await ValidationError.UpdateAsync(_ => ErrorReporter.Describe("The quote could not be created.", ex), ct);
        }
    }
}
