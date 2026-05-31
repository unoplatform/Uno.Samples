using Meridian.Models;

namespace Meridian.Services;

public interface IMarketDataService
{
	ValueTask<IImmutableList<Stock>> GetWatchlistAsync(CancellationToken ct);
	ValueTask<IImmutableList<Holding>> GetHoldingsAsync(CancellationToken ct);
	ValueTask<IImmutableList<Sector>> GetSectorsAsync(CancellationToken ct);
	ValueTask<IImmutableList<VolumeBar>> GetVolumeAsync(CancellationToken ct);
	ValueTask<IImmutableList<NewsItem>> GetNewsAsync(CancellationToken ct);
	ValueTask<IImmutableList<ChartPoint>> GetPortfolioHistoryAsync(CancellationToken ct);
	ValueTask<IImmutableList<ChartPoint>> GetStockHistoryAsync(string ticker, CancellationToken ct);
	IImmutableList<IndexTicker> GetIndexTickers();
	IImmutableList<StreamTicker> GetStreamTickers();

	// Stock Detail page methods
	ValueTask<Stock> GetStockAsync(string ticker, CancellationToken ct);
	ValueTask<Holding?> GetHoldingAsync(string ticker, CancellationToken ct);
	ValueTask<StockProfile> GetStockProfileAsync(string ticker, CancellationToken ct);
	ValueTask<AnalystData> GetAnalystDataAsync(string ticker, CancellationToken ct);
	ValueTask<IImmutableList<Financial>> GetFinancialsAsync(string ticker, string period, CancellationToken ct);
	ValueTask<IImmutableList<KeyStat>> GetKeyStatsAsync(string ticker, CancellationToken ct);
	ValueTask<IImmutableList<ChartPoint>> GetStockHistoryAsync(string ticker, string timeframe, CancellationToken ct);
	ValueTask<IImmutableList<NewsItem>> GetNewsForTickerAsync(string ticker, CancellationToken ct);
	ValueTask<IImmutableList<SimilarStock>> GetSimilarHoldingsAsync(string ticker, CancellationToken ct);
}
