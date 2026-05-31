using Meridian.Helpers;
using Meridian.Models;

namespace Meridian.Services;

public class MockMarketDataService : IMarketDataService
{
	private readonly IImmutableList<Stock> _stocks;
	private readonly IImmutableList<Holding> _holdings;
	private readonly IImmutableList<Sector> _sectors;
	private readonly IImmutableList<VolumeBar> _volume;
	private readonly IImmutableList<NewsItem> _news;
	private readonly IImmutableList<ChartPoint> _portfolioHistory;
	private readonly Dictionary<string, IImmutableList<ChartPoint>> _stockHistories;
	private readonly IImmutableList<IndexTicker> _indexTickers;
	private readonly IImmutableList<StreamTicker> _streamTickers;
	private readonly Dictionary<string, StockProfile> _profiles;
	private readonly Dictionary<string, AnalystData> _analysts;
	private readonly Dictionary<string, IImmutableList<Financial>> _annualFinancials;
	private readonly Dictionary<string, IImmutableList<Financial>> _quarterlyFinancials;
	private readonly Dictionary<string, IImmutableList<NewsItem>> _tickerNews;
	private readonly Dictionary<string, string> _tickerSectors;

	public MockMarketDataService()
	{
		_stocks = BuildStocks();
		_holdings = BuildHoldings();
		_sectors = BuildSectors();
		_volume = BuildVolume();
		_news = BuildNews();
		_portfolioHistory = BuildPortfolioHistory();
		_stockHistories = BuildStockHistories();
		_indexTickers = BuildIndexTickers();
		_streamTickers = BuildStreamTickers();
		_profiles = BuildProfiles();
		_analysts = BuildAnalysts();
		_annualFinancials = BuildAnnualFinancials();
		_quarterlyFinancials = BuildQuarterlyFinancials();
		_tickerNews = BuildTickerNews();
		_tickerSectors = BuildTickerSectors();
	}

	public ValueTask<IImmutableList<Stock>> GetWatchlistAsync(CancellationToken ct) =>
		ValueTask.FromResult(_stocks);

	public ValueTask<IImmutableList<Holding>> GetHoldingsAsync(CancellationToken ct) =>
		ValueTask.FromResult(_holdings);

	public ValueTask<IImmutableList<Sector>> GetSectorsAsync(CancellationToken ct) =>
		ValueTask.FromResult(_sectors);

	public ValueTask<IImmutableList<VolumeBar>> GetVolumeAsync(CancellationToken ct) =>
		ValueTask.FromResult(_volume);

	public ValueTask<IImmutableList<NewsItem>> GetNewsAsync(CancellationToken ct) =>
		ValueTask.FromResult(_news);

	public ValueTask<IImmutableList<ChartPoint>> GetPortfolioHistoryAsync(CancellationToken ct) =>
		ValueTask.FromResult(_portfolioHistory);

	public ValueTask<IImmutableList<ChartPoint>> GetStockHistoryAsync(string ticker, CancellationToken ct) =>
		ValueTask.FromResult(
			_stockHistories.TryGetValue(ticker, out var history)
				? history
				: ImmutableList<ChartPoint>.Empty as IImmutableList<ChartPoint>);

	public IImmutableList<IndexTicker> GetIndexTickers() => _indexTickers;

	public IImmutableList<StreamTicker> GetStreamTickers() => _streamTickers;

	// ── Stock Detail methods ────────────────────────────────────────────

	public ValueTask<Stock> GetStockAsync(string ticker, CancellationToken ct) =>
		ValueTask.FromResult(
			_stocks.FirstOrDefault(s => s.Ticker == ticker)
			?? throw new KeyNotFoundException($"Stock '{ticker}' not found in mock data."));

	public ValueTask<Holding?> GetHoldingAsync(string ticker, CancellationToken ct) =>
		ValueTask.FromResult(_holdings.FirstOrDefault(h => h.Ticker == ticker));

	public ValueTask<StockProfile> GetStockProfileAsync(string ticker, CancellationToken ct) =>
		ValueTask.FromResult(_profiles.TryGetValue(ticker, out var p) ? p : _profiles["AAPL"]);

	public ValueTask<AnalystData> GetAnalystDataAsync(string ticker, CancellationToken ct) =>
		ValueTask.FromResult(_analysts.TryGetValue(ticker, out var a) ? a : _analysts["AAPL"]);

	public ValueTask<IImmutableList<Financial>> GetFinancialsAsync(string ticker, string period, CancellationToken ct)
	{
		var dict = period == "Quarterly" ? _quarterlyFinancials : _annualFinancials;
		return ValueTask.FromResult(
			dict.TryGetValue(ticker, out var f) ? f : dict["AAPL"]);
	}

	public ValueTask<IImmutableList<KeyStat>> GetKeyStatsAsync(string ticker, CancellationToken ct)
	{
		var stock = _stocks.FirstOrDefault(s => s.Ticker == ticker) ?? _stocks[0];
		var stats = ImmutableList.Create(
			new KeyStat("Prev Close", $"${stock.Price - stock.Change:F2}"),
			new KeyStat("Open", $"${stock.Open:F2}"),
			new KeyStat("Day Range", $"${stock.Low:F2} – ${stock.High:F2}"),
			new KeyStat("52-Week Range", ticker switch
			{
				"AAPL" => "$189.50 – $258.30",
				"NVDA" => "$620.00 – $974.00",
				"MSFT" => "$388.00 – $480.50",
				_ => $"${stock.Price * 0.75m:F2} – ${stock.Price * 1.15m:F2}"
			}),
			new KeyStat("Volume", stock.Volume),
			new KeyStat("Avg Volume", ticker switch
			{
				"AAPL" => "58.4M", "NVDA" => "38.2M", "MSFT" => "24.1M",
				_ => $"{decimal.Parse(stock.Volume.Replace("M", "")) * 0.92m:F1}M"
			}),
			new KeyStat("Market Cap", ticker switch
			{
				"AAPL" => "3.82T", "NVDA" => "2.20T", "MSFT" => "3.48T",
				"GOOGL" => "1.76T", "META" => "0.98T", "TSLA" => "0.79T",
				_ => $"{stock.Price * 0.01m:F2}T"
			}),
			new KeyStat("P/E (TTM)", ticker switch
			{
				"AAPL" => "31.2", "NVDA" => "64.8", "MSFT" => "35.4",
				"GOOGL" => "23.1", "META" => "26.8", "TSLA" => "58.2",
				_ => "28.5"
			}),
			new KeyStat("EPS (TTM)", ticker switch
			{
				"AAPL" => "$7.94", "NVDA" => "$13.77", "MSFT" => "$13.22",
				"GOOGL" => "$6.17", "META" => "$13.07", "TSLA" => "$4.28",
				_ => "$8.50"
			}),
			new KeyStat("Dividend Yield", ticker switch
			{
				"AAPL" => "0.44%", "MSFT" => "0.71%", "JPM" => "2.12%",
				_ => "—"
			}),
			new KeyStat("Beta", ticker switch
			{
				"AAPL" => "1.24", "NVDA" => "1.68", "MSFT" => "0.89",
				"GOOGL" => "1.06", "META" => "1.22", "TSLA" => "2.05",
				_ => "1.15"
			}),
			new KeyStat("52-Week Change",
				$"{(stock.Pct >= 0 ? "+" : "")}{stock.Pct * 8:F1}%",
				IsColored: true, IsPositive: stock.Pct >= 0)
		);
		return ValueTask.FromResult(stats as IImmutableList<KeyStat>);
	}

	public ValueTask<IImmutableList<ChartPoint>> GetStockHistoryAsync(
		string ticker, string timeframe, CancellationToken ct)
	{
		var all = _stockHistories.TryGetValue(ticker, out var history)
			? history
			: ImmutableList<ChartPoint>.Empty as IImmutableList<ChartPoint>;
		return ValueTask.FromResult(ChartHelper.FilterByTimeframe(all, timeframe));
	}

	public ValueTask<IImmutableList<NewsItem>> GetNewsForTickerAsync(string ticker, CancellationToken ct) =>
		ValueTask.FromResult(
			_tickerNews.TryGetValue(ticker, out var n)
				? n
				: ImmutableList<NewsItem>.Empty as IImmutableList<NewsItem>);

	public ValueTask<IImmutableList<SimilarStock>> GetSimilarHoldingsAsync(string ticker, CancellationToken ct)
	{
		if (!_tickerSectors.TryGetValue(ticker, out var sector))
			return ValueTask.FromResult(ImmutableList<SimilarStock>.Empty as IImmutableList<SimilarStock>);

		var similar = _stocks
			.Where(s => s.Ticker != ticker && _tickerSectors.TryGetValue(s.Ticker, out var ss) && ss == sector)
			.Select(s => new SimilarStock(s.Ticker, s.Name, s.Price, s.Pct))
			.ToImmutableList();
		return ValueTask.FromResult(similar as IImmutableList<SimilarStock>);
	}

	// ── Stock data ──────────────────────────────────────────────────────

	private static IImmutableList<Stock> BuildStocks() =>
		ImmutableList.Create(
			new Stock("AAPL", "Apple Inc.", 247.63m, 3.42m, 1.40m, 251.20m, 244.18m, 244.88m, "62.1M"),
			new Stock("NVDA", "NVIDIA Corp.", 892.14m, -12.82m, -1.42m, 901.44m, 886.22m, 898.10m, "41.3M"),
			new Stock("MSFT", "Microsoft Corp.", 468.21m, 5.18m, 1.12m, 472.80m, 464.15m, 465.00m, "28.7M"),
			new Stock("GOOGL", "Alphabet Inc.", 142.58m, -1.24m, -0.86m, 145.20m, 141.80m, 143.50m, "22.4M"),
			new Stock("META", "Meta Platforms", 350.42m, 8.56m, 2.50m, 355.00m, 345.20m, 346.00m, "35.8M"),
			new Stock("JPM", "JPMorgan Chase", 195.84m, 2.14m, 1.10m, 198.40m, 194.20m, 194.50m, "14.2M"),
			new Stock("AMZN", "Amazon.com", 178.32m, -2.44m, -1.35m, 182.10m, 177.50m, 180.00m, "52.6M"),
			new Stock("TSLA", "Tesla Inc.", 248.92m, 12.48m, 5.28m, 252.80m, 238.40m, 240.00m, "89.4M"));

	// ── Holdings data ───────────────────────────────────────────────────

	private static IImmutableList<Holding> BuildHoldings() =>
		ImmutableList.Create(
			new Holding("AAPL", 85, 178.40m, 247.63m),
			new Holding("NVDA", 22, 480.00m, 892.14m),
			new Holding("MSFT", 40, 380.00m, 468.21m),
			new Holding("GOOGL", 60, 142.58m, 142.58m),
			new Holding("META", 18, 350.42m, 350.42m),
			new Holding("JPM", 30, 195.84m, 195.84m),
			new Holding("TSLA", 15, 210.00m, 248.92m));

	// ── Sectors data ────────────────────────────────────────────────────

	private static IImmutableList<Sector> BuildSectors() =>
		ImmutableList.Create(
			new Sector("Technology", 68.2, "#2D6A4F"),
			new Sector("Consumer Disc.", 14.5, "#C9A96E"),
			new Sector("Financials", 9.8, "#8A8A8A"),
			new Sector("Healthcare", 4.8, "#B5342B"),
			new Sector("Energy", 2.7, "#C4C0B8"));

	// ── Volume data (24 hourly bars) ────────────────────────────────────

	private static IImmutableList<VolumeBar> BuildVolume()
	{
		// Realistic intraday volume (millions): low overnight, spike at open, taper off
		int[] volumes =
		[
			5, 5, 5, 4, 6, 7, 9, 12, 16,
			45, 82, 68, 55, 62, 58, 72, 65,
			48, 32, 18, 13, 8, 6, 5
		];

		var builder = ImmutableList.CreateBuilder<VolumeBar>();
		for (var h = 0; h < 24; h++)
		{
			builder.Add(new VolumeBar($"{h}:00", volumes[h]));
		}
		return builder.ToImmutable();
	}

	// ── News data ───────────────────────────────────────────────────────

	private static IImmutableList<NewsItem> BuildNews() =>
		ImmutableList.Create(
			new NewsItem(
				"2m",
				"Fed signals potential rate adjustment in Q2 as inflation data shows mixed signals across key economic indicators",
				"Macro"),
			new NewsItem(
				"18m",
				"NVIDIA beats earnings expectations, raises guidance on strong data center demand and AI infrastructure growth",
				"Earnings"),
			new NewsItem(
				"34m",
				"Treasury yields climb as inflation data exceeds forecasts, pushing 10-year note to highest level since November",
				"Bonds"),
			new NewsItem(
				"1h",
				"Apple announces expanded AI features across product lineup, including enhanced Siri capabilities and on-device processing",
				"Tech"));

	// ── Portfolio history (90 days) ─────────────────────────────────────

	private static IImmutableList<ChartPoint> BuildPortfolioHistory()
	{
		const int days = 90;
		const double startValue = 123_500.0;
		const double endValue = 163_842.0;
		var startDate = new DateTimeOffset(2025, 12, 18, 0, 0, 0, TimeSpan.Zero);

		return GenerateRandomWalk(startDate, days, startValue, endValue, seed: 42);
	}

	// ── Per-stock histories (90 days each) ──────────────────────────────

	private static Dictionary<string, IImmutableList<ChartPoint>> BuildStockHistories()
	{
		const int days = 90;
		var startDate = new DateTimeOffset(2025, 12, 18, 0, 0, 0, TimeSpan.Zero);

		// (ticker, endPrice, approximateStartPrice, seed)
		(string Ticker, double End, double Start, int Seed)[] specs =
		[
			("AAPL", 247.63, 218.00, 100),
			("NVDA", 892.14, 820.00, 200),
			("MSFT", 468.21, 430.00, 300),
			("GOOGL", 142.58, 138.00, 400),
			("META", 350.42, 310.00, 500),
			("JPM", 195.84, 182.00, 600),
			("AMZN", 178.32, 170.00, 700),
			("TSLA", 248.92, 210.00, 800),
		];

		var dict = new Dictionary<string, IImmutableList<ChartPoint>>(specs.Length);
		foreach (var (ticker, end, start, seed) in specs)
		{
			dict[ticker] = GenerateRandomWalk(startDate, days, start, end, seed);
		}
		return dict;
	}

	// ── Index tickers ───────────────────────────────────────────────────

	private static IImmutableList<IndexTicker> BuildIndexTickers() =>
		ImmutableList.Create(
			new IndexTicker("S&P 500", "5,892", "+0.87%", true),
			new IndexTicker("NASDAQ", "18,742", "+1.12%", true),
			new IndexTicker("DOW 30", "43,218", "+0.34%", true));

	// ── Stream tickers ──────────────────────────────────────────────────

	private static IImmutableList<StreamTicker> BuildStreamTickers() =>
		ImmutableList.Create(
			new StreamTicker("AAPL", "$247.63", "+1.40%", true),
			new StreamTicker("NVDA", "$892.14", "-1.42%", false),
			new StreamTicker("MSFT", "$468.21", "+1.12%", true),
			new StreamTicker("GOOGL", "$142.58", "-0.86%", false),
			new StreamTicker("META", "$350.42", "+2.50%", true),
			new StreamTicker("TSLA", "$248.92", "+5.28%", true),
			new StreamTicker("AMZN", "$178.32", "-1.35%", false),
			new StreamTicker("JPM", "$195.84", "+1.10%", true),
			new StreamTicker("V", "$281.45", "+0.78%", true),
			new StreamTicker("UNH", "$524.60", "-0.42%", false),
			new StreamTicker("HD", "$362.18", "+0.95%", true),
			new StreamTicker("DIS", "$112.34", "+1.82%", true));

	// ── Stock profiles ──────────────────────────────────────────────────

	private static Dictionary<string, StockProfile> BuildProfiles() => new()
	{
		["AAPL"] = new StockProfile(
			"Apple Inc. designs, manufactures, and markets smartphones, personal computers, tablets, wearables, and accessories worldwide. The company offers iPhone, Mac, iPad, Apple Watch, AirPods, Apple TV, and HomePod. It also provides AppleCare support and cloud services, and operates platforms including the App Store, Apple Music, Apple Pay, and Apple TV+.",
			"Technology", "Consumer Electronics", 1976, "Cupertino, CA", "Tim Cook", "164,000", "https://apple.com"),
		["NVDA"] = new StockProfile(
			"NVIDIA Corporation provides graphics, computing, and networking solutions. The company's products are used in gaming, professional visualization, data center, and automotive markets. NVIDIA is a pioneer in accelerated computing and a leader in AI hardware and software platforms.",
			"Technology", "Semiconductors", 1993, "Santa Clara, CA", "Jensen Huang", "29,600", "https://nvidia.com"),
		["MSFT"] = new StockProfile(
			"Microsoft Corporation develops, licenses, and supports software, services, devices, and solutions worldwide. The company operates in Productivity and Business Processes, Intelligent Cloud, and More Personal Computing segments, offering Office 365, Azure, Windows, LinkedIn, and Xbox products.",
			"Technology", "Software—Infrastructure", 1975, "Redmond, WA", "Satya Nadella", "228,000", "https://microsoft.com"),
		["GOOGL"] = new StockProfile(
			"Alphabet Inc. offers various products and platforms including Google Search, YouTube, Android, Chrome, Google Cloud, and Waymo. The company generates most of its revenue from advertising services through Google properties and network partners.",
			"Technology", "Internet Content & Information", 1998, "Mountain View, CA", "Sundar Pichai", "182,500", "https://abc.xyz"),
		["META"] = new StockProfile(
			"Meta Platforms, Inc. operates social technology products including Facebook, Instagram, Messenger, WhatsApp, and the Meta Quest virtual reality platform. The company is building the metaverse and investing heavily in AI and augmented reality technologies.",
			"Technology", "Internet Content & Information", 2004, "Menlo Park, CA", "Mark Zuckerberg", "86,482", "https://meta.com"),
		["JPM"] = new StockProfile(
			"JPMorgan Chase & Co. is a global financial services firm offering investment banking, financial services for consumers and small businesses, commercial banking, financial transaction processing, and asset management across worldwide markets.",
			"Financials", "Banks—Diversified", 1799, "New York, NY", "Jamie Dimon", "309,926", "https://jpmorganchase.com"),
		["AMZN"] = new StockProfile(
			"Amazon.com, Inc. engages in retail, cloud computing (AWS), digital streaming, and artificial intelligence. The company operates through North America, International, and Amazon Web Services segments, serving consumers, sellers, developers, and enterprises worldwide.",
			"Consumer Discretionary", "Internet Retail", 1994, "Seattle, WA", "Andy Jassy", "1,525,000", "https://amazon.com"),
		["TSLA"] = new StockProfile(
			"Tesla, Inc. designs, develops, manufactures, and sells electric vehicles, energy generation and storage systems. The company produces the Model S, 3, X, Y, Cybertruck, and Semi, and is developing autonomous driving technology and humanoid robotics.",
			"Consumer Discretionary", "Auto Manufacturers", 2003, "Austin, TX", "Elon Musk", "140,473", "https://tesla.com"),
	};

	// ── Analyst data ────────────────────────────────────────────────────

	private static Dictionary<string, AnalystData> BuildAnalysts() => new()
	{
		["AAPL"] = new AnalystData(28, 8, 2, 210m, 265m, 310m),
		["NVDA"] = new AnalystData(35, 4, 1, 750m, 950m, 1200m),
		["MSFT"] = new AnalystData(32, 6, 1, 400m, 500m, 560m),
		["GOOGL"] = new AnalystData(26, 10, 2, 120m, 165m, 200m),
		["META"] = new AnalystData(30, 5, 3, 280m, 400m, 480m),
		["JPM"] = new AnalystData(18, 10, 4, 170m, 210m, 240m),
		["AMZN"] = new AnalystData(34, 3, 1, 150m, 205m, 250m),
		["TSLA"] = new AnalystData(14, 12, 10, 150m, 280m, 400m),
	};

	// ── Financial data ──────────────────────────────────────────────────

	private static Dictionary<string, IImmutableList<Financial>> BuildAnnualFinancials() => new()
	{
		["AAPL"] = ImmutableList.Create(
			new Financial("2024", "$394.3B", "$101.2B", "$6.42", 394.3m),
			new Financial("2023", "$383.3B", "$97.0B", "$6.16", 383.3m),
			new Financial("2022", "$394.3B", "$99.8B", "$6.11", 394.3m),
			new Financial("2021", "$365.8B", "$94.7B", "$5.61", 365.8m)),
		["NVDA"] = ImmutableList.Create(
			new Financial("2024", "$130.5B", "$72.9B", "$2.94", 130.5m),
			new Financial("2023", "$60.9B", "$29.8B", "$1.19", 60.9m),
			new Financial("2022", "$27.0B", "$4.4B", "$0.17", 27.0m),
			new Financial("2021", "$26.9B", "$9.8B", "$0.39", 26.9m)),
		["MSFT"] = ImmutableList.Create(
			new Financial("2024", "$245.1B", "$88.1B", "$11.80", 245.1m),
			new Financial("2023", "$211.9B", "$72.4B", "$9.68", 211.9m),
			new Financial("2022", "$198.3B", "$72.7B", "$9.65", 198.3m),
			new Financial("2021", "$168.1B", "$61.3B", "$8.05", 168.1m)),
	};

	private static Dictionary<string, IImmutableList<Financial>> BuildQuarterlyFinancials() => new()
	{
		["AAPL"] = ImmutableList.Create(
			new Financial("Q4 2024", "$124.3B", "$36.3B", "$2.40", 124.3m),
			new Financial("Q3 2024", "$94.9B", "$23.6B", "$1.53", 94.9m),
			new Financial("Q2 2024", "$85.8B", "$21.4B", "$1.40", 85.8m),
			new Financial("Q1 2024", "$89.3B", "$19.9B", "$1.09", 89.3m),
			new Financial("Q4 2023", "$119.6B", "$33.9B", "$2.18", 119.6m),
			new Financial("Q3 2023", "$89.5B", "$22.9B", "$1.46", 89.5m),
			new Financial("Q2 2023", "$81.8B", "$19.9B", "$1.26", 81.8m),
			new Financial("Q1 2023", "$92.4B", "$20.3B", "$1.26", 92.4m)),
		["NVDA"] = ImmutableList.Create(
			new Financial("Q4 2024", "$39.3B", "$22.1B", "$0.89", 39.3m),
			new Financial("Q3 2024", "$35.1B", "$19.3B", "$0.78", 35.1m),
			new Financial("Q2 2024", "$30.0B", "$16.6B", "$0.67", 30.0m),
			new Financial("Q1 2024", "$26.1B", "$14.9B", "$0.60", 26.1m),
			new Financial("Q4 2023", "$22.1B", "$12.8B", "$0.52", 22.1m),
			new Financial("Q3 2023", "$18.1B", "$9.2B", "$0.37", 18.1m),
			new Financial("Q2 2023", "$13.5B", "$6.2B", "$0.25", 13.5m),
			new Financial("Q1 2023", "$7.2B", "$1.6B", "$0.06", 7.2m)),
		["MSFT"] = ImmutableList.Create(
			new Financial("Q4 2024", "$65.6B", "$24.1B", "$3.23", 65.6m),
			new Financial("Q3 2024", "$61.9B", "$21.9B", "$2.94", 61.9m),
			new Financial("Q2 2024", "$59.8B", "$21.5B", "$2.93", 59.8m),
			new Financial("Q1 2024", "$57.8B", "$20.6B", "$2.70", 57.8m),
			new Financial("Q4 2023", "$56.5B", "$20.1B", "$2.69", 56.5m),
			new Financial("Q3 2023", "$52.9B", "$18.3B", "$2.45", 52.9m),
			new Financial("Q2 2023", "$52.7B", "$18.1B", "$2.32", 52.7m),
			new Financial("Q1 2023", "$49.8B", "$15.9B", "$2.22", 49.8m)),
	};

	// ── Ticker-specific news ────────────────────────────────────────────

	private static Dictionary<string, IImmutableList<NewsItem>> BuildTickerNews() => new()
	{
		["AAPL"] = ImmutableList.Create(
			new NewsItem("1h", "Apple announces expanded AI features across product lineup for 2026", "Tech"),
			new NewsItem("3h", "Apple Q1 earnings beat expectations, Services revenue reaches new high", "Earnings"),
			new NewsItem("5h", "Fed signals potential rate adjustment in Q2 affecting consumer spending outlook", "Macro"),
			new NewsItem("Yesterday", "iPhone 17 production ramping up ahead of September launch, supply chain sources confirm", "Tech")),
		["NVDA"] = ImmutableList.Create(
			new NewsItem("18m", "NVIDIA beats earnings expectations, raises guidance on strong data center demand", "Earnings"),
			new NewsItem("2h", "NVIDIA unveils next-gen Blackwell Ultra AI chips at GTC conference", "Tech"),
			new NewsItem("6h", "Data center capex spending surges as hyperscalers race to build AI infrastructure", "Tech"),
			new NewsItem("1d", "NVIDIA partners with leading automakers for autonomous driving platform", "Tech")),
		["MSFT"] = ImmutableList.Create(
			new NewsItem("45m", "Microsoft Azure revenue grows 31% as enterprise AI adoption accelerates", "Earnings"),
			new NewsItem("4h", "Microsoft expands Copilot AI integration across Office 365 suite", "Tech"),
			new NewsItem("8h", "Cloud infrastructure spending expected to reach $350B by 2027", "Tech"),
			new NewsItem("1d", "Microsoft announces $10B investment in Southeast Asia data centers", "Tech")),
		["GOOGL"] = ImmutableList.Create(
			new NewsItem("30m", "Google launches Gemini 2.0 with improved reasoning capabilities", "Tech"),
			new NewsItem("3h", "Alphabet ad revenue beats estimates despite regulatory headwinds", "Earnings"),
			new NewsItem("7h", "YouTube surpasses Netflix in US streaming watch time", "Tech"),
			new NewsItem("1d", "Google Cloud secures major government contracts for AI services", "Tech")),
		["META"] = ImmutableList.Create(
			new NewsItem("1h", "Meta reports strong ad revenue growth driven by Reels and AI targeting", "Earnings"),
			new NewsItem("4h", "Meta launches next-gen Quest headset with mixed reality features", "Tech"),
			new NewsItem("12h", "Instagram crosses 3 billion monthly active users milestone", "Tech"),
			new NewsItem("1d", "Meta's AI assistant reaches 500M monthly users across platforms", "Tech")),
		["TSLA"] = ImmutableList.Create(
			new NewsItem("20m", "Tesla Cybertruck deliveries surpass 200,000 units globally", "Tech"),
			new NewsItem("2h", "Tesla FSD v13 achieves record safety metrics in latest NHTSA report", "Tech"),
			new NewsItem("5h", "EV market share continues to grow, Tesla maintains US leadership", "Tech"),
			new NewsItem("1d", "Tesla Energy division revenue doubles year-over-year", "Earnings")),
	};

	// ── Sector mapping ──────────────────────────────────────────────────

	private static Dictionary<string, string> BuildTickerSectors() => new()
	{
		["AAPL"] = "Technology",
		["NVDA"] = "Technology",
		["MSFT"] = "Technology",
		["GOOGL"] = "Technology",
		["META"] = "Technology",
		["JPM"] = "Financials",
		["AMZN"] = "Consumer Discretionary",
		["TSLA"] = "Consumer Discretionary",
	};

	// ── Helper: seeded random walk that hits exact start/end values ─────

	private static IImmutableList<ChartPoint> GenerateRandomWalk(
		DateTimeOffset startDate, int days, double startValue, double endValue, int seed)
	{
		var rng = new Random(seed);
		var builder = ImmutableList.CreateBuilder<ChartPoint>();

		// Generate raw random walk
		var raw = new double[days];
		raw[0] = 0.0;
		for (var i = 1; i < days; i++)
		{
			// Daily volatility ~1.2%
			var dailyReturn = (rng.NextDouble() - 0.48) * 0.024;
			raw[i] = raw[i - 1] + dailyReturn;
		}

		// Scale so that raw[0] → startValue and raw[days-1] → endValue
		var rawRange = raw[days - 1] - raw[0];
		var targetRange = endValue - startValue;

		for (var i = 0; i < days; i++)
		{
			double value;
			if (Math.Abs(rawRange) < 1e-10)
			{
				// Flat walk — linearly interpolate
				value = startValue + (targetRange * i / (days - 1));
			}
			else
			{
				var fraction = (raw[i] - raw[0]) / rawRange;
				value = startValue + fraction * targetRange;
			}

			var date = startDate.AddDays(i);
			builder.Add(new ChartPoint(date.ToString("yyyy-MM-dd"), (decimal)Math.Round(value, 2)));
		}

		return builder.ToImmutable();
	}
}
