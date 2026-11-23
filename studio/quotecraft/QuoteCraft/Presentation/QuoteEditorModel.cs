namespace QuoteCraft.Presentation;

public partial record QuoteEditorModel
{
    private readonly INavigator _navigator;
    private readonly IQuoteRepository _quoteRepo;
    private readonly IClientRepository _clientRepo;
    private readonly QuoteDelivery _delivery;
    private readonly QuoteLineItemDialogs _lineItems;
    private readonly string _quoteId;
    private readonly IPhotoService _photoService;
    private readonly QuoteStatusService _status;
    private readonly IDispatcher _dispatcher;
    private readonly QuoteBoard _board;

    // The fields as last saved, for Back's unsaved-changes question (Export PDF and the status buttons save too)
    private string _savedTitle;
    private string _savedNotes;
    private string _savedTaxRate;
    private string _savedClientName;
    private DateTimeOffset? _savedValidUntil;

    public QuoteEditorModel(
        QuoteEntity quote,
        INavigator navigator,
        IQuoteRepository quoteRepo,
        IClientRepository clientRepo,
        QuoteDelivery delivery,
        QuoteLineItemDialogs lineItems,
        IPhotoService photoService,
        QuoteStatusService status,
        IDispatcher dispatcher,
        QuoteBoard board)
    {
        _board = board;
        _status = status;
        _dispatcher = dispatcher;
        _navigator = navigator;
        _quoteRepo = quoteRepo;
        _clientRepo = clientRepo;
        _delivery = delivery;
        _lineItems = lineItems;
        _photoService = photoService;
        _quoteId = quote.Id;

        _savedTitle = quote.Title;
        _savedNotes = quote.Notes ?? string.Empty;
        _savedTaxRate = Money.FormatRate(quote.TaxRate);
        _savedClientName = quote.ClientName ?? string.Empty;
        _savedValidUntil = ValidUntilDate.ToPicker(quote.ValidUntil);

        Title = State<string>.Value(this, () => quote.Title);
        Notes = State<string>.Value(this, () => quote.Notes ?? string.Empty);
        TaxRate = State<string>.Value(this, () => Money.FormatRate(quote.TaxRate));
        ClientName = State<string>.Value(this, () => quote.ClientName ?? string.Empty);
        QuoteNumber = State<string>.Value(this, () => quote.QuoteNumber);
        ValidUntil = State<DateTimeOffset?>.Value(this, () => ValidUntilDate.ToPicker(quote.ValidUntil));
    }

    public IState<string> Title { get; }
    public IState<string> Notes { get; }
    public IState<string> TaxRate { get; }
    public IState<string> ClientName { get; }
    public IState<string> QuoteNumber { get; }

    /// <summary>
    /// The day the quote is valid until (PRD F1.8), as the date picker shows it. A new quote starts at the Settings
    /// default (<see cref="QuoteCreationService"/>); the date is stored only when it was changed (<see cref="ValidUntilDate"/>).
    /// </summary>
    public IState<DateTimeOffset?> ValidUntil { get; }

    public IState<string> ValidationError => State<string>.Value(this, () => string.Empty);

    /// <summary>True while Save writes the quote (the page shows a progress bar).</summary>
    public IState<bool> IsSaving => State<bool>.Value(this, () => false);

    public IState<int> LineItemsVersion => State<int>.Value(this, () => 0);
    public IState<int> PhotosVersion => State<int>.Value(this, () => 0);

    public string QuoteId => _quoteId;


    public IListFeed<string> Photos => PhotosVersion
        .SelectAsync(async (_, ct) =>
            (IImmutableList<string>)_photoService.GetPhotos(_quoteId).ToImmutableList())
        .AsListFeed();

    /// <summary>Add Photo: the limit first, then the picker; a failure shows next to the buttons.</summary>
    public ValueTask AddPhoto(CancellationToken ct) => ValidationError.ReportFailureAsync("The photo could not be added.", async () =>
    {
        if (_photoService.GetPhotos(_quoteId).Count >= _photoService.MaxPhotos)
        {
            await _navigator.ConfirmAsync(this, new ConfirmRequest("\uE722", "Photo Limit Reached",
                $"You can attach up to {_photoService.MaxPhotos} photos per quote."), ct);
            return;
        }

        if (await ImagePicker.PickAsync(_dispatcher, ct) is not { } file)
        {
            return;
        }

        string? result;
        using (var source = await file.OpenStreamForReadAsync())
        {
            result = await _photoService.AddPhotoAsync(_quoteId, source, file.FileType);
        }
        if (result is not null)
            await PhotosVersion.UpdateAsync(v => v + 1, ct);
    }, ct);

    /// <summary>A thumbnail's tap: the photo as large as a dialog shows it (PRD F14.8, D22).</summary>
    public ValueTask ViewPhoto(string photoPath, CancellationToken ct) => ValidationError.ReportFailureAsync("The photo could not be opened.", async () =>
        await _navigator.ShowPhotoAsync(this, photoPath, ct), ct);

    /// <summary>A photo's delete button: the file goes and the photos are read again; a failure shows in the message line.</summary>
    public ValueTask DeletePhoto(string photoPath, CancellationToken ct) => ValidationError.ReportFailureAsync("The photo could not be deleted.", async () =>
    {
        _photoService.DeletePhoto(photoPath);
        await PhotosVersion.UpdateAsync(v => v + 1, ct);
    }, ct);

    public IListFeed<LineItemEntity> LineItems => LineItemsVersion
        .SelectAsync(async (_, ct) =>
            (IImmutableList<LineItemEntity>)(await _quoteRepo.GetLineItemsAsync(_quoteId)).ToImmutableList())
        .AsListFeed();

    /// <summary>
    /// Subtotal, tax and total, recomputed when the items change and as the tax rate is typed. The typed text is never
    /// rewritten; until it reads as a rate (0 to 100), the tax line asks for one and the total leaves tax out.
    /// </summary>
    public IFeed<QuoteTotals> Totals => Feed.Combine(LineItemsVersion, TaxRate)
        .SelectAsync(async (inputs, ct) =>
        {
            var (_, rateText) = inputs;
            var rateReads = Money.TryParseRate(rateText, out var rate) && rate <= 100;
            var quote = new QuoteEntity
            {
                TaxRate = rateReads ? rate : 0m,
                LineItems = await _quoteRepo.GetLineItemsAsync(_quoteId),
            };
            var label = rateReads ? $"Tax ({Money.FormatRate(rate)} %)" : "Tax (enter a rate from 0 to 100)";
            return new QuoteTotals(quote.Subtotal, label, quote.TaxAmount, quote.Total);
        });

    public ValueTask AddLineItem(CancellationToken ct) => EditLineItemAsync(null, ct);

    public ValueTask EditLineItem(LineItemEntity item, CancellationToken ct) => EditLineItemAsync(item, ct);

    /// <summary>The line-item dialog (<see cref="QuoteLineItemDialogs"/>): the items are read again when they changed.</summary>
    private ValueTask EditLineItemAsync(LineItemEntity? item, CancellationToken ct) => ValidationError.ReportFailureAsync("The line item could not be saved.", async () =>
    {
        if (await _lineItems.EditAsync(_navigator, this, _quoteId, item, ct))
        {
            await LineItemsChangedAsync(ct);
        }
    }, ct);

    /// <summary>From Catalog: the browser adds items as they are tapped; the list reloads when it closes.</summary>
    public ValueTask OpenCatalog(CancellationToken ct) => ValidationError.ReportFailureAsync("The catalog could not be opened.", async () =>
    {
        await _lineItems.OpenCatalogAsync(_navigator, this, _quoteId, ct);
        await LineItemsChangedAsync(ct);
    }, ct);

    public ValueTask MoveLineItemUp(LineItemEntity item, CancellationToken ct) => MoveLineItemAsync(item, -1, ct);

    public ValueTask MoveLineItemDown(LineItemEntity item, CancellationToken ct) => MoveLineItemAsync(item, +1, ct);

    /// <summary>Swaps the item with its neighbour above or below; a failure shows in the message line.</summary>
    private ValueTask MoveLineItemAsync(LineItemEntity item, int offset, CancellationToken ct) => ValidationError.ReportFailureAsync("The line item could not be moved.", async () =>
    {
        var ids = (await _quoteRepo.GetLineItemsAsync(_quoteId)).Select(i => i.Id).ToList();
        var from = ids.IndexOf(item.Id);
        var to = from + offset;
        if (from < 0 || to < 0 || to >= ids.Count)
        {
            return;
        }

        (ids[from], ids[to]) = (ids[to], ids[from]);
        await _quoteRepo.ReorderLineItemsAsync(_quoteId, ids);
        await LineItemsChangedAsync(ct);
    }, ct);

    /// <summary>The items changed: this page reads them again, and so do the quotes list and detail.</summary>
    private async ValueTask LineItemsChangedAsync(CancellationToken ct)
    {
        await LineItemsVersion.UpdateAsync(v => v + 1, ct);
        _board.QuotesChanged();
    }

    /// <summary>
    /// The checks Save, Export PDF and the status buttons run before they write the edits: a title, a tax rate from 0
    /// to 100, and a changed valid-until date that is not in the past. A failed one says why in the message line, and
    /// nothing is written.
    /// </summary>
    private async ValueTask<bool> ValidateEditsAsync(CancellationToken ct)
    {
        var validUntil = await ValidUntil;
        var error = string.IsNullOrWhiteSpace(await Title) ? "Quote title is required."
            : !Money.TryParseRate(await TaxRate, out var tax) || tax > 100 ? "Tax rate must be a number between 0 and 100."
            : validUntil is { } picked && !ValidUntilDate.SameDay(picked, _savedValidUntil) ? ValidUntilDate.Check(picked, DateTimeOffset.UtcNow)
            : null;
        await ValidationError.UpdateAsync(_ => error ?? string.Empty, ct);
        return error is null;
    }

    public async ValueTask Save(CancellationToken ct)
    {
        if (!await ValidateEditsAsync(ct))
        {
            return;
        }

        await IsSaving.UpdateAsync(_ => true, ct);
        try
        {
            await PersistEditsAsync();
        }
        catch (Exception ex)
        {
            ErrorReporter.Log(ex, "Save quote");
            await ValidationError.UpdateAsync(_ => "The quote could not be saved. Try again.", ct);
            return;
        }
        finally
        {
            await IsSaving.UpdateAsync(_ => false, ct);
        }
        await GoBackAsync(ct);
    }

    /// <summary>Retry after a failed load: read the line items and photos again.</summary>
    public async ValueTask Reload(CancellationToken ct)
    {
        await LineItemsVersion.UpdateAsync(v => v + 1, ct);
        await PhotosVersion.UpdateAsync(v => v + 1, ct);
    }

    public ValueTask MarkSent(CancellationToken ct) => UpdateStatus(QuoteStatus.Sent, ct);

    public ValueTask MarkAccepted(CancellationToken ct) => UpdateStatus(QuoteStatus.Accepted, ct);

    public ValueTask MarkDeclined(CancellationToken ct) => UpdateStatus(QuoteStatus.Declined, ct);

    /// <summary>
    /// Checks and saves the edits as Save does, then saves the PDF; a failure shows on the page, next to the button.
    /// </summary>
    public async ValueTask ExportPdf(CancellationToken ct)
    {
        if (!await ValidateEditsAsync(ct))
        {
            return;
        }

        try
        {
            await PersistEditsAsync();
            // When nothing opened the saved file, the message line says where it is.
            if (await _delivery.ExportPdfAsync(_quoteId) is { Length: > 0 } note)
            {
                await ValidationError.UpdateAsync(_ => note, ct);
            }
        }
        catch (Exception ex)
        {
            ErrorReporter.Log(ex, "Export PDF");
            await ValidationError.UpdateAsync(_ => ErrorReporter.Describe("The PDF could not be created.", ex), ct);
        }
    }

    private async Task<bool> HasUnsavedChangesAsync()
    {
        return await Title != _savedTitle ||
               await Notes != _savedNotes ||
               await TaxRate != _savedTaxRate ||
               await ClientName != _savedClientName ||
               !ValidUntilDate.SameDay(await ValidUntil, _savedValidUntil);
    }

    /// <summary>The back arrow: asks before dropping unsaved edits.</summary>
    public ValueTask Back(CancellationToken ct) => ValidationError.ReportFailureAsync("The editor could not close.", async () =>
    {
        if (await HasUnsavedChangesAsync()
            && !await _navigator.ConfirmAsync(this, new ConfirmRequest("\uE7BA", "Unsaved Changes",
                "You have unsaved changes. Are you sure you want to leave?", "Discard", "Stay"), ct))
        {
            return;
        }
        await GoBackAsync(ct);
    }, ct);

    /// <summary>
    /// Back to where the editor was opened from; to the quotes list when there is nothing to go back to (the editor
    /// opened straight after onboarding).
    /// </summary>
    private async ValueTask GoBackAsync(CancellationToken ct)
    {
        var response = await _navigator.NavigateBackAsync(this, cancellation: ct);
        if (response is not { Success: true })
        {
            await _navigator.NavigateRouteAsync(this, "Dashboard", cancellation: ct);
        }
    }

    /// <summary>Clients whose name contains what is typed in the Client field (<see cref="ClientLink.SuggestAsync"/>).</summary>
    public IListFeed<string> ClientSuggestions => ClientName
        .SelectAsync(async (typed, ct) => await ClientLink.SuggestAsync(typed, _clientRepo))
        .AsListFeed();

    /// <summary>Writes the fields (checked first: <see cref="ValidateEditsAsync"/>) and remembers them as saved.</summary>
    private async Task PersistEditsAsync()
    {
        var existing = await _quoteRepo.GetByIdAsync(_quoteId);
        if (existing is null) return;

        var title = await Title ?? string.Empty;
        var notes = await Notes ?? string.Empty;
        var rateText = await TaxRate ?? string.Empty;
        var typedClient = await ClientName ?? string.Empty;
        var validUntil = await ValidUntil;

        existing.Title = title;
        existing.Notes = notes;

        // An untouched date keeps its exact time; a picked day is valid to its end.
        if (validUntil is { } picked && !ValidUntilDate.SameDay(picked, _savedValidUntil))
            existing.ValidUntil = ValidUntilDate.FromPicker(picked);

        // A changed name links the quote to the client with that name (or to none), so the client's counts and the
        // Send address follow the name on the quote.
        var clientName = typedClient.Trim();
        existing.ClientId = ClientLink.Resolve(clientName, existing.ClientName, existing.ClientId, await _clientRepo.GetAllAsync());
        existing.ClientName = string.IsNullOrEmpty(clientName) ? null : clientName;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        if (Money.TryParseRate(rateText, out var tax))
            existing.TaxRate = tax;

        await _quoteRepo.SaveAsync(existing);
        _board.QuotesChanged();
        (_savedTitle, _savedNotes, _savedTaxRate, _savedClientName) = (title, notes, rateText, typedClient);
        _savedValidUntil = validUntil ?? _savedValidUntil;
    }

    /// <summary>When the quote was sent, accepted, declined, expired or reopened (PRD F3.4), oldest first.</summary>
    public IListFeed<StatusHistoryEntry> StatusHistory => LineItemsVersion
        .SelectAsync(async (_, ct) =>
            (IImmutableList<StatusHistoryEntry>)(await _status.GetHistoryAsync(_quoteId)).ToImmutableList())
        .AsListFeed();

    /// <summary>Mark Sent, Accepted or Declined: the edits are checked and saved as Save does, then the status changes.</summary>
    private async ValueTask UpdateStatus(QuoteStatus status, CancellationToken ct)
    {
        if (!await ValidateEditsAsync(ct))
        {
            return;
        }

        try
        {
            // The edits first, so a status change does not drop them.
            await PersistEditsAsync();
            await _status.SetStatusAsync(_quoteId, status);
            _board.QuotesChanged();
        }
        catch (Exception ex)
        {
            ErrorReporter.Log(ex, $"Mark {status}");
            await ValidationError.UpdateAsync(_ => ErrorReporter.Describe($"The quote could not be marked {status}.", ex), ct);
            return;
        }
        await GoBackAsync(ct);
    }
}
