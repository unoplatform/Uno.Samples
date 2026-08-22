using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Windows.Security.Authentication.Web;
using TokenCacheExtensions = Uno.Extensions.Authentication.TokenCacheExtensions;

namespace Authentication.WebExtensionsDemo.Authentication;

/// <summary>
/// The OAuth mechanics the Web provider deliberately leaves to the app: building the
/// authorization request (with PKCE), exchanging the returned code at the token endpoint, and
/// redeeming refresh tokens. <c>WebAuthenticationProvider</c> only drives the browser surface and
/// stores whatever tokens the callbacks hand back - which is exactly what makes it fit any
/// OAuth-ish endpoint, not just OpenID Connect ones.
/// </summary>
/// <remarks>
/// Points at the public Duende demo server so the sample runs with no registration. Registered as
/// a singleton and handed to <c>AddWeb&lt;DuendeOAuthClient&gt;</c>, whose typed callbacks receive
/// it - see App.xaml.cs.
/// </remarks>
public sealed class DuendeOAuthClient
{
    public const string Authority = "https://demo.duendesoftware.com";
    public const string AuthorizeEndpoint = $"{Authority}/connect/authorize";
    public const string TokenEndpoint = $"{Authority}/connect/token";
    public const string EndSessionEndpoint = $"{Authority}/connect/endsession";

    /// <summary>The demo server's confidential interactive client (published secret: "secret").</summary>
    public const string ClientId = "interactive.confidential";
    public const string ClientSecret = "secret";
    public const string Scope = "openid profile email api offline_access";

    /// <summary>The demo server's test API; echoes the claims of a valid access token.</summary>
    public const string ApiEndpoint = $"{Authority}/api/test";

    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>The PKCE verifier for the in-flight authorization request.</summary>
    private string? _codeVerifier;

    public DuendeOAuthClient(IHttpClientFactory httpClientFactory)
        => _httpClientFactory = httpClientFactory;

    /// <summary>
    /// The redirect URI the platform's <see cref="WebAuthenticationBroker"/> returns on: custom
    /// scheme on Android/iOS, the app's origin on WebAssembly, and the loopback listener
    /// Uno.Extensions registers on Skia Desktop. The demo server accepts arbitrary redirect URIs.
    /// </summary>
    public string CallbackUri => WebAuthenticationBroker.GetCurrentApplicationCallbackUri().OriginalString;

    /// <summary>
    /// Builds the authorization-code request with a fresh PKCE pair (the demo server requires
    /// PKCE on code flows, as IdentityServer does by default).
    /// </summary>
    public string BuildAuthorizeUri()
    {
        _codeVerifier = Base64Url(RandomNumberGenerator.GetBytes(32));
        var challenge = Base64Url(SHA256.HashData(Encoding.ASCII.GetBytes(_codeVerifier)));
        var state = Base64Url(RandomNumberGenerator.GetBytes(16));

        return $"{AuthorizeEndpoint}"
            + $"?client_id={Uri.EscapeDataString(ClientId)}"
            + "&response_type=code"
            + $"&scope={Uri.EscapeDataString(Scope)}"
            + $"&redirect_uri={Uri.EscapeDataString(CallbackUri)}"
            + $"&state={state}"
            + $"&code_challenge={challenge}"
            + "&code_challenge_method=S256";
    }

    /// <summary>
    /// Exchanges the authorization code carried on the redirect for tokens, returning them keyed
    /// the way Uno.Extensions' token cache expects - or null when the redirect carries no code
    /// (a failed login).
    /// </summary>
    public async Task<IDictionary<string, string>?> ExchangeCodeAsync(string redirectUri, CancellationToken ct)
    {
        var code = GetQueryValue(redirectUri, "code");
        if (string.IsNullOrEmpty(code) || _codeVerifier is not { } verifier)
        {
            return null;
        }

        return await RequestTokensAsync(
            new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["code"] = code,
                ["redirect_uri"] = CallbackUri,
                ["code_verifier"] = verifier,
            },
            ct);
    }

    /// <summary>Redeems the stored refresh token; null when there is none or the server rejects it.</summary>
    public async Task<IDictionary<string, string>?> RefreshTokensAsync(IDictionary<string, string> tokens, CancellationToken ct)
    {
        if (!tokens.TryGetValue(TokenCacheExtensions.RefreshTokenKey, out var refreshToken)
            || string.IsNullOrEmpty(refreshToken))
        {
            return null;
        }

        return await RequestTokensAsync(
            new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken,
            },
            ct);
    }

    /// <summary>
    /// The end-session URL, with the id_token hint the server wants before it will end the
    /// session without prompting.
    /// </summary>
    public string BuildEndSessionUri(IDictionary<string, string>? tokens)
    {
        var uri = $"{EndSessionEndpoint}?post_logout_redirect_uri={Uri.EscapeDataString(CallbackUri)}";

        if (tokens is not null
            && tokens.TryGetValue(TokenCacheExtensions.IdTokenKey, out var idToken)
            && !string.IsNullOrEmpty(idToken))
        {
            uri += $"&id_token_hint={Uri.EscapeDataString(idToken)}";
        }

        return uri;
    }

    private async Task<IDictionary<string, string>?> RequestTokensAsync(Dictionary<string, string> form, CancellationToken ct)
    {
        form["client_id"] = ClientId;
        form["client_secret"] = ClientSecret;

        using var client = _httpClientFactory.CreateClient("DuendeDemo");
        using var response = await client.PostAsync(TokenEndpoint, new FormUrlEncodedContent(form), ct);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var payload = await response.Content.ReadAsStringAsync(ct);
        var tokens = JsonSerializer.Deserialize(payload, DuendeJsonContext.Default.TokenEndpointResponse);

        if (tokens?.AccessToken is not { Length: > 0 } accessToken)
        {
            return null;
        }

        var result = new Dictionary<string, string>
        {
            [TokenCacheExtensions.AccessTokenKey] = accessToken,
        };
        if (!string.IsNullOrEmpty(tokens.RefreshToken))
        {
            result[TokenCacheExtensions.RefreshTokenKey] = tokens.RefreshToken;
        }
        if (!string.IsNullOrEmpty(tokens.IdToken))
        {
            result[TokenCacheExtensions.IdTokenKey] = tokens.IdToken;
        }

        return result;
    }

    private static string? GetQueryValue(string uri, string key)
    {
        var queryStart = uri.IndexOf('?');
        if (queryStart < 0)
        {
            return null;
        }

        foreach (var pair in uri[(queryStart + 1)..].Split('&'))
        {
            var separatorIndex = pair.IndexOf('=');
            if (separatorIndex > 0 && pair[..separatorIndex] == key)
            {
                return Uri.UnescapeDataString(pair[(separatorIndex + 1)..]);
            }
        }

        return null;
    }

    private static string Base64Url(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}

/// <summary>Shape of the token endpoint response.</summary>
public sealed record TokenEndpointResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; init; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; init; }

    [JsonPropertyName("id_token")]
    public string? IdToken { get; init; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
}

/// <summary>Source-generated serialization, so the WebAssembly and mobile heads stay trim-safe.</summary>
[JsonSerializable(typeof(TokenEndpointResponse))]
public partial class DuendeJsonContext : JsonSerializerContext
{
}
