using System.Buffers.Text;
using System.Text.Json;
using Authentication.OidcExtensionsDemo.Common;
using Microsoft.Extensions.Configuration;
using ITokenCache = Uno.Extensions.Authentication.ITokenCache;
using TokenCacheExtensions = Uno.Extensions.Authentication.TokenCacheExtensions;

namespace Authentication.OidcExtensionsDemo.Authentication;

/// <summary>
/// Wraps Uno.Extensions' <see cref="IAuthenticationService"/> and narrates every step it takes
/// into <see cref="Log"/> so the whole flow is visible in the app.
/// </summary>
/// <remarks>
/// <para>
/// This is the only type in the sample that talks to the authentication stack, and it uses no
/// OIDC library type directly: the <c>Uno.Extensions.Authentication.Oidc</c> provider owns the
/// <c>OidcClient</c>, builds it from the <c>OidcAuthentication</c> configuration section,
/// opens the identity provider's page through Uno's <c>WebAuthenticationBroker</c> (on the Skia
/// desktop head, the loopback broker the provider registers), stores the tokens and refreshes
/// them. The app only sees
/// three calls (<c>LoginAsync</c>, <c>RefreshAsync</c>, <c>LogoutAsync</c>), the
/// <c>LoggedOut</c> event, and the tokens in <see cref="ITokenCache"/>.
/// </para>
/// <para>
/// Registered as a singleton in <c>App.xaml.cs</c> so the log and sign-in state survive
/// switching between the app's sections, and so there is exactly one <c>LoggedOut</c>
/// subscription for the life of the app.
/// </para>
/// </remarks>
public sealed class OidcFlowService
{
    /// <summary>The name passed to <c>auth.AddOidc(name: ...)</c> - also the configuration section.</summary>
    public const string ProviderName = "OidcAuthentication";

    private readonly IAuthenticationService _auth;
    private readonly ITokenCache _tokens;
    private readonly IConfiguration _configuration;

    public OidcFlowService(
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

    /// <summary>Step-by-step narration of the flow, bound to the UI.</summary>
    public AuthFlowLog Log { get; }

    /// <summary>Whether the last authentication call left the app signed in.</summary>
    public bool IsSignedIn { get; private set; }

    /// <summary>The access token currently held in <see cref="ITokenCache"/>, if any.</summary>
    public string? AccessToken { get; private set; }

    /// <summary>Raised whenever the sign-in state changes.</summary>
    public event EventHandler? StateChanged;

    public string? Authority => _configuration[$"{ProviderName}:Authority"];

    public string? ClientId => _configuration[$"{ProviderName}:ClientId"];

    public string? Scope => _configuration[$"{ProviderName}:Scope"];

    /// <summary>The <c>RedirectUri</c> as written in configuration; see <see cref="RedirectUri"/> for what is sent.</summary>
    public string? ConfiguredRedirectUri => _configuration[$"{ProviderName}:RedirectUri"];

    /// <summary>Whether the section names an authority and a client.</summary>
    public bool IsConfigured =>
        Uri.TryCreate(Authority, UriKind.Absolute, out _) && !string.IsNullOrWhiteSpace(ClientId);

    /// <summary>The redirect URI the provider sends on this platform.</summary>
    public string RedirectUri => PlatformSupport.EffectiveRedirectUri(ConfiguredRedirectUri);

    /// <summary>
    /// What the app runs once at launch, before the first page shows: a narrated silent refresh.
    /// </summary>
    public Task<bool> StartupAsync(CancellationToken ct = default)
    {
        Log.Info(
            $"Ready on {PlatformSupport.PlatformName}",
            $"""
            Redirect URI in use: {RedirectUri}

            At startup the app runs the silent path first, exactly like a production app: renew
            the stored tokens when there are any, and only ask the user when that fails.
            """);

        return RefreshAsync(ct);
    }

    /// <summary>
    /// The silent path a real app should run at startup: use the stored refresh token to get a
    /// new access token, and never show UI.
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
            Silent only. The provider sends the stored refresh token to the token endpoint; false
            comes back when there is none, or when the identity provider rejects it. A rejected
            refresh clears the tokens, which raises LoggedOut.
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
                    No refresh token is stored, so interaction is required. This is normal on
                    first run, after Sign out, and when the Scope did not include offline_access:
                    without that scope the identity provider issues no refresh token and the
                    session ends with the app.
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
    /// The interactive flow: the provider opens the identity provider's sign-in page and
    /// exchanges the returned authorization code (with PKCE) for tokens.
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
            The provider builds the authorization request (code flow with PKCE), opens the
            identity provider's page and waits for the redirect below, then exchanges the code
            for tokens at the token endpoint. Credentials are never passed: the identity
            provider collects them.

            Redirect URI {RedirectUri}
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
                    LoginAsync returned false without throwing. The provider logs the OidcClient
                    error at Error level: a cancelled browser page, a redirect URI the identity
                    provider rejected, or a browser that could not deliver the callback to the
                    app (see the Platform setup page for this head).
                    """);
            }

            return success;
        }
        catch (Exception ex)
        {
            LogException("Sign-in failed", ex);
            return false;
        }
    }

    /// <summary>
    /// Ends the session at the identity provider and clears <see cref="ITokenCache"/>.
    /// </summary>
    public async Task SignOutAsync(IDispatcher dispatcher, CancellationToken ct = default)
    {
        Log.Call(
            "IAuthenticationService.LogoutAsync(dispatcher)",
            """
            The provider opens the identity provider's end-session endpoint in the browser, waits
            for the post-logout redirect, then clears the tokens - which raises LoggedOut.
            """);

        try
        {
            var loggedOut = await _auth.LogoutAsync(dispatcher, ct);
            await PublishAsync(signedIn: false, ct);

            if (loggedOut)
            {
                Log.Success("Signed out", "The identity provider ended its session and the tokens were cleared.");
            }
            else
            {
                Log.Warning("Sign-out did not complete", "LogoutAsync returned false; the tokens were kept.");
            }
        }
        catch (Exception ex)
        {
            // Google, for example, has no end_session_endpoint: OidcClient throws, and the
            // service clears the tokens before rethrowing, so the app is signed out regardless.
            await PublishAsync(signedIn: false, ct);
            LogException("Sign-out threw", ex);
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

    /// <summary>
    /// The payload of the ID token held in <see cref="ITokenCache"/> as indented JSON - the
    /// signed-in user's claims - or <c>null</c> when none is cached. Decoded, not validated: the
    /// app may show these, only the API may trust them.
    /// </summary>
    public async Task<string?> DescribeClaimsAsync(CancellationToken ct = default)
    {
        try
        {
            var idToken = await _tokens.TokenAsync(TokenCacheExtensions.IdTokenKey, ct);
            // TokenAsync yields null (despite its signature) when the key is absent, e.g. after sign-out.
            if (idToken?.Split('.') is not [_, var payload, _])
            {
                return null;
            }

            using var claims = JsonDocument.Parse(Base64Url.DecodeFromChars(payload));
            return JsonSerializer.Serialize(claims.RootElement, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ID token could not be decoded: {ex.GetType().Name}";
        }
    }

    /// <summary>Called by the API page after a call, so the TOKEN card reflects a refresh or a sign-out the handler caused.</summary>
    public Task SyncStateAsync(CancellationToken ct = default) => PublishAsync(IsSignedIn, ct, fromTokens: true);

    private static string InteractiveUiDescription => PlatformSupport.Current switch
    {
        AppPlatform.Android => "Chrome custom tab, returning through the intent filter for the redirect URI's scheme",
        AppPlatform.AppleUIKit => "ASWebAuthenticationSession, returning through the scheme declared in Info.plist",
        AppPlatform.WebAssembly => "a popup opened by Uno's WebAuthenticationBroker, returning to the origin's /authentication-callback",
        AppPlatform.Desktop => "the default browser, returning to the loopback listener of the broker the provider registers",
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
            The tokens were cleared. This fires on Sign out, on a silent refresh the identity
            provider rejected, and on an API 401 that a refresh could not fix - one handler, for
            the life of the app, is where a real app navigates back to its login page.
            """);
    }

    private bool EnsureConfigured()
    {
        if (IsConfigured)
        {
            return true;
        }

        Log.Error(
            "No identity provider configured",
            $"""
            The OidcAuthentication section does not contain an Authority and a ClientId
            (currently: {(string.IsNullOrEmpty(Authority) ? "(empty)" : Authority)} / {(string.IsNullOrEmpty(ClientId) ? "(empty)" : ClientId)}).

            Put the values your identity provider gave you in appsettings.development.json
            under OidcAuthentication, and register this platform's redirect URI there:
            {RedirectUri}

            The Platform setup page walks through it.
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

    /// <summary>
    /// Maps the failures this sample runs into most often onto the fix. The provider rethrows
    /// OidcClient and broker exceptions untouched.
    /// </summary>
    private string Troubleshoot(Exception exception) => exception switch
    {
        InvalidOperationException { Message: var m } when m.Contains("end session", StringComparison.OrdinalIgnoreCase) =>
            "The identity provider's discovery document has no end_session_endpoint (Google, for "
            + "example). The tokens are cleared before the exception reaches the app, so the user "
            + "is signed out locally; catch this in the sign-out command.",

        InvalidOperationException { Message: var m } when m.Contains("scheme", StringComparison.OrdinalIgnoreCase) =>
            $"Uno's WebAuthenticationBroker could not match the redirect URI's scheme to one the app "
            + $"declares. Declare the scheme of {RedirectUri} where this head reads it - see the "
            + "Platform setup page.",

        NotImplementedException =>
            "Uno's built-in WebAuthenticationBroker has no implementation on this head. On Skia "
            + "desktop, Uno.Extensions 8.0.0-dev.135 and later register a loopback broker from "
            + "AddOidc: either this build runs an older Uno.Extensions, or something used "
            + "WebAuthenticationBroker before UseAuthentication ran and it kept the stub.",

        HttpRequestException =>
            "The identity provider could not be reached. Check the Authority URL and the network; "
            + "the demo server must be reachable from the device.",

        _ when PlatformSupport.Current == AppPlatform.WebAssembly =>
            "Check that the popup was not blocked by the browser and that the identity provider "
            + "accepts the redirect URI above. See the Platform setup page.",

        _ => "See the Platform setup page for what this head needs."
    };
}
