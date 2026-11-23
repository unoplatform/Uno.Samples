using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuoteCraft.Data.Remote;

/// <summary>
/// HTTP client wrapper for Supabase REST API.
/// Handles auth headers, token refresh, and typed API methods.
/// </summary>
// Trimmed heads (WebAssembly): the JSON payload types are this assembly's own (DTOs and anonymous types), and
// Platforms/WebAssembly/LinkerConfig.xml preserves the assembly whole, so reflection serialization finds their members.
[System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming", "IL2026",
    Justification = "Serialized types are in this assembly, which LinkerConfig.xml preserves.")]
public class SupabaseClient
{
    private readonly HttpClient _http;
    private readonly IAuthService _authService;
    private readonly ILogger<SupabaseClient> _logger;
    private readonly SupabaseConfig _config;
    private readonly string _baseUrl;
    private readonly string _anonKey;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public SupabaseClient(HttpClient httpClient, IAuthService authService, SupabaseConfig config, ILogger<SupabaseClient> logger)
    {
        _http = httpClient;
        _authService = authService;
        _logger = logger;
        _config = config;
        _baseUrl = config.Url;
        _anonKey = config.AnonKey;
    }

    /// <summary>
    /// GET rows from a Supabase table with optional query parameters.
    /// </summary>
    public async Task<List<T>> GetAsync<T>(string table, string? queryParams = null)
    {
        var url = $"{_baseUrl}/rest/v1/{table}";
        if (!string.IsNullOrEmpty(queryParams))
            url += $"?{queryParams}";

        var request = CreateRequest(HttpMethod.Get, url);
        var response = await SendWithRetryAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<T>>(_jsonOptions) ?? [];
    }

    /// <summary>
    /// UPSERT multiple rows in batch.
    /// </summary>
    public async Task UpsertBatchAsync<T>(string table, List<T> data)
    {
        if (data.Count == 0) return;

        var url = $"{_baseUrl}/rest/v1/{table}";
        var request = CreateRequest(HttpMethod.Post, url);
        request.Headers.Add("Prefer", "resolution=merge-duplicates");
        request.Content = JsonContent.Create(data, options: _jsonOptions);

        var response = await SendWithRetryAsync(request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Invoke a Supabase Edge Function.
    /// </summary>
    public async Task<T?> InvokeFunctionAsync<T>(string functionName, object? body = null)
    {
        var url = $"{_baseUrl}/functions/v1/{functionName}";
        var request = CreateRequest(HttpMethod.Post, url);
        if (body is not null)
            request.Content = JsonContent.Create(body, options: _jsonOptions);

        var response = await SendWithRetryAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string url)
    {
        _config.EnsureConfigured();

        var request = new HttpRequestMessage(method, url);
        request.Headers.Add("apikey", _anonKey);

        var token = _authService.CurrentState.AccessToken;
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        return request;
    }

    private async Task<HttpResponseMessage> SendWithRetryAsync(HttpRequestMessage request)
    {
        var response = await _http.SendAsync(request);

        // If unauthorized, try refreshing the token and retry once
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _logger.LogInformation("Access token expired, attempting refresh");
            var refreshed = await _authService.RefreshSessionAsync();
            if (refreshed is { IsAuthenticated: true, AccessToken: { } token })
            {
                var retry = new HttpRequestMessage(request.Method, request.RequestUri);
                retry.Headers.Add("apikey", _anonKey);
                retry.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                if (request.Content is not null)
                {
                    var contentBytes = await request.Content.ReadAsByteArrayAsync();
                    retry.Content = new ByteArrayContent(contentBytes);
                    retry.Content.Headers.ContentType = request.Content.Headers.ContentType;
                }

                response = await _http.SendAsync(retry);
            }
        }

        return response;
    }
}
