namespace QuoteCraft.Presentation;

/// <summary>
/// A quote's line-item dialog and Add from Catalog, shared by the quote editor and the open quote on the Quotes page.
/// Each returns once the quote's items are written; the caller reads them again.
/// </summary>
public sealed class QuoteLineItemDialogs
{
    private readonly IQuoteRepository _quoteRepo;

    public QuoteLineItemDialogs(IQuoteRepository quoteRepo)
    {
        _quoteRepo = quoteRepo;
    }

    /// <summary>
    /// The line-item dialog (route <c>!LineItemEditor</c>): a new item (<paramref name="item"/> null) goes last, an
    /// edited one keeps its place, and Delete removes it.
    /// </summary>
    /// <returns>True when the quote's items changed; false when the dialog was cancelled.</returns>
    public async Task<bool> EditAsync(INavigator navigator, object sender, string quoteId, LineItemEntity? item, CancellationToken ct)
    {
        if (await navigator.ShowDialogAsync<LineItemEditResult>(sender, "LineItemEditor", new LineItemEditRequest(item), ct) is not { } result)
        {
            return false;
        }

        if (result.Saved is { } saved)
        {
            saved.QuoteId = quoteId;
            await _quoteRepo.SaveLineItemAsync(saved);
        }
        else if (result.DeletedId is { Length: > 0 } deletedId)
        {
            await _quoteRepo.DeleteLineItemAsync(deletedId);
        }
        return true;
    }

    /// <summary>Add from Catalog (route <c>!CatalogBrowser</c>): the browser adds items as they are tapped; this returns when it closes.</summary>
    public async Task OpenCatalogAsync(INavigator navigator, object sender, string quoteId, CancellationToken ct) =>
        await navigator.ShowDialogAsync<object>(sender, "CatalogBrowser", new CatalogBrowserRequest(quoteId), ct);
}
