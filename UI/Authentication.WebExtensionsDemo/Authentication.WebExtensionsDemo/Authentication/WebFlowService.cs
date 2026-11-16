using System.Net;
using System.Security.Cryptography;
using Authentication.WebExtensionsDemo.Common;
using Authentication.WebExtensionsDemo.DataContracts;
using Authentication.WebExtensionsDemo.Services.Endpoints;
using Microsoft.Extensions.Configuration;
using Refit;
using Uno.Extensions.Authentication.Web;
using ITokenCache = Uno.Extensions.Authentication.ITokenCache;
using TokenCacheExtensions = Uno.Extensions.Authentication.TokenCacheExtensions;

namespace Authentication.WebExtensionsDemo.Authentication;

/// <summary>
/// Wraps Uno.Extensions' <see cref="IAuthenticationService"/>, wires the Web provider's
/// callbacks, and narrates every step into <see cref="Log"/> so the whole flow is visible in the
/// app.
/// </summary>
/// <remarks>
/// <para>
/// The <c>Uno.Extensions.Authentication</c> Web provider (<c>AddWeb</c>) opens
/// <c>LoginStartUri</c> through Uno's <c>WebAuthenticationBroker</c> (on the Skia desktop head,
/// the loopback broker <c>AddWeb</c> registers), waits for the browser to reach the callback URI,
/// and reads <c>access_token</c> and <c>refresh_token</c> out of that URL's query or fragment.
/// Everything specific to this backend is in the three callbacks <see cref="Configure"/> adds:
/// a <c>state</c> value on the way out, the check of that value on the way back, and the call
/// that trades a refresh token for new tokens.
/// </para>
/// <para>
/// Registered as a singleton in <c>App.xaml.cs</c> so the log and sign-in state survive
/// switching between the app's sections, so the pending <c>state</c> outlives the browser round
/// trip, and so there is exactly one <c>LoggedOut</c> subscription for the life of the app.
/// </para>
/// </remarks>
public sealed class WebFlowService
{
    /// <summary>The name passed to <c>auth.AddWeb(name: ...)</c> - also the configuration section.</summary>
    public const string ProviderName = "WebAuthentication";

    private const string StateParameter = "state";

    private readonly IAuthenticationService _auth;
    private readonly ITokenCache _tokens;
    private readonly IConfiguration _configuration;

    /// <summary>The <c>state</c> sent with the sign-in in flight; one use only.</summary>
    private string? _pendingState;

    public WebFlowService(
        IAuthenticationService auth,
        ITokenCache tokens,
        IConfiguration configuration,
        SecretRedactor redactor)
    {
        _auth = auth;
        _tokens = tokens;
        _configuration = configuration;

        Log = new AuthFlowLog(redactor);

        // Raised whenever the stored tokens are cleared: an explicit LogoutAsync, a RefreshAsync
        // that could not renew the session, or an API 401 that a refresh could not fix. A real
        // app navigates to its login page from here; this one records it and updates its state.
        _auth.LoggedOut += OnLoggedOut;
    }

    /// <summary>
    /// The backend-specific part of the provider. <c>AddWeb&lt;IAuthApi&gt;</c> rather than
    /// <c>AddWeb</c> because only the typed builder's <c>PostLogin</c> receives the callback URL,
    /// which is where the <c>state</c> comes back, and because its <c>Refresh</c> receives the
    /// Refit client that calls the refresh endpoint.
    /// </summary>
    public static void Configure(IWebAuthenticationBuilder<IAuthApi> web) => web
        .PrepareLoginStartUri((api, services, tokens, credentials, loginStartUri, ct) =>
            ValueTask.FromResult(services.GetRequiredService<WebFlowService>().AddState(loginStartUri)))
        .PostLogin((api, services, tokens, credentials, callbackUrl, extracted, ct) =>
            ValueTask.FromResult(services.GetRequiredService<WebFlowService>().CheckState(callbackUrl, extracted)))
        .Refresh((api, services, tokens, stored, ct) =>
            services.GetRequiredService<WebFlowService>().RefreshTokensAsync(api, stored, ct));

    /// <summary>Step-by-step narration of the flow, bound to the UI.</summary>
    public AuthFlowLog Log { get; }

    /// <summary>Whether the last authentication call left the app signed in.</summary>
    public bool IsSignedIn { get; private set; }

    /// <summary>The access token currently held in <see cref="ITokenCache"/>, if any.</summary>
    public string? AccessToken { get; private set; }

    /// <summary>Raised whenever the sign-in state changes.</summary>
    public event EventHandler? StateChanged;

    public string? LoginStartUri => _configuration[$"{ProviderName}:LoginStartUri"];

    public string? LogoutStartUri => _configuration[$"{ProviderName}:LogoutStartUri"];

    /// <summary>The <c>AuthApi</c> endpoint: the backend's own API, which serves the refresh call.</summary>
    public string? ApiUrl => _configuration["AuthApi:Url"];

    /// <summary>
    /// Where the browser has to land for the sign-in to complete. With no <c>LoginCallbackUri</c>
    /// configured, the provider takes the <c>redirect_uri</c> query parameter of
    /// <c>LoginStartUri</c>, which is how this sample's configuration is written.
    /// </summary>
    public string CallbackUri =>
        _configuration[$"{ProviderName}:LoginCallbackUri"] is { Length: > 0 } configured
            ? configured
            : RedirectUriOf(LoginStartUri) ?? "(no redirect_uri in LoginStartUri)";

    /// <summary>Whether the section names a login page the provider can open.</summary>
    public bool IsConfigured => Uri.TryCreate(LoginStartUri, UriKind.Absolute, out _);

    /// <summary>
    /// What the app runs once at launch, before the first page shows: a narrated silent refresh.
    /// </summary>
    public Task<bool> StartupAsync(CancellationToken ct = default)
    {
        Log.Info(
            $"Ready on {PlatformSupport.PlatformName}",
            $"""
            Login page   {LoginStartUri}
            Callback URI {CallbackUri}

            At startup the app runs the silent path first, exactly like a production app: renew
            the stored tokens when there are any, and only ask the user when that fails.
            """);

        return RefreshAsync(ct);
    }

    /// <summary>
    /// The silent path a real app should run at startup: trade the stored refresh token for new
    /// tokens through the backend's API, and never show UI.
    /// </summary>
    public async Task<bool> RefreshAsync(CancellationToken ct = default)
    {
        if (!EnsureConfigured())
        {
            return false;
        }

        Log.Call(
            "IAuthenticationService.RefreshAsync()",
            """
            Silent only. With tokens stored, the provider runs the Refresh callback this app
            registered (POST /auth/refresh with the refresh token); with none, it returns false
            without calling anything. A refresh that yields no tokens clears the cache, which
            raises LoggedOut.
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
                    No tokens are stored, or the backend rejected the refresh token, so
                    interaction is required. This is normal on first run, after Sign out, and
                    after the demo server restarts (it keeps its tokens in memory).
                    """);
            }

            return refreshed;
        }
        catch (Exception ex)
        {
            await PublishAsync(signedIn: false, ct, fromTokens: true);
            LogException("Silent refresh failed", ex);
            return false;
        }
    }

    /// <summary>
    /// The interactive flow: the provider opens the backend's login page in the browser and reads
    /// the tokens from the URL the backend redirects to.
    /// </summary>
    public async Task<bool> SignInAsync(IDispatcher dispatcher, CancellationToken ct = default)
    {
        if (!EnsureConfigured())
        {
            return false;
        }

        Log.Call(
            "IAuthenticationService.LoginAsync(dispatcher)",
            $"""
            The provider opens LoginStartUri (plus the state added below) and waits for the
            browser to reach the callback URI, then reads access_token and refresh_token from
            that URL. Credentials are never passed: the backend's page collects them.

            Callback URI {CallbackUri}
            Browser      {InteractiveUiDescription}
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
                    """
                    LoginAsync returned false without throwing: the browser page was closed or
                    cancelled, the redirect carried no access_token, or the state check above
                    rejected it.
                    """);
            }

            return success;
        }
        catch (Exception ex)
        {
            await PublishAsync(signedIn: false, ct, fromTokens: true);
            LogException("Sign-in failed", ex);
            return false;
        }
        finally
        {
            _pendingState = null;
        }
    }

    /// <summary>
    /// Ends the session at the backend (in the browser) and clears <see cref="ITokenCache"/>.
    /// </summary>
    public async Task SignOutAsync(IDispatcher dispatcher, CancellationToken ct = default)
    {
        Log.Call(
            "IAuthenticationService.LogoutAsync(dispatcher)",
            $"""
            The provider opens LogoutStartUri in the browser so the backend can end its own
            session (here, the cookie that lets a second sign-in skip the form), waits for the
            redirect back to the callback URI, then clears the tokens - which raises LoggedOut.

            Logout page {LogoutStartUri}
            """);

        try
        {
            var loggedOut = await _auth.LogoutAsync(dispatcher, ct);
            await PublishAsync(signedIn: false, ct, fromTokens: true);

            if (loggedOut)
            {
                Log.Success("Signed out", "The backend ended its browser session and the tokens were cleared.");
            }
            else
            {
                Log.Warning(
                    "Sign-out did not complete",
                    "LogoutAsync returned false and kept the tokens: the Web provider does this when LogoutStartUri is missing.");
            }
        }
        catch (Exception ex)
        {
            // The service clears the tokens before rethrowing, so the app is signed out regardless.
            await PublishAsync(signedIn: false, ct, fromTokens: true);
            LogException("Sign-out threw", ex);
        }
    }

    /// <summary>What the app can see of the token cache, for the TOKENS card in the UI.</summary>
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

    /// <summary>Called by the API page after a call, so the TOKENS card reflects a refresh or a sign-out the handler caused.</summary>
    public Task SyncStateAsync(CancellationToken ct = default) => PublishAsync(IsSignedIn, ct, fromTokens: true);

    /// <summary>
    /// <c>PrepareLoginStartUri</c>: adds a fresh <c>state</c> to the login URL. The backend
    /// echoes it on the redirect, and <see cref="CheckState"/> refuses tokens that come back
    /// without it. On desktop this is what ties the callback to this sign-in: the loopback
    /// listener accepts the first request to its path from any local page or process.
    /// </summary>
    private string AddState(string? loginStartUri)
    {
        _pendingState = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));

        var separator = loginStartUri?.Contains('?') == true ? '&' : '?';
        var prepared = $"{loginStartUri}{separator}{StateParameter}={_pendingState}";

        Log.Info("PrepareLoginStartUri: state added", prepared);
        return prepared;
    }

    /// <summary>
    /// <c>PostLogin</c>: accepts the tokens the provider extracted only when the callback URL
    /// carries the <c>state</c> this sign-in sent. Returning <c>null</c> fails the sign-in.
    /// </summary>
    private IDictionary<string, string>? CheckState(string callbackUrl, IDictionary<string, string> extracted)
    {
        var expected = Interlocked.Exchange(ref _pendingState, null);
        var returned = ArgumentOf(callbackUrl, StateParameter);

        if (expected is null || returned != expected)
        {
            Log.Error(
                "PostLogin: state mismatch, tokens refused",
                $"""
                Sent     {expected ?? "(no sign-in in flight)"}
                Returned {returned ?? "(none)"}

                The callback did not come from the sign-in this app started.
                """);
            return null;
        }

        Log.Info(
            "PostLogin: state matches",
            $"Tokens in the callback: {string.Join(", ", extracted.Keys)}");
        return extracted;
    }

    /// <summary>
    /// <c>Refresh</c>: trades the stored refresh token for new tokens through the backend's API.
    /// <c>null</c> ends the session (the service clears the cache and raises
    /// <c>LoggedOut</c>). <see cref="IAuthApi.RefreshAsync"/> carries the no-refresh header, so a
    /// 401 from it cannot loop back into another refresh.
    /// </summary>
    private async ValueTask<IDictionary<string, string>?> RefreshTokensAsync(IAuthApi api, IDictionary<string, string> stored, CancellationToken ct)
    {
        if (!stored.TryGetValue(TokenCacheExtensions.RefreshTokenKey, out var refreshToken) || string.IsNullOrEmpty(refreshToken))
        {
            Log.Warning("Refresh: no refresh token stored", "Ending the session.");
            return null;
        }

        try
        {
            var response = await api.RefreshAsync(new RefreshRequest(refreshToken), ct);
            Log.Info("Refresh: POST /auth/refresh returned new tokens", $"The access token lives {response.ExpiresIn} s; the refresh token rotated.");

            return new Dictionary<string, string>
            {
                [TokenCacheExtensions.AccessTokenKey] = response.AccessToken,
                [TokenCacheExtensions.RefreshTokenKey] = response.RefreshToken,
            };
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            Log.Warning("Refresh: POST /auth/refresh answered 401", "The backend no longer knows this refresh token (spent, or the server restarted). Ending the session.");
            return null;
        }
    }

    private static string InteractiveUiDescription => PlatformSupport.Current switch
    {
        AppPlatform.Android => "Chrome custom tab, returning through the intent filter for the callback's scheme",
        AppPlatform.AppleUIKit => "ASWebAuthenticationSession, returning through the scheme declared in Info.plist",
        AppPlatform.WebAssembly => "a popup opened by Uno's WebAuthenticationBroker, returning to the origin's /authentication-callback",
        AppPlatform.Desktop => "the default browser, returning to the loopback listener of the broker AddWeb registers",
        _ => "the platform's web authentication broker"
    };

    private void OnLoggedOut(object? sender, EventArgs e)
    {
        IsSignedIn = false;
        AccessToken = null;
        StateChanged?.Invoke(this, EventArgs.Empty);

        Log.Info(
            "IAuthenticationService.LoggedOut raised",
            """
            The tokens were cleared. This fires on Sign out, on a refresh the backend rejected,
            and on an API 401 that a refresh could not fix - one handler, for the life of the
            app, is where a real app navigates back to its login page.
            """);
    }

    private bool EnsureConfigured()
    {
        if (IsConfigured)
        {
            return true;
        }

        Log.Error(
            "No login page configured",
            $"""
            The WebAuthentication section has no absolute LoginStartUri
            (currently: {(string.IsNullOrEmpty(LoginStartUri) ? "(empty)" : LoginStartUri)}).

            Put your backend's login URL, with its redirect_uri, in appsettings.development.json
            under WebAuthentication. The Platform setup page walks through it.
            """);

        return false;
    }

    private async Task PublishAsync(bool signedIn, CancellationToken ct, bool fromTokens = false)
    {
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

        IsSignedIn = fromTokens ? AccessToken is not null : signedIn;

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void LogException(string title, Exception exception)
    {
        Log.Error(
            title,
            $"{exception.GetType().Name}{Environment.NewLine}{exception.Message}{Environment.NewLine}{Environment.NewLine}{Troubleshoot(exception)}");
    }

    /// <summary>Maps the failures this sample runs into most often onto the fix.</summary>
    private string Troubleshoot(Exception exception) => exception switch
    {
        InvalidOperationException { Message: var m } when m.Contains("scheme", StringComparison.OrdinalIgnoreCase) =>
            $"Uno's WebAuthenticationBroker could not match the callback's scheme to one the app "
            + $"declares. Declare the scheme of {CallbackUri} where this head reads it - see the "
            + "Platform setup page.",

        ArgumentException { ParamName: "callbackUri" } =>
            "The desktop loopback broker only accepts a loopback HTTP callback "
            + "(http://localhost:<port>/...). Check the redirect_uri of LoginStartUri on this head.",

        NotImplementedException =>
            "Uno's built-in WebAuthenticationBroker has no implementation on this head. On Skia "
            + "desktop, AddWeb registers a loopback broker since Uno.Extensions 8.0.0-dev.135: "
            + "either this build runs an older Uno.Extensions, or something used "
            + "WebAuthenticationBroker before UseAuthentication ran and it kept the stub.",

        HttpRequestException =>
            $"The backend could not be reached at {ApiUrl}. Start the server project "
            + "(Authentication.WebExtensionsDemo.Server) and check the address this head uses.",

        ApiException { StatusCode: var status } =>
            $"The backend answered {(int)status} {status}.",

        _ => "See the Platform setup page for what this head needs."
    };

    private static string? RedirectUriOf(string? uri) => ArgumentOf(uri, "redirect_uri");

    /// <summary>A query or fragment argument of <paramref name="uri"/>, decoded.</summary>
    private static string? ArgumentOf(string? uri, string name)
    {
        if (string.IsNullOrEmpty(uri))
        {
            return null;
        }

        var start = uri.IndexOfAny(['?', '#']);
        var arguments = start < 0 ? uri : uri[(start + 1)..];

        foreach (var pair in arguments.Split(['&', '#', '?'], StringSplitOptions.RemoveEmptyEntries))
        {
            var separator = pair.IndexOf('=');
            if (separator > 0 && pair[..separator] == name)
            {
                return Uri.UnescapeDataString(pair[(separator + 1)..]);
            }
        }

        return null;
    }
}
