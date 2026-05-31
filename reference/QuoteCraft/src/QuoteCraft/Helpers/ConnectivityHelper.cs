namespace QuoteCraft.Helpers;

/// <summary>
/// Monitors network connectivity and exposes online/offline state.
/// </summary>
public class ConnectivityHelper : IDisposable
{
    private readonly ILogger<ConnectivityHelper> _logger;
    private readonly HttpClient _http;
    private Timer? _pollTimer;
    private bool _isOnline = true;

    public event Action<bool>? ConnectivityChanged;

    public bool IsOnline
    {
        get => _isOnline;
        private set
        {
            if (_isOnline != value)
            {
                _isOnline = value;
                ConnectivityChanged?.Invoke(value);
            }
        }
    }

    public ConnectivityHelper(HttpClient httpClient, ILogger<ConnectivityHelper> logger)
    {
        _http = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Start periodic connectivity checks every 15 seconds.
    /// </summary>
    public void StartMonitoring()
    {
        _pollTimer?.Dispose();
        _pollTimer = new Timer(async _ => await CheckConnectivityAsync(), null, TimeSpan.Zero, TimeSpan.FromSeconds(15));
    }

    public void StopMonitoring()
    {
        _pollTimer?.Dispose();
        _pollTimer = null;
    }

    public async Task<bool> CheckConnectivityAsync()
    {
        try
        {
            // Simple connectivity check - HEAD request to Supabase or a known endpoint
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var response = await _http.SendAsync(
                new HttpRequestMessage(HttpMethod.Head, "https://httpbin.org/status/200"),
                cts.Token);
            IsOnline = response.IsSuccessStatusCode;
        }
        catch
        {
            IsOnline = false;
        }

        return IsOnline;
    }

    public void Dispose()
    {
        _pollTimer?.Dispose();
    }
}
