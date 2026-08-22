using Windows.Security.Authentication.Web;
using ITokenCache = Uno.Extensions.Authentication.ITokenCache;
using TokenCacheExtensions = Uno.Extensions.Authentication.TokenCacheExtensions;

namespace Authentication.WebExtensionsDemo.Authentication;

/// <summary>
/// Wraps Uno.Extensions' <see cref="IAuthenticationService"/> and narrates every step it takes
/// into <see cref="Log"/> so the whole flow is visible in the app.
/// </summary>
/// <remarks>
/// <para>
/// This is the only type in the sample that talks to the authentication service. The
/// <c>Uno.Extensions.Authentication</c> Web provider drives the platform's
/// <c>WebAuthenticationBroker</c> and stores tokens in <see cref="ITokenCache"/>; the OAuth
/// mechanics (PKCE, code exchange, refresh) live in <see cref="DuendeOAuthClient"/>, plugged in
/// through <c>AddWeb</c>'s callbacks in App.xaml.cs.
/// </para>
/// <para>
/// Registered as a singleton in <c>App.xaml.cs</c> so the log and sign-in state survive the app's
/// lifetime.
/// </para>
/// </remarks>
public sealed class WebFlowService
{
    public const string Authority = DuendeOAuthClient.Authority;
    public const string ClientId = DuendeOAuthClient.ClientId;
    public const string Scope = DuendeOAuthClient.Scope;
    public const string ApiEndpoint = DuendeOAuthClient.ApiEndpoint;

    private readonly IAuthenticationService _auth;
    private readonly ITokenCache _tokens;
    private readonly IHttpClientFactory _httpClientFactory;

    public WebFlowService(IAuthenticationService auth, ITokenCache tokens, IHttpClientFactory httpClientFactory)
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
    /// The redirect URI the platform's <see cref="WebAuthenticationBroker"/> returns on. On Skia
    /// Desktop that is the loopback listener Uno.Extensions registers; on Android/iOS it comes
    /// from the app's custom scheme; on WebAssembly it is the app's origin. The demo server
    /// accepts arbitrary redirect URIs, so nothing needs registering.
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
    /// The silent path a real app should run at startup: the provider hands the stored tokens to
    /// the Refresh callback, which redeems the refresh token. No UI is ever shown.
    /// </summary>
    public async Task<bool> RefreshAsync(CancellationToken ct = default)
    {
        Log.Call(
            "IAuthenticationService.RefreshAsync()",
            """
            Silent only - the Web provider hands the stored tokens to the Refresh callback
            (DuendeOAuthClient.RefreshTokensAsync), which redeems the refresh token at the token
            endpoint. False comes back when there is no refresh token, or when the identity
            provider rejects it - in either case interaction is required.
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
    /// The interactive flow: the PrepareLoginStartUri callback builds the PKCE authorization
    /// request, the provider opens it in the platform's browser surface, and the PostLogin
    /// callback exchanges the returned code for tokens.
    /// </summary>
    public async Task<bool> SignInAsync(IDispatcher dispatcher, CancellationToken ct = default)
    {
        Log.Call(
            "IAuthenticationService.LoginAsync(dispatcher)",
            $"""
            The provider opens the authorization request in {PlatformSupport.SignInSurface}.
            When the redirect comes back, the PostLogin callback exchanges the code (with the
            PKCE verifier) at the token endpoint. Sign in with the demo server's test user:
            bob / bob.

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
                    "LoginAsync returned false - the redirect carried no code, or the code "
                    + "exchange was rejected. Details, if any, are in the application log.");
            }

            return success;
        }
        catch (OperationCanceledException)
        {
            Log.Warning("Sign-in cancelled", "The sign-in UI was dismissed before completing - the previous session, if any, is kept.");
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
            "The provider opens the end-session endpoint (with the id_token hint) in the browser "
            + "surface; when that completes, the ITokenCache is cleared. Backing out keeps the "
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
        NotImplementedException =>
            "WebAuthenticationBroker has no implementation on this target. On Skia Desktop this "
            + "means the loopback broker Uno.Extensions registers in AddWeb did not take - check "
            + "that the Uno.Extensions.Authentication packages are current.",

        HttpRequestException =>
            $"Could not reach {Authority} - this sample needs network access to the public Duende demo server.",

        _ => "The flow log above shows the step that failed."
    };
}
