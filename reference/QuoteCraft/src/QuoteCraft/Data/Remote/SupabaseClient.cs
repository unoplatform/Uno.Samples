using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using QuoteCraft.Services;

namespace QuoteCraft.Data.Remote;

/// <summary>
/// HTTP client wrapper for Supabase REST API.
/// Handles auth headers, token refresh, and typed API methods.
/// </summary>
public class SupabaseClient
{
    private readonly HttpClient _http;
    private readonly IAuthService _authService;
    private readonly ILogger<SupabaseClient> _logger;
    private readonly string _baseUrl;
    private readonly string _anonKey;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public SupabaseClient(HttpClient httpClient, IAuthService authService, ILogger<SupabaseClient> logger)
    {
        _http = httpClient;
        _authService = authService;
        _logger = logger;
        _baseUrl = SupabaseConfig.Url;
        _anonKey = SupabaseConfig.AnonKey;
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
    /// UPSERT (insert or update on conflict) rows into a Supabase table.
    /// </summary>
    public async Task UpsertAsync<T>(string table, T data)
    {
        var url = $"{_baseUrl}/rest/v1/{table}";
        var request = CreateRequest(HttpMethod.Post, url);
        request.Headers.Add("Prefer", "resolution=merge-duplicates");
        request.Content = JsonContent.Create(data, options: _jsonOptions);

        var response = await SendWithRetryAsync(request);
        response.EnsureSuccessStatusCode();
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
    /// DELETE rows matching a filter.
    /// </summary>
    public async Task DeleteAsync(string table, string filterParam)
    {
        var url = $"{_baseUrl}/rest/v1/{table}?{filterParam}";
        var request = CreateRequest(HttpMethod.Delete, url);

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

    /// <summary>
    /// Invoke a Supabase Edge Function, returning raw string response.
    /// </summary>
    public async Task<string> InvokeFunctionRawAsync(string functionName, object? body = null)
    {
        var url = $"{_baseUrl}/functions/v1/{functionName}";
        var request = CreateRequest(HttpMethod.Post, url);
        if (body is not null)
            request.Content = JsonContent.Create(body, options: _jsonOptions);

        var response = await SendWithRetryAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string url)
    {
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
            if (refreshed.IsAuthenticated)
            {
                // Clone the request with new token
                var retry = new HttpRequestMessage(request.Method, request.RequestUri);
                retry.Headers.Add("apikey", _anonKey);
                retry.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", refreshed.AccessToken!);
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
