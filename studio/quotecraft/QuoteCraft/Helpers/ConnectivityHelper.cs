namespace QuoteCraft.Helpers;

/// <summary>
/// Monitors network connectivity and exposes online/offline state.
/// </summary>
public class ConnectivityHelper : IDisposable
{
    private readonly HttpClient _http;
    private readonly Data.Remote.SupabaseConfig _supabase;
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

    public ConnectivityHelper(HttpClient httpClient, Data.Remote.SupabaseConfig supabase)
    {
        _http = httpClient;
        _supabase = supabase;
    }

    /// <summary>
    /// Start periodic connectivity checks every 15 seconds. Connectivity only matters for sync, so a local-only build
    /// (no Supabase project) never polls.
    /// </summary>
    public void StartMonitoring()
    {
        if (!_supabase.IsConfigured)
            return;

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
        if (!_supabase.IsConfigured)
            return IsOnline;

        try
        {
            // Online means the Supabase project answers at all; any HTTP status will do.
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            using var response = await _http.SendAsync(
                new HttpRequestMessage(HttpMethod.Head, _supabase.Url),
                cts.Token);
            IsOnline = true;
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
