using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using QuoteCraft.Data.Remote;

namespace QuoteCraft.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly ILogger<AuthService> _logger;
    private readonly string _supabaseUrl;
    private readonly string _supabaseAnonKey;
    private AuthState _currentState;
    private static readonly string _tokenFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "QuoteCraft", ".auth_token");

    public event Action<AuthState>? AuthStateChanged;

    public AuthService(HttpClient httpClient, ILogger<AuthService> logger)
    {
        _http = httpClient;
        _logger = logger;

        // These should come from configuration; placeholder values for setup
        _supabaseUrl = SupabaseConfig.Url;
        _supabaseAnonKey = SupabaseConfig.AnonKey;

        _currentState = new AuthState(false, null, null, null, null);
        _ = RestoreSessionAsync();
    }

    public AuthState CurrentState => _currentState;

    public async Task<AuthState> SignUpAsync(string email, string password)
    {
        var url = $"{_supabaseUrl}/auth/v1/signup";
        var body = new { email, password };

        var request = CreateAuthRequest(HttpMethod.Post, url, body);
        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<SupabaseAuthResponse>();
        return UpdateState(result);
    }

    public async Task<AuthState> SignInAsync(string email, string password)
    {
        var url = $"{_supabaseUrl}/auth/v1/token?grant_type=password";
        var body = new { email, password };

        var request = CreateAuthRequest(HttpMethod.Post, url, body);
        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<SupabaseAuthResponse>();
        return UpdateState(result);
    }

    public async Task<AuthState> SignInWithMagicLinkAsync(string email)
    {
        var url = $"{_supabaseUrl}/auth/v1/magiclink";
        var body = new { email };

        var request = CreateAuthRequest(HttpMethod.Post, url, body);
        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        // Magic link doesn't return tokens immediately — user must click email link
        return _currentState;
    }

    public async Task SignOutAsync()
    {
        if (_currentState.AccessToken is not null)
        {
            try
            {
                var url = $"{_supabaseUrl}/auth/v1/logout";
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("apikey", _supabaseAnonKey);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _currentState.AccessToken);
                await _http.SendAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to call Supabase logout endpoint");
            }
        }

        _currentState = new AuthState(false, null, null, null, null);
        DeleteStoredToken();
        AuthStateChanged?.Invoke(_currentState);
    }

    public async Task<AuthState> RefreshSessionAsync()
    {
        if (string.IsNullOrEmpty(_currentState.RefreshToken))
            return _currentState;

        try
        {
            var url = $"{_supabaseUrl}/auth/v1/token?grant_type=refresh_token";
            var body = new { refresh_token = _currentState.RefreshToken };

            var request = CreateAuthRequest(HttpMethod.Post, url, body);
            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<SupabaseAuthResponse>();
            return UpdateState(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to refresh session");
            return _currentState;
        }
    }

    public async Task<bool> IsSessionValidAsync()
    {
        if (!_currentState.IsAuthenticated || string.IsNullOrEmpty(_currentState.AccessToken))
            return false;

        try
        {
            var url = $"{_supabaseUrl}/auth/v1/user";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("apikey", _supabaseAnonKey);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _currentState.AccessToken);

            var response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode) return true;

            // Try refresh if access token expired
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshed = await RefreshSessionAsync();
                return refreshed.IsAuthenticated;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Session validation failed");
        }

        return false;
    }

    private HttpRequestMessage CreateAuthRequest(HttpMethod method, string url, object? body = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Add("apikey", _supabaseAnonKey);

        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        return request;
    }

    private AuthState UpdateState(SupabaseAuthResponse? result)
    {
        if (result?.AccessToken is null || result.User is null)
        {
            return _currentState;
        }

        _currentState = new AuthState(
            IsAuthenticated: true,
            UserId: result.User.Id,
            Email: result.User.Email,
            AccessToken: result.AccessToken,
            RefreshToken: result.RefreshToken
        );

        PersistToken(_currentState);
        AuthStateChanged?.Invoke(_currentState);
        return _currentState;
    }

    private async Task RestoreSessionAsync()
    {
        try
        {
            if (!File.Exists(_tokenFilePath)) return;

            var json = await File.ReadAllTextAsync(_tokenFilePath);
            var stored = JsonSerializer.Deserialize<StoredSession>(json);
            if (stored?.RefreshToken is null) return;

            _currentState = new AuthState(false, stored.UserId, stored.Email, null, stored.RefreshToken);
            await RefreshSessionAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to restore auth session");
        }
    }

    private static void PersistToken(AuthState state)
    {
        try
        {
            var dir = Path.GetDirectoryName(_tokenFilePath)!;
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var stored = new StoredSession(state.UserId, state.Email, state.RefreshToken);
            File.WriteAllText(_tokenFilePath, JsonSerializer.Serialize(stored));
        }
        catch { }
    }

    private static void DeleteStoredToken()
    {
        try { if (File.Exists(_tokenFilePath)) File.Delete(_tokenFilePath); } catch { }
    }

    private record StoredSession(string? UserId, string? Email, string? RefreshToken);
}

internal class SupabaseAuthResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public int? ExpiresIn { get; set; }

    [JsonPropertyName("user")]
    public SupabaseUser? User { get; set; }
}

internal class SupabaseUser
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }
}
