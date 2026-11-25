namespace QuoteCraft.Presentation;

/// <summary>A quote in the list, and whether it is the one open in the detail.</summary>
public partial record QuoteListItem(QuoteEntity Quote, bool IsSelected)
{
    public string Key => Quote.Id;
}

/// <summary>
/// The Quotes page: the list with its search, status filter, counts and analytics, and New Quote. A tapped quote opens
/// in the page's detail region (<see cref="QuoteDetailModel"/>); the two share <see cref="QuoteBoard"/>.
/// </summary>
public partial record DashboardModel
{
    private readonly INavigator _navigator;
    private readonly IQuoteRepository _quoteRepo;
    private readonly IFeatureGateService _featureGate;
    private readonly QuoteStatusService _status;
    private readonly QuoteBoard _board;

    public DashboardModel(
        INavigator navigator,
        IQuoteRepository quoteRepo,
        IFeatureGateService featureGate,
        QuoteStatusService status,
        QuoteBoard board)
    {
        _navigator = navigator;
        _quoteRepo = quoteRepo;
        _featureGate = featureGate;
        _status = status;
        _board = board;

        // A new page starts with no quote open: its detail region is empty.
        _board.Close();
    }

    // No Viewed: nothing sets it since share links went (D11). A Viewed quote from an older install counts, and
    // filters, as Sent (IsSent).
    public IListFeed<string> FilterNames => ListFeed.Async(async ct => (IImmutableList<string>)ImmutableList.Create(
        "All", "Draft", "Sent", "Accepted", "Declined", "Expired"));

    private static bool IsSent(QuoteEntity quote) => quote.Status is QuoteStatus.Sent or QuoteStatus.Viewed;

    public IState<string> SelectedFilter => State<string>.Value(this, () => "All");
    public IState<string> SearchText => State<string>.Value(this, () => string.Empty);

    /// <summary>What New Quote could not do; shown in a banner.</summary>
    public IState<string> ActionError => State<string>.Value(this, () => string.Empty);

    public async ValueTask DismissError(CancellationToken ct) => await ActionError.UpdateAsync(_ => string.Empty, ct);

    private IFeed<QuoteBoardState>? _boardFeed;

    private IFeed<QuoteBoardState> Board => _boardFeed ??= Feed.AsyncEnumerable(_board.WatchAsync);

    /// <summary>True while a quote is open: its detail covers the list below 1080 px and fills the detail column above.</summary>
    public IFeed<bool> IsQuoteOpen => Board.Select(b => b.OpenQuoteId is not null);

    // The quotes, read once per change and shared by the list, the counts and the analytics. The four feeds ask at
    // once on the page's first load, so one reads (and expires overdue quotes) while the others wait for its list.
    private readonly SemaphoreSlim _cacheGate = new(1, 1);
    private List<QuoteEntity>? _cachedQuotes;
    private int _cachedVersion = -1;

    private async Task<List<QuoteEntity>> GetCachedQuotesAsync(int version)
    {
        await _cacheGate.WaitAsync();
        try
        {
            if (_cachedQuotes is not null && _cachedVersion == version)
            {
                return _cachedQuotes;
            }
            if (_cachedQuotes is null)
            {
                // Overdue quotes expire as the list first opens (the expiry service also checks every few minutes).
                await ErrorReporter.RunQuietlyAsync("Expiring overdue quotes", () => _status.ExpireOverdueAsync(DateTimeOffset.UtcNow));
            }
            _cachedQuotes = await _quoteRepo.GetAllAsync();
            _cachedVersion = version;
            return _cachedQuotes;
        }
        finally
        {
            _cacheGate.Release();
        }
    }

    private IListFeed<QuoteListItem>? _quotesFeed;

    public IListFeed<QuoteListItem> Quotes => _quotesFeed ??=
        Feed.Combine(SelectedFilter, SearchText, Board)
            .SelectAsync(async (inputs, ct) =>
            {
                var (filter, search, board) = inputs;
                var all = await GetCachedQuotesAsync(board.Version);

                IEnumerable<QuoteEntity> filtered = all;

                if (!string.IsNullOrEmpty(filter) && filter != "All")
                {
                    filtered = filter == nameof(QuoteStatus.Sent)
                        ? filtered.Where(IsSent)
                        : filtered.Where(q => Enum.TryParse<QuoteStatus>(filter, out var s) && q.Status == s);
                }

                if (!string.IsNullOrWhiteSpace(search))
                {
                    filtered = filtered.Where(q =>
                        q.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        (q.ClientName ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        q.QuoteNumber.Contains(search, StringComparison.OrdinalIgnoreCase));
                }

                return (IImmutableList<QuoteListItem>)filtered
                    .Select(q => new QuoteListItem(q, q.Id == board.OpenQuoteId))
                    .ToImmutableList();
            })
            .AsListFeed();

    private IFeed<int> Version => Board.Select(b => b.Version);

    public IFeed<int> TotalCount => Version.SelectAsync(async (version, ct) => (await GetCachedQuotesAsync(version)).Count);

    public IFeed<QuoteStatusCounts> StatusCounts => Version
        .SelectAsync(async (version, ct) =>
        {
            var all = await GetCachedQuotesAsync(version);
            return new QuoteStatusCounts(
                all.Count,
                all.Count(q => q.Status == QuoteStatus.Draft),
                all.Count(IsSent),
                all.Count(q => q.Status == QuoteStatus.Accepted),
                all.Count(q => q.Status == QuoteStatus.Declined),
                all.Count(q => q.Status == QuoteStatus.Expired));
        });

    /// <summary>This month's totals and the acceptance rate, with 7-day sparklines.</summary>
    public IFeed<DashboardAnalytics> Analytics => Version
        .SelectAsync(async (version, ct) => DashboardAnalytics.From(await GetCachedQuotesAsync(version), DateTimeOffset.UtcNow));

    /// <summary>Retry after a failed read: every view of the quotes reads them again.</summary>
    public ValueTask Refresh()
    {
        _board.QuotesChanged();
        return ValueTask.CompletedTask;
    }

    /// <summary>A tap (or Enter) on a list card: the quote opens in the detail region.</summary>
    public async ValueTask OpenQuote(QuoteListItem item, CancellationToken ct)
    {
        await ActionError.ReportFailureAsync("The quote could not be opened.", async () =>
        {
            _board.Open(item.Quote.Id);
            await _navigator.NavigateRouteAsync(this, "./QuoteDetail", data: item.Quote, cancellation: ct);
        }, ct);
    }

    /// <summary>Plans and the upgrade live in Settings.</summary>
    public async ValueTask OpenSettings(CancellationToken ct) =>
        await _navigator.NavigateRouteAsync(this, "Settings", cancellation: ct);

    /// <summary>New Quote: the plan limit first (only where plans exist), then the dialog, then the editor.</summary>
    public ValueTask CreateQuote(CancellationToken ct) => ActionError.ReportFailureAsync("The quote could not be created.", async () =>
    {
        if (!await _featureGate.CanCreateQuoteAsync())
        {
            if (await _navigator.ConfirmAsync(this, new ConfirmRequest("", "Quote Limit Reached",
                    _featureGate.GetUpgradeMessage("quotes"), "Upgrade", "OK"), ct))
            {
                await OpenSettings(ct);
            }
            return;
        }

        if (await _navigator.ShowDialogAsync<QuoteEntity>(this, "NewQuote", null, ct) is { } quote)
        {
            _board.QuotesChanged();
            await _navigator.NavigateRouteAsync(this, "QuoteEditor", data: quote, cancellation: ct);
        }
    }, ct);
}
