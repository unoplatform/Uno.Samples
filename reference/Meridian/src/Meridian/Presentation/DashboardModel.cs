using Meridian.Models;
using Meridian.Services;

namespace Meridian.Presentation;

public partial record DashboardModel(IMarketDataService MarketData)
{
    // Watchlist filtered by SearchQuery
    public IFeed<IImmutableList<Stock>> Watchlist =>
        SearchQuery.SelectAsync(async (query, ct) =>
        {
            var all = await MarketData.GetWatchlistAsync(ct);
            if (string.IsNullOrWhiteSpace(query))
                return all;
            return (IImmutableList<Stock>)all
                .Where(s => s.Ticker.Contains(query, StringComparison.OrdinalIgnoreCase)
                          || s.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToImmutableList();
        });
    public IListFeed<Holding> Holdings => ListFeed.Async(MarketData.GetHoldingsAsync);
    public IListFeed<Sector> Sectors => ListFeed.Async(MarketData.GetSectorsAsync);
    public IListFeed<VolumeBar> Volume => ListFeed.Async(MarketData.GetVolumeAsync);
    public IListFeed<NewsItem> News => ListFeed.Async(MarketData.GetNewsAsync);
    public IFeed<IImmutableList<ChartPoint>> PortfolioHistory => Feed.Async(MarketData.GetPortfolioHistoryAsync);

    // Portfolio summary computed from holdings
    public IFeed<PortfolioSummary> Summary => Feed.Async(async ct =>
    {
        var holdings = await MarketData.GetHoldingsAsync(ct);
        var totalValue = holdings.Sum(h => h.MarketValue);
        var totalCost = holdings.Sum(h => h.Shares * h.AvgCost);
        var totalGain = totalValue - totalCost;
        var totalPct = totalCost != 0 ? totalGain / totalCost * 100 : 0;
        return new PortfolioSummary(totalValue, totalGain, totalPct, totalGain >= 0);
    });

    // Index tickers (sync → async wrapper)
    public IListFeed<IndexTicker> IndexTickers =>
        ListFeed.Async(async ct => MarketData.GetIndexTickers());

    // Editable states (empty string = "none" sentinel)
    public IState<string> SelectedTimeframe => State.Value(this, () => "3M");
    public IState<string> ChartTicker => State.Value(this, () => "");
    public IState<string> ExpandedTicker => State.Value(this, () => "");
    public IState<string> SearchQuery => State.Value(this, () => "");
    public IState<string> HoveredHolding => State.Value(this, () => "");
    public IState<string> TradeStockTicker => State.Value(this, () => "");

    // Computed feed: chart data swaps based on ChartTicker
    public IFeed<IImmutableList<ChartPoint>> ChartData =>
        ChartTicker.SelectAsync(async (ticker, ct) =>
            string.IsNullOrEmpty(ticker)
                ? await MarketData.GetPortfolioHistoryAsync(ct)
                : await MarketData.GetStockHistoryAsync(ticker, ct));

    // Commands
    public async ValueTask SelectHolding(string ticker)
    {
        var current = await ChartTicker;
        await ChartTicker.Set(
            current == ticker ? "" : ticker,
            CancellationToken.None);
    }

    public async ValueTask ToggleExpanded(string ticker)
    {
        var current = await ExpandedTicker;
        await ExpandedTicker.Set(
            current == ticker ? "" : ticker,
            CancellationToken.None);
    }

    public async ValueTask OpenTrade(string ticker) =>
        await TradeStockTicker.Set(ticker, CancellationToken.None);

    public async ValueTask CloseTrade() =>
        await TradeStockTicker.Set("", CancellationToken.None);

    public async ValueTask BackToPortfolio() =>
        await ChartTicker.Set("", CancellationToken.None);
}
