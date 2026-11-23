namespace QuoteCraft.Presentation;

/// <summary>
/// The open quote on the Quotes page (route <c>./QuoteDetail</c>, the page's detail region): its detail and print
/// preview, inline edits of line items and notes, and its actions (send, PDF, duplicate, delete, reopen, the full
/// editor). The list is <see cref="DashboardModel"/>; the two share <see cref="QuoteBoard"/>.
/// </summary>
public partial record QuoteDetailModel
{
    private readonly string _quoteId;
    private readonly INavigator _navigator;
    private readonly IQuoteRepository _quoteRepo;
    private readonly QuoteDelivery _delivery;
    private readonly QuoteLineItemDialogs _lineItems;
    private readonly QuoteCreationService _creation;
    private readonly IFeatureGateService _featureGate;
    private readonly QuoteStatusService _status;
    private readonly QuoteBoard _board;

    public QuoteDetailModel(
        QuoteEntity? quote,
        INavigator navigator,
        IQuoteRepository quoteRepo,
        QuoteDelivery delivery,
        QuoteLineItemDialogs lineItems,
        QuoteCreationService creation,
        IFeatureGateService featureGate,
        QuoteStatusService status,
        QuoteBoard board)
    {
        // Navigation can restore the detail region's last route without its data (back from the full editor, when the
        // Quotes page is made again): there is no quote to show then, and the new page has none open (DashboardModel).
        _quoteId = quote?.Id ?? string.Empty;
        _navigator = navigator;
        _quoteRepo = quoteRepo;
        _delivery = delivery;
        _lineItems = lineItems;
        _creation = creation;
        _featureGate = featureGate;
        _status = status;
        _board = board;
    }

    public string QuoteId => _quoteId;

    /// <summary>Edit (line items and notes in place) is on; Done saves the notes.</summary>
    public IState<bool> IsEditMode => State<bool>.Value(this, () => false);

    public IState<string> EditingNotes => State<string>.Value(this, () => string.Empty);

    public IState<bool> IsPreviewMode => State<bool>.Value(this, () => false);

    /// <summary>Shown when a plan limit stopped Duplicate (only where plans exist, D3).</summary>
    public IState<string> UpgradeMessage => State<string>.Value(this, () => string.Empty);

    /// <summary>What the last action could not do (a PDF that could not be written, say); shown in a banner.</summary>
    public IState<string> ActionError => State<string>.Value(this, () => string.Empty);

    /// <summary>
    /// The quote, its items, its client and the business, read again whenever quotes change; none when the quote is
    /// gone (the page shows its empty state).
    /// </summary>
    private IFeed<QuotePreviewData> Data => Feed.AsyncEnumerable(_board.WatchAsync)
        .Select(state => state.Version)
        .SelectDataAsync(async (_, ct) => await _delivery.ReadAsync(_quoteId) is { } data
            ? Option.Some(data)
            : Option.None<QuotePreviewData>());

    public IFeed<QuoteDetail> Detail => Data.Select(d => new QuoteDetail(d.Quote, d.LineItems, d.Client));

    public IFeed<QuotePreviewData> Preview => Data;

    public async ValueTask DismissError(CancellationToken ct) => await ActionError.UpdateAsync(_ => string.Empty, ct);

    public async ValueTask DismissUpgrade(CancellationToken ct) => await UpgradeMessage.UpdateAsync(_ => string.Empty, ct);

    /// <summary>Retry after a failed read: every view of the quotes reads them again.</summary>
    public ValueTask Refresh()
    {
        _board.QuotesChanged();
        return ValueTask.CompletedTask;
    }

    /// <summary>One-pane layouts (below 1080): close the quote and show the list again.</summary>
    public ValueTask BackToList()
    {
        _board.Close();
        return ValueTask.CompletedTask;
    }

    public async ValueTask PreviewQuote(CancellationToken ct) => await IsPreviewMode.UpdateAsync(_ => true, ct);

    public async ValueTask BackToDetail(CancellationToken ct) => await IsPreviewMode.UpdateAsync(_ => false, ct);

    /// <summary>The full editor (title, client, tax rate), with the quote as stored now.</summary>
    public ValueTask EditQuote(CancellationToken ct) => ActionError.ReportFailureAsync("The quote could not be opened.", async () =>
    {
        if (await _quoteRepo.GetByIdAsync(_quoteId) is { } quote)
        {
            await _navigator.NavigateRouteAsync(this, "QuoteEditor", data: quote, cancellation: ct);
        }
    }, ct);

    /// <summary>Plans and the upgrade live in Settings.</summary>
    public async ValueTask OpenSettings(CancellationToken ct) =>
        await _navigator.NavigateRouteAsync(this, "Settings", cancellation: ct);

    /// <summary>The expired banner's Reopen: back to Draft, valid again, ready to edit and send (PRD F17.3).</summary>
    public ValueTask ReopenQuote(CancellationToken ct) => ActionError.ReportFailureAsync("The quote could not be reopened.", async () =>
    {
        if (await _status.ReopenAsync(_quoteId) is not null)
        {
            _board.QuotesChanged();
        }
    }, ct);

    public ValueTask DeleteQuote(CancellationToken ct) => ActionError.ReportFailureAsync("The quote could not be deleted.", async () =>
    {
        if (!await _navigator.ConfirmAsync(this, new ConfirmRequest("", "Delete Quote?",
                "This quote and its line items will be removed from your quotes.", "Delete", "Cancel"), ct))
        {
            return;
        }

        await _quoteRepo.DeleteAsync(_quoteId);
        _board.Close();
        _board.QuotesChanged();
    }, ct);

    /// <summary>
    /// Saves the quote's PDF; the quote's status does not change. When nothing opened the saved file, the banner says
    /// where it is.
    /// </summary>
    public ValueTask DownloadPdf(CancellationToken ct) => ActionError.ReportFailureAsync("The PDF could not be created.", async () =>
        await ShowNoteAsync(await _delivery.ExportPdfAsync(_quoteId), ct), ct);

    /// <summary>
    /// Send: the dialog, then either the PDF alone, or the PDF, the email and the quote marked Sent unless the client has
    /// already answered (<see cref="QuoteStatusService"/> keeps a valid-until date that is ahead). Without an email app
    /// the quote stays as it was.
    /// </summary>
    public ValueTask SendQuote(CancellationToken ct) => ActionError.ReportFailureAsync("The quote could not be sent.", async () =>
    {
        if (await _delivery.GetSendDetailsAsync(_quoteId) is not { } details
            || await _navigator.ShowDialogAsync<SendQuoteChoice>(this, "SendQuote", details, ct) is not { } choice)
        {
            return;
        }

        if (choice.PdfOnly)
        {
            await ShowNoteAsync(await _delivery.ExportPdfAsync(_quoteId), ct);
            return;
        }

        var note = await _delivery.SendByEmailAsync(_quoteId, choice.Recipient, choice.Subject, choice.Body);
        _board.QuotesChanged();
        await ShowNoteAsync(note, ct);
    }, ct);

    /// <summary>What the user needs to know about the saved PDF (where it is, when no app opened it), in the banner.</summary>
    private async ValueTask ShowNoteAsync(string? note, CancellationToken ct)
    {
        if (note is { Length: > 0 })
        {
            await ActionError.UpdateAsync(_ => note, ct);
        }
    }

    /// <summary>A Draft copy with the same items, opened in the editor; the plan limit first (only where plans exist).</summary>
    public ValueTask DuplicateQuote(CancellationToken ct) => ActionError.ReportFailureAsync("The quote could not be duplicated.", async () =>
    {
        if (!await _featureGate.CanCreateQuoteAsync())
        {
            await UpgradeMessage.UpdateAsync(_ => _featureGate.GetUpgradeMessage("quotes"), ct);
            return;
        }

        if (await _creation.DuplicateAsync(_quoteId) is { } copy)
        {
            _board.QuotesChanged();
            await _navigator.NavigateRouteAsync(this, "QuoteEditor", data: copy, cancellation: ct);
        }
    }, ct);

    /// <summary>Edit: the notes go into the editor box, and taps on line items open them.</summary>
    public ValueTask StartEdit(CancellationToken ct) => ActionError.ReportFailureAsync("The quote could not be opened for editing.", async () =>
    {
        var quote = await _quoteRepo.GetByIdAsync(_quoteId);
        await EditingNotes.UpdateAsync(_ => quote?.Notes ?? string.Empty, ct);
        await IsEditMode.UpdateAsync(_ => true, ct);
    }, ct);

    /// <summary>Done: saves the notes if they changed.</summary>
    public ValueTask FinishEdit(CancellationToken ct) => ActionError.ReportFailureAsync("Your notes could not be saved.", async () =>
    {
        if (await _quoteRepo.GetByIdAsync(_quoteId) is { } quote)
        {
            var notes = await EditingNotes ?? string.Empty;
            if (quote.Notes != notes)
            {
                quote.Notes = notes;
                await _quoteRepo.SaveAsync(quote);
                _board.QuotesChanged();
            }
        }
        await IsEditMode.UpdateAsync(_ => false, ct);
    }, ct);

    public ValueTask AddLineItem(CancellationToken ct) => EditLineItemAsync(null, ct);

    /// <summary>A tap on a line item: opens it while editing, does nothing otherwise.</summary>
    public async ValueTask EditLineItem(LineItemRow row, CancellationToken ct)
    {
        if (await IsEditMode)
        {
            await EditLineItemAsync(row.Item, ct);
        }
    }

    /// <summary>The line-item dialog (<see cref="QuoteLineItemDialogs"/>): every view of the quote reads it again.</summary>
    private ValueTask EditLineItemAsync(LineItemEntity? item, CancellationToken ct) => ActionError.ReportFailureAsync("The line item could not be saved.", async () =>
    {
        if (await _lineItems.EditAsync(_navigator, this, _quoteId, item, ct))
        {
            _board.QuotesChanged();
        }
    }, ct);

    /// <summary>From Catalog: the browser adds items as they are tapped; the quote reloads when it closes.</summary>
    public ValueTask OpenCatalog(CancellationToken ct) => ActionError.ReportFailureAsync("The catalog could not be opened.", async () =>
    {
        await _lineItems.OpenCatalogAsync(_navigator, this, _quoteId, ct);
        _board.QuotesChanged();
    }, ct);
}
