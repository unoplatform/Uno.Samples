using Meridian.Models;
using Meridian.Services;

namespace Meridian.Presentation;

public partial record StockDetailModel(string Ticker, IMarketDataService MarketData)
{
	// The stock being viewed — loaded from Ticker (injected by Uno Navigation)
	public IFeed<Stock> Stock => Feed.Async(async ct => await MarketData.GetStockAsync(Ticker, ct));

	// ── Read-only feeds (all driven by Ticker) ──

	// MVUX's Feed.Async<T> has a `notnull` constraint, but a null Holding is the
	// intentional "no position held for this ticker" state surfaced to the detail page.
	// Suppress the unsound-nullability warnings on this single feed only.
#pragma warning disable CS8714 // notnull constraint
#pragma warning disable CS8621 // lambda return-type nullability
	public IFeed<Holding?> Position =>
		Feed.Async(async ct => await MarketData.GetHoldingAsync(Ticker, ct));
#pragma warning restore CS8621
#pragma warning restore CS8714

	public IFeed<StockProfile> Profile =>
		Feed.Async(async ct => await MarketData.GetStockProfileAsync(Ticker, ct));

	public IFeed<AnalystData> Analysts =>
		Feed.Async(async ct => await MarketData.GetAnalystDataAsync(Ticker, ct));

	public IFeed<IImmutableList<KeyStat>> KeyStats =>
		Feed.Async(async ct => await MarketData.GetKeyStatsAsync(Ticker, ct));

	public IFeed<IImmutableList<NewsItem>> RelatedNews =>
		Feed.Async(async ct => await MarketData.GetNewsForTickerAsync(Ticker, ct));

	public IFeed<IImmutableList<SimilarStock>> SimilarHoldings =>
		Feed.Async(async ct => await MarketData.GetSimilarHoldingsAsync(Ticker, ct));

	// ── Computed feeds (react to state changes) ──

	public IFeed<IImmutableList<ChartPoint>> ChartData =>
		SelectedTimeframe.SelectAsync(async (tf, ct) =>
			await MarketData.GetStockHistoryAsync(Ticker, tf, ct));

	public IFeed<IImmutableList<Financial>> Financials =>
		FinancialsPeriod.SelectAsync(async (period, ct) =>
			await MarketData.GetFinancialsAsync(Ticker, period, ct));

	// ── Editable states ──

	public IState<string> SelectedTimeframe => State.Value(this, () => "3M");
	public IState<string> ChartType => State.Value(this, () => "Area");
	public IState<string> FinancialsPeriod => State.Value(this, () => "Annual");
	public IState<bool> IsInWatchlist => State.Value(this, () => false);
	public IState<bool> AboutExpanded => State.Value(this, () => false);
	public IState<string> TradeStockTicker => State.Value(this, () => "");

	// ── Commands ──

	public async ValueTask ToggleWatchlist()
	{
		var current = await IsInWatchlist;
		await IsInWatchlist.Set(!current, CancellationToken.None);
	}

	public async ValueTask OpenTrade() =>
		await TradeStockTicker.Set(Ticker, CancellationToken.None);

	public async ValueTask CloseTrade() =>
		await TradeStockTicker.Set("", CancellationToken.None);

	public async ValueTask ToggleAbout()
	{
		var current = await AboutExpanded;
		await AboutExpanded.Set(!current, CancellationToken.None);
	}
}
