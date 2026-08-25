using Microsoft.Extensions.Configuration;
using Windows.Security.Authentication.Web;
using ITokenCache = Uno.Extensions.Authentication.ITokenCache;
using TokenCacheExtensions = Uno.Extensions.Authentication.TokenCacheExtensions;

namespace Authentication.OidcExtensionsDemo.AuthFlow;

/// <summary>
/// Wraps Uno.Extensions' <see cref="IAuthenticationService"/> and narrates every step it takes
/// into <see cref="Log"/>, so the whole flow is visible on screen the way it is in the
/// Authentication.MsalExtensionsDemo sample.
/// </summary>
/// <remarks>
/// Registered as a singleton in <c>App.xaml.cs</c> so the log and sign-in state survive the
/// Login/Main page navigation. This is the only type in the sample that talks to the
/// authentication stack; the models stay one-call thin.
/// </remarks>
public sealed class AuthFlowService
{
    /// <summary>The name passed to <c>auth.AddOidc(name: ...)</c> - also the configuration section.</summary>
    public const string ProviderName = "OidcAuthentication";

    /// <summary>The demo server's test API; echoes the claims of a valid access token.</summary>
    public const string ApiEndpoint = "https://demo.duendesoftware.com/api/test";

    private readonly IAuthenticationService _auth;
    private readonly ITokenCache _tokens;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AuthFlowService(
        IAuthenticationService auth,
        ITokenCache tokens,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _auth = auth;
        _tokens = tokens;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    /// <summary>Step-by-step narration of the flow, bound to the UI on both pages.</summary>
    public AuthFlowLog Log { get; } = new();

    /// <summary>Whether the last authentication call left the app signed in.</summary>
    public bool IsSignedIn { get; private set; }

    /// <summary>The most recent demo API response, shown on the Main page.</summary>
    public string? ApiResponse { get; private set; }

    /// <summary>Raised whenever the sign-in state or API response changes.</summary>
    public event EventHandler? StateChanged;

    public string Authority => _configuration[$"{ProviderName}:Authority"] ?? "(not configured)";

    public string ClientId => _configuration[$"{ProviderName}:ClientId"] ?? "(not configured)";

    public string Scope => _configuration[$"{ProviderName}:Scope"] ?? "(not configured)";

    /// <summary>
    /// The redirect URI the provider uses on this platform - derived from
    /// <see cref="WebAuthenticationBroker.GetCurrentApplicationCallbackUri"/> because the app
    /// enables <c>AutoRedirectUriFromWebAuthenticationBroker</c>.
    /// </summary>
    public string RedirectUri
    {
        get
        {
            try
            {
                return WebAuthenticationBroker.GetCurrentApplicationCallbackUri().OriginalString;
            }
            catch (Exception ex)
            {
                return $"(unavailable: {ex.GetType().Name})";
            }
        }
    }

    /// <summary>
    /// What the app runs once at launch, before the first page shows: a narrated silent refresh.
    /// </summary>
    public Task<bool> StartupAsync(CancellationToken ct = default)
    {
        Log.Info(
            $"Ready on {PlatformSupport.PlatformName}",
            $"""
            Authority     {Authority}
            Client        {ClientId}
            Redirect URI  {RedirectUri}

            At startup the app runs the silent path first, exactly like a production app: sign
            back in from the stored refresh token when possible, and only ask the user when
            needed.
            """);

        return RefreshAsync(ct);
    }

    /// <summary>Silent only: redeem the stored refresh token. No UI is ever shown.</summary>
    public async Task<bool> RefreshAsync(CancellationToken ct = default)
    {
        Log.Call(
            "IAuthenticationService.RefreshAsync()",
            "The provider redeems the refresh token held in ITokenCache against the token "
            + "endpoint. False comes back when there is no refresh token, or when the identity "
            + "provider rejects it - in either case interaction is required.");

        try
        {
            var refreshed = await _auth.RefreshAsync(ct);

            if (refreshed)
            {
                Log.Success("Silent refresh succeeded", await DescribeTokensAsync(ct));
            }
            else
            {
                Log.Warning(
                    "Nothing to refresh",
                    "No usable refresh token in ITokenCache, so interaction is required. This is "
                    + "normal on first run.");
            }

            Publish(refreshed);
            return refreshed;
        }
        catch (Exception ex)
        {
            LogException("Silent refresh failed", ex);
            Publish(false);
            return false;
        }
    }

    /// <summary>
    /// The interactive flow: discovery, the authorization request in the platform's browser
    /// surface, then code redemption - all inside one provider call.
    /// </summary>
    public async Task<bool> SignInAsync(IDispatcher? dispatcher, CancellationToken ct = default)
    {
        Log.Call(
            "IAuthenticationService.LoginAsync(dispatcher)",
            $"""
            The provider runs OIDC discovery against the authority, opens the authorization
            request in {PlatformSupport.SignInSurface}, and redeems the returned code at the
            token endpoint. Sign in with the demo server's test user: bob / bob.

            Redirect URI {RedirectUri}
            """);

        try
        {
            var success = await _auth.LoginAsync(dispatcher, cancellationToken: ct);

            if (success)
            {
                Log.Success("Sign-in succeeded", await DescribeTokensAsync(ct));
            }
            else
            {
                Log.Warning(
                    "Sign-in did not complete",
                    "LoginAsync returned false - the browser flow was dismissed or the identity "
                    + "provider returned an error.");
            }

            Publish(success);
            return success;
        }
        catch (OperationCanceledException)
        {
            Log.Warning("Sign-in cancelled", "The sign-in UI was dismissed before completing.");
            Publish(false);
            return false;
        }
        catch (Exception ex)
        {
            LogException("Sign-in failed", ex);
            Publish(false);
            return false;
        }
    }

    /// <summary>
    /// Local sign-out only: clears the cached tokens without the browser end-session round trip.
    /// The identity provider's browser session survives, so the next interactive sign-in may
    /// complete without prompting for credentials.
    /// </summary>
    public async Task SignOutLocallyAsync(CancellationToken ct = default)
    {
        Log.Call(
            "ITokenCache.ClearAsync()",
            "Local sign-out: the cached tokens are cleared and IsAuthenticated flips to false, "
            + "with no browser round trip. The identity provider's own browser session survives, "
            + "so the next sign-in may not prompt for credentials.");

        try
        {
            await _tokens.ClearAsync(ct);
            Log.Success("Signed out locally", "ITokenCache: empty");
        }
        catch (Exception ex)
        {
            LogException("Local sign-out failed", ex);
        }

        Publish(false);
    }

    /// <summary>
    /// Full sign-out: drives the identity provider's end-session flow (with the id_token hint) in
    /// the browser, then clears the cached tokens.
    /// </summary>
    public async Task<bool> SignOutEverywhereAsync(IDispatcher? dispatcher, CancellationToken ct = default)
    {
        Log.Call(
            "IAuthenticationService.LogoutAsync()",
            "The provider opens the end-session endpoint (with the cached id_token as the hint) "
            + "in the browser surface; when that completes, the ITokenCache is cleared. Backing "
            + "out keeps the session.");

        try
        {
            var loggedOut = await _auth.LogoutAsync(dispatcher, ct);

            if (loggedOut)
            {
                Log.Success("Signed out", "The end-session flow completed and the token cache was cleared.");
            }
            else
            {
                Log.Warning("Sign-out did not complete", "The end-session flow was cancelled or failed; the session was kept.");
            }

            Publish(!loggedOut && IsSignedIn);
            return loggedOut;
        }
        catch (Exception ex)
        {
            LogException("Sign-out failed", ex);
            return false;
        }
    }

    /// <summary>
    /// Calls the demo server's test API with the access token, proving the token a real API
    /// accepts came through the extensions pipeline.
    /// </summary>
    public async Task CallApiAsync(CancellationToken ct = default)
    {
        var token = await _tokens.AccessTokenAsync(ct);
        if (string.IsNullOrEmpty(token))
        {
            ApiResponse = "No access token in ITokenCache - sign in first.";
            StateChanged?.Invoke(this, EventArgs.Empty);
            return;
        }

        Log.Call($"GET {ApiEndpoint}", "Authorization: Bearer <access token from ITokenCache>");

        try
        {
            using var client = _httpClientFactory.CreateClient("DemoApi");
            using var request = new HttpRequestMessage(HttpMethod.Get, ApiEndpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using var response = await client.SendAsync(request, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            if (response.IsSuccessStatusCode)
            {
                Log.Success($"API returned {(int)response.StatusCode}");
                ApiResponse = body;
            }
            else
            {
                Log.Warning($"API returned {(int)response.StatusCode}");
                ApiResponse = $"{(int)response.StatusCode} {response.ReasonPhrase}{Environment.NewLine}{body}";
            }
        }
        catch (Exception ex)
        {
            Log.Error("API call failed", ex.Message);
            ApiResponse = $"{ex.GetType().Name}: {ex.Message}";
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>What the app can see of the token cache, for the TOKEN card in the UI.</summary>
    public async Task<string> DescribeTokensAsync(CancellationToken ct = default)
    {
        try
        {
            var provider = await _tokens.GetCurrentProviderAsync(ct);
            var tokens = await _tokens.GetAsync(ct);

            if (tokens.Count == 0)
            {
                return "ITokenCache: empty";
            }

            var entries = string.Join(
                Environment.NewLine,
                tokens.Select(pair => $"{pair.Key,-13} {pair.Value.Length} chars"));

            return $"""
                Provider      {provider ?? "(none)"}
                {entries}
                """;
        }
        catch (Exception ex)
        {
            return $"ITokenCache could not be read: {ex.GetType().Name}";
        }
    }

    private void Publish(bool signedIn)
    {
        IsSignedIn = signedIn;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void LogException(string title, Exception exception) =>
        Log.Error(
            title,
            $"""
            {exception.GetType().Name}
            {exception.Message}
            """);
}
