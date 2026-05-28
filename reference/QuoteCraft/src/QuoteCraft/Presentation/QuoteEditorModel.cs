using QuoteCraft.Services;

namespace QuoteCraft.Presentation;

public partial record QuoteEditorModel
{
    private readonly INavigator _navigator;
    private readonly IQuoteRepository _quoteRepo;
    private readonly IClientRepository _clientRepo;
    private readonly ICatalogItemRepository _catalogRepo;
    private readonly IBusinessProfileRepository _profileRepo;
    private readonly IShareService _shareService;
    private readonly string _quoteId;
    private readonly IPhotoService _photoService;
    private readonly IStatusHistoryRepository _statusHistoryRepo;

    // Track initial values for unsaved changes detection
    private readonly string _initialTitle;
    private readonly string _initialNotes;
    private readonly string _initialTaxRate;
    private readonly string _initialClientName;

    public QuoteEditorModel(
        QuoteEntity quote,
        INavigator navigator,
        IQuoteRepository quoteRepo,
        IClientRepository clientRepo,
        ICatalogItemRepository catalogRepo,
        IBusinessProfileRepository profileRepo,
        IShareService shareService,
        IPhotoService photoService,
        IStatusHistoryRepository statusHistoryRepo)
    {
        _navigator = navigator;
        _quoteRepo = quoteRepo;
        _clientRepo = clientRepo;
        _catalogRepo = catalogRepo;
        _profileRepo = profileRepo;
        _shareService = shareService;
        _photoService = photoService;
        _statusHistoryRepo = statusHistoryRepo;
        _quoteId = quote.Id;

        _initialTitle = quote.Title;
        _initialNotes = quote.Notes ?? string.Empty;
        _initialTaxRate = quote.TaxRate.ToString("F1");
        _initialClientName = quote.ClientName ?? string.Empty;

        Title = State<string>.Value(this, () => quote.Title);
        Notes = State<string>.Value(this, () => quote.Notes ?? string.Empty);
        TaxRate = State<string>.Value(this, () => quote.TaxRate.ToString("F1"));
        ClientName = State<string>.Value(this, () => quote.ClientName ?? string.Empty);
        QuoteNumber = State<string>.Value(this, () => quote.QuoteNumber);
    }

    public IState<string> Title { get; }
    public IState<string> Notes { get; }
    public IState<string> TaxRate { get; }
    public IState<string> ClientName { get; }
    public IState<string> QuoteNumber { get; }
    public IState<string> ValidationError => State<string>.Value(this, () => string.Empty);

    public IState<int> LineItemsVersion => State<int>.Value(this, () => 0);
    public IState<int> PhotosVersion => State<int>.Value(this, () => 0);

    public string QuoteId => _quoteId;

    public IListFeed<string> Photos => PhotosVersion
        .SelectAsync(async (_, ct) =>
            (IImmutableList<string>)_photoService.GetPhotos(_quoteId).ToImmutableList())
        .AsListFeed();

    public async ValueTask AddPhotoFromPath(string filePath, CancellationToken ct)
    {
        var result = await _photoService.AddPhotoAsync(_quoteId, filePath);
        if (result is not null)
            await PhotosVersion.UpdateAsync(v => v + 1, ct);
    }

    public int CurrentPhotoCount => _photoService.GetPhotos(_quoteId).Count;

    public int MaxPhotos => _photoService.MaxPhotos;

    public async ValueTask DeletePhoto(string photoPath, CancellationToken ct)
    {
        _photoService.DeletePhoto(photoPath);
        await PhotosVersion.UpdateAsync(v => v + 1, ct);
    }

    public IListFeed<LineItemEntity> LineItems => LineItemsVersion
        .SelectAsync(async (_, ct) =>
            (IImmutableList<LineItemEntity>)(await _quoteRepo.GetLineItemsAsync(_quoteId)).ToImmutableList())
        .AsListFeed();

    public async ValueTask SaveLineItem(LineItemEntity item, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(item.Description)) return;
        item.QuoteId = _quoteId;
        await _quoteRepo.SaveLineItemAsync(item);
        await LineItemsVersion.UpdateAsync(v => v + 1, ct);
    }

    public async ValueTask AddFromCatalog(CatalogItemEntity catalogItem, CancellationToken ct)
    {
        var profile = await _profileRepo.GetAsync();
        var markup = profile.DefaultMarkup;
        var effectivePrice = markup > 0
            ? catalogItem.UnitPrice * (1 + markup / 100m)
            : catalogItem.UnitPrice;

        var item = new LineItemEntity
        {
            QuoteId = _quoteId,
            Description = catalogItem.Description,
            UnitPrice = Math.Round(effectivePrice, 2),
            Quantity = 1,
            SortOrder = (await _quoteRepo.GetLineItemsAsync(_quoteId)).Count,
        };
        await _quoteRepo.SaveLineItemAsync(item);
        await LineItemsVersion.UpdateAsync(v => v + 1, ct);
    }

    public async ValueTask DeleteLineItem(LineItemEntity item, CancellationToken ct)
    {
        await _quoteRepo.DeleteLineItemAsync(item.Id);
        await LineItemsVersion.UpdateAsync(v => v + 1, ct);
    }

    public async ValueTask MoveLineItemUp(LineItemEntity item, CancellationToken ct)
    {
        var items = await _quoteRepo.GetLineItemsAsync(_quoteId);
        var sorted = items.OrderBy(i => i.SortOrder).ToList();
        var index = sorted.FindIndex(i => i.Id == item.Id);
        if (index <= 0) return;

        var prev = sorted[index - 1];
        (prev.SortOrder, item.SortOrder) = (item.SortOrder, prev.SortOrder);
        await _quoteRepo.SaveLineItemsBatchAsync(prev, item);
        await LineItemsVersion.UpdateAsync(v => v + 1, ct);
    }

    public async ValueTask MoveLineItemDown(LineItemEntity item, CancellationToken ct)
    {
        var items = await _quoteRepo.GetLineItemsAsync(_quoteId);
        var sorted = items.OrderBy(i => i.SortOrder).ToList();
        var index = sorted.FindIndex(i => i.Id == item.Id);
        if (index < 0 || index >= sorted.Count - 1) return;

        var next = sorted[index + 1];
        (next.SortOrder, item.SortOrder) = (item.SortOrder, next.SortOrder);
        await _quoteRepo.SaveLineItemsBatchAsync(next, item);
        await LineItemsVersion.UpdateAsync(v => v + 1, ct);
    }

    public async ValueTask Save(CancellationToken ct)
    {
        var title = (await Title)?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(title))
        {
            await ValidationError.UpdateAsync(_ => "Quote title is required.", ct);
            return;
        }

        var taxStr = await TaxRate ?? "0";
        if (!decimal.TryParse(taxStr, out var tax) || tax < 0 || tax > 100)
        {
            await ValidationError.UpdateAsync(_ => "Tax rate must be a number between 0 and 100.", ct);
            return;
        }

        await ValidationError.UpdateAsync(_ => string.Empty, ct);
        await PersistEditsAsync();
        await _navigator.NavigateBackAsync(this);
    }

    public async ValueTask MarkSent(CancellationToken ct)
    {
        await UpdateStatus(QuoteStatus.Sent);
    }

    public async ValueTask MarkAccepted(CancellationToken ct)
    {
        await UpdateStatus(QuoteStatus.Accepted);
    }

    public async ValueTask MarkDeclined(CancellationToken ct)
    {
        await UpdateStatus(QuoteStatus.Declined);
    }

    public async ValueTask ExportPdf(CancellationToken ct)
    {
        await PersistEditsAsync();

        var quote = await _quoteRepo.GetByIdAsync(_quoteId);
        if (quote is null) return;

        await _shareService.ShareQuotePdfAsync(quote);
    }

    public async ValueTask CopyShareLink(CancellationToken ct)
    {
        await PersistEditsAsync();

        var quote = await _quoteRepo.GetByIdAsync(_quoteId);
        if (quote is null) return;

        await _shareService.CopyShareLinkAsync(quote);
    }

    public async ValueTask SendShareLink(CancellationToken ct)
    {
        await PersistEditsAsync();

        var quote = await _quoteRepo.GetByIdAsync(_quoteId);
        if (quote is null) return;

        var link = await _shareService.GenerateShareLinkAsync(quote);

        // Open email with link
        var subject = $"Quote #{quote.QuoteNumber} - {quote.Title}";
        var body = $"Please review your quote at: {link}";
        var mailto = $"mailto:{Uri.EscapeDataString(quote.ClientName ?? "")}?subject={Uri.EscapeDataString(subject)}&body={Uri.EscapeDataString(body)}";
        await Windows.System.Launcher.LaunchUriAsync(new Uri(mailto));

        await _shareService.MarkAsSentAsync(quote);
    }

    public async Task<bool> HasUnsavedChangesAsync()
    {
        return await Title != _initialTitle ||
               await Notes != _initialNotes ||
               await TaxRate != _initialTaxRate ||
               await ClientName != _initialClientName;
    }

    public async ValueTask GoBack(CancellationToken ct)
    {
        await _navigator.NavigateBackAsync(this);
    }

    public Task<List<CatalogItemEntity>> GetCatalogItemsAsync() => _catalogRepo.GetAllAsync();

    public async Task<List<string>> SearchClientNamesAsync(string query)
    {
        var clients = await _clientRepo.GetAllAsync();
        return clients
            .Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Select(c => c.Name)
            .Take(8)
            .ToList();
    }

    private async Task PersistEditsAsync()
    {
        var existing = await _quoteRepo.GetByIdAsync(_quoteId);
        if (existing is null) return;

        existing.Title = await Title ?? "Untitled";
        existing.Notes = await Notes;
        existing.ClientName = await ClientName;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        var taxStr = await TaxRate ?? "0";
        if (decimal.TryParse(taxStr, out var tax))
            existing.TaxRate = tax;

        await _quoteRepo.SaveAsync(existing);
    }

    public IListFeed<StatusHistoryEntry> StatusHistory => LineItemsVersion
        .SelectAsync(async (_, ct) =>
            (IImmutableList<StatusHistoryEntry>)(await _statusHistoryRepo.GetByQuoteIdAsync(_quoteId)).ToImmutableList())
        .AsListFeed();

    private async Task UpdateStatus(QuoteStatus status)
    {
        var existing = await _quoteRepo.GetByIdAsync(_quoteId);
        if (existing is null) return;

        existing.Status = status;
        if (status == QuoteStatus.Sent)
            existing.SentAt = DateTimeOffset.UtcNow;

        existing.UpdatedAt = DateTimeOffset.UtcNow;
        await _quoteRepo.SaveAsync(existing);

        // Record status history
        await _statusHistoryRepo.RecordAsync(_quoteId, status.ToString(), "user");

        await _navigator.NavigateBackAsync(this);
    }
}
