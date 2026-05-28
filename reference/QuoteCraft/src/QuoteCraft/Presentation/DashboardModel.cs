namespace QuoteCraft.Presentation;

public partial record DashboardModel
{
    private readonly INavigator _navigator;
    private readonly IQuoteRepository _quoteRepo;
    private readonly IClientRepository _clientRepo;
    private readonly IBusinessProfileRepository _profileRepo;
    private readonly ICatalogItemRepository _catalogRepo;
    private readonly QuoteNumberGenerator _quoteNumberGen;
    private readonly Services.IShareService _shareService;
    private readonly Services.IFeatureGateService _featureGate;
    private readonly Services.IEmailLauncherService _emailService;

    public DashboardModel(
        INavigator navigator,
        IQuoteRepository quoteRepo,
        IClientRepository clientRepo,
        IBusinessProfileRepository profileRepo,
        ICatalogItemRepository catalogRepo,
        QuoteNumberGenerator quoteNumberGen,
        Services.IShareService shareService,
        Services.IFeatureGateService featureGate,
        Services.IEmailLauncherService emailService)
    {
        _navigator = navigator;
        _quoteRepo = quoteRepo;
        _clientRepo = clientRepo;
        _profileRepo = profileRepo;
        _catalogRepo = catalogRepo;
        _quoteNumberGen = quoteNumberGen;
        _shareService = shareService;
        _featureGate = featureGate;
        _emailService = emailService;
    }

    public IState<string> SelectedFilter => State<string>.Value(this, () => "All");
    public IState<string> SearchText => State<string>.Value(this, () => string.Empty);
    public IState<QuoteEntity> SelectedQuote => State<QuoteEntity>.Empty(this);
    public IState<bool> IsPreviewMode => State<bool>.Value(this, () => false);
    public IState<int> DetailVersion => State<int>.Value(this, () => 0);

    // Feature gate state: shown when a limit is hit
    public IState<string> UpgradeMessage => State<string>.Value(this, () => string.Empty);

    // Cached quote list: refreshed once per DetailVersion bump, shared across derived feeds
    private List<QuoteEntity>? _cachedQuotes;
    private int _cachedVersion = -1;

    private async Task<List<QuoteEntity>> GetCachedQuotesAsync(int version)
    {
        if (_cachedQuotes is not null && _cachedVersion == version)
            return _cachedQuotes;
        _cachedQuotes = await _quoteRepo.GetAllAsync();
        _cachedVersion = version;
        return _cachedQuotes;
    }

    private IListFeed<QuoteEntity>? _quotesFeed;

    public IListFeed<QuoteEntity> Quotes => _quotesFeed ??=
        Feed.Combine(SelectedFilter, SearchText, DetailVersion)
            .SelectAsync(async (inputs, ct) =>
            {
                var (filter, search, version) = inputs;
                var all = await GetCachedQuotesAsync(version);

                IEnumerable<QuoteEntity> filtered = all;

                if (!string.IsNullOrEmpty(filter) && filter != "All")
                    filtered = filtered.Where(q => Enum.TryParse<QuoteStatus>(filter, out var s) && q.Status == s);

                if (!string.IsNullOrWhiteSpace(search))
                    filtered = filtered.Where(q =>
                        q.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        (q.ClientName ?? "").Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        q.QuoteNumber.Contains(search, StringComparison.OrdinalIgnoreCase));

                return (IImmutableList<QuoteEntity>)filtered.ToImmutableList();
            })
            .AsListFeed();

    public IFeed<int> TotalCount => DetailVersion
        .SelectAsync(async (version, ct) => (await GetCachedQuotesAsync(version)).Count);

    public IFeed<QuoteStatusCounts> StatusCounts => DetailVersion
        .SelectAsync(async (version, ct) =>
        {
            var all = await GetCachedQuotesAsync(version);
            return new QuoteStatusCounts(
                all.Count,
                all.Count(q => q.Status == QuoteStatus.Draft),
                all.Count(q => q.Status == QuoteStatus.Sent),
                all.Count(q => q.Status == QuoteStatus.Viewed),
                all.Count(q => q.Status == QuoteStatus.Accepted),
                all.Count(q => q.Status == QuoteStatus.Declined),
                all.Count(q => q.Status == QuoteStatus.Expired));
        });

    // Analytics: computed from all quotes, with 7-day sparkline data
    public IFeed<DashboardAnalytics> Analytics => DetailVersion
        .SelectAsync(async (version, ct) =>
        {
            var all = await GetCachedQuotesAsync(version);
            var now = DateTimeOffset.UtcNow;
            var startOfMonth = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero);

            var thisMonth = all.Where(q => q.CreatedAt >= startOfMonth).ToList();

            var totalQuoted = thisMonth.Sum(q => q.Total);
            var sent = thisMonth.Count(q => q.Status != QuoteStatus.Draft);

            var resolved = all.Count(q =>
                q.Status == QuoteStatus.Accepted || q.Status == QuoteStatus.Declined);
            var accepted = all.Count(q => q.Status == QuoteStatus.Accepted);
            var rate = resolved > 0 ? Math.Round((double)accepted / resolved * 100, 1) : 0;

            // 7-day sparkline: group quotes by day bucket once, then index
            const int days = 7;
            var quotedDaily = new double[days];
            var sentDaily = new double[days];
            var acceptanceDaily = new double[days];
            var baseDate = now.Date.AddDays(-(days - 1));

            var byDay = all.Where(q => q.CreatedAt >= baseDate)
                .GroupBy(q => (int)(q.CreatedAt.Date - baseDate).TotalDays)
                .Where(g => g.Key >= 0 && g.Key < days)
                .ToDictionary(g => g.Key, g => g.ToList());

            var allResolved = all.Where(q =>
                q.Status == QuoteStatus.Accepted || q.Status == QuoteStatus.Declined).ToList();
            var allAccepted = allResolved.Count(q => q.Status == QuoteStatus.Accepted);
            var baseRate = allResolved.Count > 0
                ? Math.Round((double)allAccepted / allResolved.Count * 100, 1) : 0;

            for (int i = 0; i < days; i++)
            {
                if (byDay.TryGetValue(i, out var dayQuotes))
                {
                    quotedDaily[i] = (double)dayQuotes.Sum(q => q.Total);
                    sentDaily[i] = dayQuotes.Count(q => q.Status != QuoteStatus.Draft);
                }
                acceptanceDaily[i] = baseRate;
            }

            return new DashboardAnalytics(totalQuoted, sent, rate,
                quotedDaily, sentDaily, acceptanceDaily);
        });

    // Shared detail data: single DB fetch for both detail and preview views
    private IFeed<QuotePreviewData> QuoteDetailData =>
        Feed.Combine(SelectedQuote, DetailVersion)
            .SelectAsync(async (inputs, ct) =>
            {
                var (quote, _) = inputs;
                var freshQuote = await _quoteRepo.GetByIdAsync(quote.Id) ?? quote;
                ClientEntity? client = null;
                if (!string.IsNullOrEmpty(freshQuote.ClientId))
                    client = await _clientRepo.GetByIdAsync(freshQuote.ClientId);
                var profile = await _profileRepo.GetAsync();
                return new QuotePreviewData(freshQuote, freshQuote.LineItems.ToImmutableList(), profile, client);
            });

    public IFeed<QuoteDetail> SelectedQuoteDetail => QuoteDetailData
        .SelectAsync(async (d, ct) => new QuoteDetail(d.Quote, d.LineItems, d.Client));

    public IFeed<QuotePreviewData> PreviewData => QuoteDetailData;

    public async ValueTask RefreshDetail(CancellationToken ct)
    {
        await DetailVersion.UpdateAsync(v => v + 1, ct);
    }

    public async ValueTask ExpireOverdueQuotes(CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var expirable = await _quoteRepo.GetExpirableAsync(now);

        foreach (var q in expirable)
        {
            q.Status = QuoteStatus.Expired;
            q.UpdatedAt = DateTimeOffset.UtcNow;
            await _quoteRepo.SaveAsync(q);
        }

        if (expirable.Count > 0)
            await DetailVersion.UpdateAsync(v => v + 1, ct);
    }

    public async ValueTask ReopenQuote(CancellationToken ct)
    {
        var quote = await SelectedQuote;
        if (quote is not null && quote.Status == QuoteStatus.Expired)
        {
            var fresh = await _quoteRepo.GetByIdAsync(quote.Id);
            if (fresh is not null)
            {
                fresh.Status = QuoteStatus.Draft;
                fresh.ValidUntil = DateTimeOffset.UtcNow.AddDays(
                    (await _profileRepo.GetAsync()).QuoteValidDays);
                fresh.UpdatedAt = DateTimeOffset.UtcNow;
                await _quoteRepo.SaveAsync(fresh);
                await SelectedQuote.UpdateAsync(_ => fresh, ct);
                await DetailVersion.UpdateAsync(v => v + 1, ct);
            }
        }
    }

    public async ValueTask OpenQuote(QuoteEntity quote, CancellationToken ct)
    {
        await IsPreviewMode.UpdateAsync(_ => false, ct);
        await SelectedQuote.UpdateAsync(_ => quote, ct);
    }

    public async ValueTask EditQuote(CancellationToken ct)
    {
        var quote = await SelectedQuote;
        if (quote is not null)
            await _navigator.NavigateRouteAsync(this, "QuoteEditor", data: quote!);
    }

    public async ValueTask DeleteQuote(CancellationToken ct)
    {
        var quote = await SelectedQuote;
        if (quote is not null)
        {
            await _quoteRepo.DeleteAsync(quote.Id);
            await SelectedQuote.UpdateAsync(_ => null!, ct);
            await DetailVersion.UpdateAsync(v => v + 1, ct);
        }
    }

    public async ValueTask PreviewQuote(CancellationToken ct)
    {
        await IsPreviewMode.UpdateAsync(_ => true, ct);
    }

    public async ValueTask BackToDetail(CancellationToken ct)
    {
        await IsPreviewMode.UpdateAsync(_ => false, ct);
    }

    public async ValueTask DownloadPdf(CancellationToken ct)
    {
        var quote = await SelectedQuote;
        if (quote is not null)
        {
            var freshQuote = await _quoteRepo.GetByIdAsync(quote.Id);
            if (freshQuote is not null)
            {
                await _shareService.ShareQuotePdfAsync(freshQuote);
                await DetailVersion.UpdateAsync(v => v + 1, ct);
            }
        }
    }

    public async ValueTask CreateQuote(CancellationToken ct)
    {
        // Feature gate: check quote limit
        if (!await _featureGate.CanCreateQuoteAsync())
        {
            await UpgradeMessage.UpdateAsync(_ => _featureGate.GetUpgradeMessage("quotes"), ct);
            return;
        }

        var profile = await _profileRepo.GetAsync();
        var quoteNumber = await _quoteNumberGen.GenerateAsync();
        var quote = new QuoteEntity
        {
            Title = "New Quote",
            QuoteNumber = quoteNumber,
            Status = QuoteStatus.Draft,
            TaxRate = profile.DefaultTaxRate,
            ValidUntil = DateTimeOffset.UtcNow.AddDays(profile.QuoteValidDays),
        };
        await _quoteRepo.SaveAsync(quote);
        await _navigator.NavigateRouteAsync(this, "QuoteEditor", data: quote);
    }

    public async ValueTask DuplicateQuote(CancellationToken ct)
    {
        var quote = await SelectedQuote;
        if (quote is null) return;

        // Feature gate: check quote limit
        if (!await _featureGate.CanCreateQuoteAsync())
        {
            await UpgradeMessage.UpdateAsync(_ => _featureGate.GetUpgradeMessage("quotes"), ct);
            return;
        }

        var freshQuote = await _quoteRepo.GetByIdAsync(quote.Id);
        if (freshQuote is null) return;

        var quoteNumber = await _quoteNumberGen.GenerateAsync();
        var newQuote = new QuoteEntity
        {
            Title = freshQuote.Title + " (Copy)",
            QuoteNumber = quoteNumber,
            Status = QuoteStatus.Draft,
            ClientId = freshQuote.ClientId,
            ClientName = freshQuote.ClientName,
            Notes = freshQuote.Notes,
            TaxRate = freshQuote.TaxRate,
            ValidUntil = DateTimeOffset.UtcNow.AddDays(
                (await _profileRepo.GetAsync()).QuoteValidDays),
        };
        await _quoteRepo.SaveAsync(newQuote);

        // Clone line items
        foreach (var item in freshQuote.LineItems)
        {
            var newItem = new LineItemEntity
            {
                QuoteId = newQuote.Id,
                Description = item.Description,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                SortOrder = item.SortOrder,
            };
            await _quoteRepo.SaveLineItemAsync(newItem);
        }

        // Open the duplicated quote in the editor
        await _navigator.NavigateRouteAsync(this, "QuoteEditor", data: newQuote);
    }

    public async ValueTask DismissUpgrade(CancellationToken ct)
    {
        await UpgradeMessage.UpdateAsync(_ => string.Empty, ct);
    }

    // ── Inline Edit Support ──────────────────────────────────────────────

    public async ValueTask SaveInlineNotes(string notes, CancellationToken ct)
    {
        var quote = await SelectedQuote;
        if (quote is null) return;

        var fresh = await _quoteRepo.GetByIdAsync(quote.Id);
        if (fresh is not null && fresh.Notes != notes)
        {
            fresh.Notes = notes;
            await _quoteRepo.SaveAsync(fresh);
            await DetailVersion.UpdateAsync(v => v + 1, ct);
        }
    }

    public async ValueTask SaveInlineLineItem(LineItemEntity item, CancellationToken ct)
    {
        var quote = await SelectedQuote;
        if (quote is null || string.IsNullOrWhiteSpace(item.Description)) return;

        item.QuoteId = quote.Id;
        if (string.IsNullOrEmpty(item.Id))
            item.SortOrder = (await _quoteRepo.GetByIdAsync(quote.Id))?.LineItems.Count ?? 0;

        await _quoteRepo.SaveLineItemAsync(item);
        await DetailVersion.UpdateAsync(v => v + 1, ct);
    }

    public async ValueTask DeleteInlineLineItem(string lineItemId, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(lineItemId)) return;
        await _quoteRepo.DeleteLineItemAsync(lineItemId);
        await DetailVersion.UpdateAsync(v => v + 1, ct);
    }

    public async ValueTask AddInlineFromCatalog(CatalogItemEntity catalogItem, CancellationToken ct)
    {
        var quote = await SelectedQuote;
        if (quote is null) return;

        var profile = await _profileRepo.GetAsync();
        var markup = profile.DefaultMarkup;
        var effectivePrice = markup > 0
            ? catalogItem.UnitPrice * (1 + markup / 100m)
            : catalogItem.UnitPrice;

        var item = new LineItemEntity
        {
            QuoteId = quote.Id,
            Description = catalogItem.Description,
            UnitPrice = Math.Round(effectivePrice, 2),
            Quantity = 1,
            SortOrder = (await _quoteRepo.GetByIdAsync(quote.Id))?.LineItems.Count ?? 0,
        };
        await _quoteRepo.SaveLineItemAsync(item);
        await DetailVersion.UpdateAsync(v => v + 1, ct);
    }

    public Task<List<CatalogItemEntity>> GetCatalogItemsAsync() => _catalogRepo.GetAllAsync();

    // ── Inline Dialog Support ──────────────────────────────────────────────
    // Exposed so dialog-driven code-behind can persist without service-locator lookups.

    public Task<bool> CanCreateQuoteAsync() => _featureGate.CanCreateQuoteAsync();

    public string GetQuoteLimitMessage() => _featureGate.GetUpgradeMessage("quotes");

    public Task<List<ClientEntity>> GetAllClientsAsync() => _clientRepo.GetAllAsync();

    public Task<ClientEntity?> GetClientByIdAsync(string id) => _clientRepo.GetByIdAsync(id);

    public Task<BusinessProfileEntity> GetProfileAsync() => _profileRepo.GetAsync();

    public Task<QuoteEntity?> GetFreshQuoteAsync(string id) => _quoteRepo.GetByIdAsync(id);

    public async ValueTask<QuoteEntity> CreateQuoteAsync(string title, string clientName, CancellationToken ct)
    {
        var profile = await _profileRepo.GetAsync();
        var quoteNumber = await _quoteNumberGen.GenerateAsync();
        var quote = new QuoteEntity
        {
            Title = string.IsNullOrWhiteSpace(title) ? "New Quote" : title,
            QuoteNumber = quoteNumber,
            Status = QuoteStatus.Draft,
            TaxRate = profile.DefaultTaxRate,
            ValidUntil = DateTimeOffset.UtcNow.AddDays(profile.QuoteValidDays),
        };

        if (!string.IsNullOrWhiteSpace(clientName))
        {
            quote.ClientName = clientName;
            var all = await _clientRepo.GetAllAsync();
            var match = all.FirstOrDefault(c =>
                c.Name.Equals(clientName, StringComparison.OrdinalIgnoreCase));
            if (match is not null)
                quote.ClientId = match.Id;
        }

        await _quoteRepo.SaveAsync(quote);
        await _navigator.NavigateRouteAsync(this, "QuoteEditor", data: quote);
        return quote;
    }

    public Task GenerateAndDownloadPdfAsync(QuoteEntity quote) => _shareService.GenerateAndDownloadPdfAsync(quote);

    public Task MarkQuoteAsSentAsync(QuoteEntity quote) => _shareService.MarkAsSentAsync(quote);

    public Task ComposeEmailAsync(string recipient, string subject, string body) =>
        _emailService.ComposeEmailAsync(recipient, subject, body);

    public async Task<string> GetSelectedQuoteNotesAsync()
    {
        var quote = await SelectedQuote;
        if (quote is not null)
        {
            var fresh = await _quoteRepo.GetByIdAsync(quote.Id);
            return fresh?.Notes ?? string.Empty;
        }
        return string.Empty;
    }
}
