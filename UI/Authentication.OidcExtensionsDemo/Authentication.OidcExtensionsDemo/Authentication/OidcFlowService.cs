using Windows.Security.Authentication.Web;
using ITokenCache = Uno.Extensions.Authentication.ITokenCache;
using TokenCacheExtensions = Uno.Extensions.Authentication.TokenCacheExtensions;

namespace Authentication.OidcExtensionsDemo.Authentication;

/// <summary>
/// Wraps Uno.Extensions' <see cref="IAuthenticationService"/> and narrates every step it takes
/// into <see cref="Log"/> so the whole flow is visible in the app.
/// </summary>
/// <remarks>
/// <para>
/// This is the only type in the sample that talks to the authentication stack. The
/// <c>Uno.Extensions.Authentication.Oidc</c> provider owns the Duende <c>OidcClient</c>: it runs
/// discovery against the authority, drives the interactive flow through the platform's
/// <c>WebAuthenticationBroker</c>, redeems and refreshes tokens, and stores them in
/// <see cref="ITokenCache"/>. The app only sees three calls (<c>LoginAsync</c>,
/// <c>RefreshAsync</c>, <c>LogoutAsync</c>).
/// </para>
/// <para>
/// Registered as a singleton in <c>App.xaml.cs</c> so the log and sign-in state survive the app's
/// lifetime.
/// </para>
/// </remarks>
public sealed class OidcFlowService
{
    /// <summary>The public Duende demo identity server this sample signs in against.</summary>
    public const string Authority = "https://demo.duendesoftware.com/";

    /// <summary>The demo server's confidential interactive client (published secret: "secret").</summary>
    public const string ClientId = "interactive.confidential";

    public const string Scope = "openid profile email api offline_access";

    /// <summary>The demo server's test API; echoes the claims of a valid access token.</summary>
    public const string ApiEndpoint = "https://demo.duendesoftware.com/api/test";

    private readonly IAuthenticationService _auth;
    private readonly ITokenCache _tokens;
    private readonly IHttpClientFactory _httpClientFactory;

    public OidcFlowService(IAuthenticationService auth, ITokenCache tokens, IHttpClientFactory httpClientFactory)
    {
        _auth = auth;
        _tokens = tokens;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>Step-by-step narration of the flow, bound to the UI.</summary>
    public AuthFlowLog Log { get; } = new();

    /// <summary>Whether the last authentication call left the app signed in.</summary>
    public bool IsSignedIn { get; private set; }

    /// <summary>The access token currently held in <see cref="ITokenCache"/>, if any.</summary>
    public string? AccessToken { get; private set; }

    /// <summary>Raised whenever the sign-in state changes.</summary>
    public event EventHandler? StateChanged;

    /// <summary>
    /// The redirect URI the provider uses on this platform - derived from
    /// <see cref="WebAuthenticationBroker.GetCurrentApplicationCallbackUri"/> because the sample
    /// enables <c>AutoRedirectUriFromWebAuthenticationBroker</c>. On Skia Desktop that is the
    /// loopback listener Uno.Extensions registers; on Android/iOS it comes from the app's custom
    /// scheme; on WebAssembly it is the app's origin. The demo server accepts arbitrary redirect
    /// URIs, so nothing needs registering.
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

    /// <summary>
    /// The silent path a real app should run at startup: redeem the stored refresh token against
    /// the token endpoint. No UI is ever shown.
    /// </summary>
    public async Task<bool> RefreshAsync(CancellationToken ct = default)
    {
        Log.Call(
            "IAuthenticationService.RefreshAsync()",
            """
            Silent only - the provider redeems the refresh token held in ITokenCache against the
            token endpoint. False comes back when there is no refresh token, or when the identity
            provider rejects it (expired, revoked) - in either case interaction is required.
            """);

        try
        {
            var refreshed = await _auth.RefreshAsync(ct);

            if (refreshed)
            {
                await PublishAsync(signedIn: true, ct);
                Log.Success("Silent refresh succeeded", await DescribeTokensAsync(ct));
            }
            else
            {
                await PublishAsync(signedIn: false, ct);
                Log.Warning(
                    "Nothing to refresh",
                    """
                    No usable refresh token in ITokenCache, so interaction is required. This is
                    normal on first run. After a sign-in, the tokens persist in the platform's
                    key-value storage, so a restart lands on the success path instead.
                    """);
            }

            return refreshed;
        }
        catch (Exception ex)
        {
            LogException("Silent refresh failed", ex);
            return false;
        }
    }

    /// <summary>
    /// The interactive flow: discovery, then the authorization request in the platform's
    /// browser surface, then code redemption - all inside one provider call.
    /// </summary>
    public async Task<bool> SignInAsync(IDispatcher dispatcher, CancellationToken ct = default)
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
                await PublishAsync(signedIn: true, ct);
                Log.Success("Sign-in succeeded", await DescribeTokensAsync(ct));
            }
            else
            {
                await PublishAsync(signedIn: false, ct);
                Log.Warning(
                    "Sign-in did not complete",
                    "LoginAsync returned false - the browser flow was dismissed or the identity "
                    + "provider returned an error. Details, if any, are in the application log.");
            }

            return success;
        }
        catch (OperationCanceledException)
        {
            Log.Warning("Sign-in cancelled", "The sign-in UI was dismissed before completing.");
            return false;
        }
        catch (Exception ex)
        {
            LogException("Sign-in failed", ex);
            return false;
        }
    }

    /// <summary>
    /// Drives the end-session flow at the identity provider, then clears
    /// <see cref="ITokenCache"/>.
    /// </summary>
    public async Task SignOutAsync(IDispatcher dispatcher, CancellationToken ct = default)
    {
        Log.Call(
            "IAuthenticationService.LogoutAsync()",
            "The provider opens the end-session endpoint in the browser surface; when that "
            + "completes, the ITokenCache is cleared. Backing out of the browser keeps the "
            + "session - a cancelled sign-out must not sign the user out locally.");

        try
        {
            var loggedOut = await _auth.LogoutAsync(dispatcher, ct);

            await PublishAsync(signedIn: !loggedOut && IsSignedIn, ct);

            if (loggedOut)
            {
                Log.Success("Signed out", "The end-session flow completed and the token cache was cleared.");
            }
            else
            {
                Log.Warning("Sign-out did not complete", "The end-session flow was cancelled or failed; the session was kept.");
            }
        }
        catch (Exception ex)
        {
            LogException("Sign-out failed", ex);
        }
    }

    /// <summary>
    /// Calls the demo server's test API with the access token, proving the token a real API
    /// accepts came through the extensions pipeline.
    /// </summary>
    public async Task<string> CallApiAsync(CancellationToken ct = default)
    {
        var token = await _tokens.AccessTokenAsync(ct);
        if (string.IsNullOrEmpty(token))
        {
            return "No access token in ITokenCache - sign in first.";
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
                return body;
            }

            Log.Warning($"API returned {(int)response.StatusCode}");
            return $"{(int)response.StatusCode} {response.ReasonPhrase}{Environment.NewLine}{body}";
        }
        catch (Exception ex)
        {
            Log.Error("API call failed", ex.Message);
            return $"{ex.GetType().Name}: {ex.Message}";
        }
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

    private async Task PublishAsync(bool signedIn, CancellationToken ct)
    {
        IsSignedIn = signedIn;

        try
        {
            var tokens = await _tokens.GetAsync(ct);
            AccessToken = tokens.TryGetValue(TokenCacheExtensions.AccessTokenKey, out var token)
                && !string.IsNullOrEmpty(token)
                ? token
                : null;
        }
        catch (Exception)
        {
            AccessToken = null;
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void LogException(string title, Exception exception)
    {
        Log.Error(
            title,
            $"""
            {exception.GetType().Name}
            {exception.Message}

            {Troubleshoot(exception)}
            """);
    }

    /// <summary>Maps the failures this sample runs into most often onto the fix.</summary>
    private static string Troubleshoot(Exception exception) => exception switch
    {
        InvalidOperationException { Message: var message } when message.Contains("IIdentityTokenValidator") =>
            "Duende's OidcClient needs either an IIdentityTokenValidator or "
            + "Policy.RequireIdentityTokenSignature = false (what this sample sets in App.xaml.cs). "
            + "If you see this, that option was removed.",

        NotImplementedException =>
            "WebAuthenticationBroker has no implementation on this target. On Skia Desktop this "
            + "means the loopback broker Uno.Extensions registers in AddOidc did not take - check "
            + "that the Uno.Extensions.Authentication packages are current.",

        HttpRequestException =>
            $"Could not reach {Authority} - this sample needs network access to the public Duende demo server.",

        _ => "The flow log above shows the step that failed."
    };
}
