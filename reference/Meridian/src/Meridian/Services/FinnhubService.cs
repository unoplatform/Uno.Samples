using System.Net.Http;
using System.Text.Json;
using Meridian.Models;

namespace Meridian.Services;

/// <summary>
/// Fetches real-time stock quotes from Finnhub API.
/// Free tier: 60 calls/minute. We poll 6 tickers every 15s = 24 calls/min.
/// Polling runs on a background thread; events are raised for UI-thread dispatch.
/// </summary>
public sealed class FinnhubService : IDisposable
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly string[] _tickers;
    private readonly Dictionary<string, StreamTicker> _latestQuotes = new();
    private readonly object _lock = new();
    private IReadOnlyList<NewsItem> _latestNews = Array.Empty<NewsItem>();
    private CancellationTokenSource? _cts;
    private Task? _pollTask;
    private Task? _newsTask;

    public event Action? QuotesUpdated;
    public event Action<IReadOnlyList<NewsItem>>? NewsUpdated;

    public FinnhubService(string apiKey, string[] tickers)
    {
        _apiKey = apiKey;
        _tickers = tickers;
        _http = new HttpClient
        {
            BaseAddress = new Uri("https://finnhub.io/api/v1/"),
            Timeout = TimeSpan.FromSeconds(10)
        };

        foreach (var t in _tickers)
            _latestQuotes[t] = new StreamTicker(t, "$0.00", "0.00%", true);
    }

    public void Start()
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            System.Diagnostics.Debug.WriteLine("FinnhubService: No API key configured. Set FINNHUB_API_KEY env var to enable live data.");
            return;
        }

        _cts = new CancellationTokenSource();
        var ct = _cts.Token;

        _pollTask = Task.Run(() => PollLoopAsync(TimeSpan.FromSeconds(15), PollAllAsync, ct), ct);
        _newsTask = Task.Run(() => PollLoopAsync(TimeSpan.FromSeconds(60), PollNewsAsync, ct), ct);
    }

    public void Stop()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    public IReadOnlyList<StreamTicker> GetLatestQuotes()
    {
        lock (_lock)
        {
            return _tickers
                .Where(t => _latestQuotes.ContainsKey(t))
                .Select(t => _latestQuotes[t])
                .ToList();
        }
    }

    public IReadOnlyList<NewsItem> GetLatestNews() => _latestNews;

    private static async Task PollLoopAsync(TimeSpan interval, Func<CancellationToken, Task> action, CancellationToken ct)
    {
        // Fire immediately, then loop on interval
        await action(ct);
        using var timer = new PeriodicTimer(interval);
        while (await timer.WaitForNextTickAsync(ct))
        {
            await action(ct);
        }
    }

    private async Task PollAllAsync(CancellationToken ct)
    {
        // Fetch all tickers in parallel
        var tasks = _tickers.Select(ticker => PollTickerAsync(ticker, ct));
        await Task.WhenAll(tasks);
        QuotesUpdated?.Invoke();
    }

    private async Task PollTickerAsync(string ticker, CancellationToken ct)
    {
        try
        {
            var response = await _http.GetAsync($"quote?symbol={ticker}&token={_apiKey}", ct);
            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var price = root.GetProperty("c").GetDecimal();
            var change = root.GetProperty("dp").GetDecimal();

            if (price == 0) return;

            var isUp = change >= 0;
            var streamTicker = new StreamTicker(
                ticker,
                $"${price:N2}",
                $"{(isUp ? "+" : "")}{change:N2}%",
                isUp);

            lock (_lock)
            {
                _latestQuotes[ticker] = streamTicker;
            }
        }
        catch (OperationCanceledException) { /* expected on shutdown */ }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Finnhub error for {ticker}: {ex.Message}");
        }
    }

    private async Task PollNewsAsync(CancellationToken ct)
    {
        try
        {
            var response = await _http.GetAsync($"news?category=general&minId=0&token={_apiKey}", ct);
            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            var items = new List<NewsItem>();

            foreach (var el in doc.RootElement.EnumerateArray())
            {
                if (items.Count >= 6) break;

                var headline = el.TryGetProperty("headline", out var h) ? h.GetString() ?? "" : "";
                var source = el.TryGetProperty("source", out var s) ? s.GetString() ?? "" : "";
                var category = el.TryGetProperty("category", out var c) ? c.GetString() ?? "" : "";
                var unixTime = el.TryGetProperty("datetime", out var dt) ? dt.GetInt64() : 0;

                if (string.IsNullOrWhiteSpace(headline)) continue;

                var publishedAt = DateTimeOffset.FromUnixTimeSeconds(unixTime);
                var ago = DateTimeOffset.UtcNow - publishedAt;
                var timeText = ago.TotalMinutes < 1 ? "just now"
                    : ago.TotalMinutes < 60 ? $"{(int)ago.TotalMinutes}m"
                    : ago.TotalHours < 24 ? $"{(int)ago.TotalHours}h"
                    : $"{(int)ago.TotalDays}d";

                var tag = string.IsNullOrWhiteSpace(source)
                    ? CapitalizeCategory(category)
                    : source.Length > 12 ? source[..12] : source;

                items.Add(new NewsItem(timeText, headline, tag));
            }

            if (items.Count > 0)
            {
                _latestNews = items;
                NewsUpdated?.Invoke(items);
            }
        }
        catch (OperationCanceledException) { /* expected on shutdown */ }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Finnhub news error: {ex.Message}");
        }
    }

    private static string CapitalizeCategory(string cat) =>
        string.IsNullOrEmpty(cat) ? "Market"
        : char.ToUpper(cat[0]) + cat[1..];

    public void Dispose()
    {
        Stop();
        _http.Dispose();
    }
}
