using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using QuoteCraft.Data.Remote;

namespace QuoteCraft.Services;

// Trimmed heads (WebAssembly): the JSON payload types are this assembly's own (DTOs and anonymous types), and
// Platforms/WebAssembly/LinkerConfig.xml preserves the assembly whole, so reflection serialization finds their members.
[System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming", "IL2026",
    Justification = "Serialized types are in this assembly, which LinkerConfig.xml preserves.")]
public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly ILogger<AuthService> _logger;
    private readonly SupabaseConfig _config;
    private readonly string _supabaseUrl;
    private readonly string _supabaseAnonKey;
    private AuthState _currentState;

    // The refresh token: in the platform's vault on Android, iOS and WinAppSDK, a file elsewhere (DECISIONS D13).
    private readonly StoredSecret _storedSession =
        new("auth-session", AppStorage.Combine(".auth_token"), StoredSecret.PlatformVault);

    public event Action<AuthState>? AuthStateChanged;

    public AuthService(HttpClient httpClient, SupabaseConfig config, ILogger<AuthService> logger)
    {
        _http = httpClient;
        _logger = logger;
        _config = config;
        _supabaseUrl = config.Url;
        _supabaseAnonKey = config.AnonKey;

        _currentState = new AuthState(false, null, null, null, null);

        // Local-first: with no Supabase project there is no account to restore.
        if (config.IsConfigured)
        {
            _ = RestoreSessionAsync();
        }
    }

    public bool IsAvailable => _config.IsConfigured;

    public AuthState CurrentState => _currentState;

    public async Task<AuthState> SignUpAsync(string email, string password)
    {
        _config.EnsureConfigured();
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
        _config.EnsureConfigured();
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
        _config.EnsureConfigured();
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
            if (_storedSession.Read() is not { } json) return;

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

    private void PersistToken(AuthState state)
    {
        try
        {
            var stored = new StoredSession(state.UserId, state.Email, state.RefreshToken);
            _storedSession.Write(JsonSerializer.Serialize(stored));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to store the auth session");
        }
    }

    private void DeleteStoredToken()
    {
        try
        {
            _storedSession.Delete();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete the stored auth session");
        }
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
