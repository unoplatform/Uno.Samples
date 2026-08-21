using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using ITokenCache = Uno.Extensions.Authentication.ITokenCache;
using TokenCacheExtensions = Uno.Extensions.Authentication.TokenCacheExtensions;

namespace Authentication.MsalExtensionsDemo.Authentication;

/// <summary>
/// Wraps Uno.Extensions' <see cref="IAuthenticationService"/> and narrates every step it takes
/// into <see cref="Log"/> so the whole flow is visible in the app.
/// </summary>
/// <remarks>
/// <para>
/// This is the only type in the sample that talks to the authentication stack. Unlike the
/// plain-MSAL sample (Authentication.MsalDemo), no MSAL type is used directly: the
/// <c>Uno.Extensions.Authentication.MSAL</c> provider owns the
/// <c>IPublicClientApplication</c> - it builds it from the <c>MsalAuthentication</c>
/// configuration section, derives the platform's redirect URI, applies <c>WithUnoHelpers()</c>
/// and persists the token cache where the platform allows it. The app only sees three calls
/// (<c>LoginAsync</c>, <c>RefreshAsync</c>, <c>LogoutAsync</c>) and the resulting access token
/// in <see cref="ITokenCache"/>.
/// </para>
/// <para>
/// Registered as a singleton in <c>App.xaml.cs</c> so the log and sign-in state survive
/// switching between the app's sections.
/// </para>
/// </remarks>
public sealed class MsalFlowService
{
    /// <summary>The name passed to <c>auth.AddMsal(window, name: ...)</c> - also the configuration section.</summary>
    public const string ProviderName = "MsalAuthentication";

    private readonly IAuthenticationService _auth;
    private readonly ITokenCache _tokens;
    private readonly IConfiguration _configuration;

    public MsalFlowService(IAuthenticationService auth, ITokenCache tokens, IConfiguration configuration)
    {
        _auth = auth;
        _tokens = tokens;
        _configuration = configuration;
    }

    /// <summary>Step-by-step narration of the flow, bound to the UI.</summary>
    public AuthFlowLog Log { get; } = new();

    /// <summary>Whether the last authentication call left the app signed in.</summary>
    public bool IsSignedIn { get; private set; }

    /// <summary>The access token currently held in <see cref="ITokenCache"/>, if any.</summary>
    public string? AccessToken { get; private set; }

    /// <summary>Raised whenever the sign-in state changes.</summary>
    public event EventHandler? StateChanged;

    public string? ClientId => _configuration[$"{ProviderName}:ClientId"];

    public string? TenantId => _configuration[$"{ProviderName}:TenantId"];

    public string[] Scopes => _configuration
        .GetSection($"{ProviderName}:Scopes")
        .GetChildren()
        .Select(child => child.Value)
        .OfType<string>()
        .ToArray();

    /// <summary>Whether the configured client ID looks like a real app registration.</summary>
    public bool IsConfigured => Guid.TryParse(ClientId, out _);

    /// <summary>The redirect URI the provider derives on this platform.</summary>
    public string RedirectUri => PlatformSupport.RedirectUri(ClientId);

    /// <summary>
    /// What the app runs once at launch, before the first page shows: a narrated silent refresh.
    /// </summary>
    public Task<bool> StartupAsync(CancellationToken ct = default)
    {
        Log.Info(
            $"Ready on {PlatformSupport.PlatformName}",
            $"""
            Redirect URI in use: {RedirectUri}

            At startup the app runs the silent path first, exactly like a production app: sign
            back in from the persisted cache when possible, and only ask the user when needed.
            """);

        return RefreshAsync(ct);
    }

    /// <summary>
    /// The silent path a real app should run at startup: serve from the persisted cache,
    /// refreshing if needed, and never show UI.
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
            Silent only - the provider looks for a cached MSAL account and calls
            AcquireTokenSilent. No UI is ever shown; false comes back when interaction would be
            required.
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
                    No usable account in MSAL's cache, so interaction is required. This is normal
                    on first run - and on WebAssembly on every run, since the cache is in memory
                    only there. On desktop the cache persists (DPAPI / keychain / keyring), so a
                    restart after a sign-in lands on the success path instead.
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
    /// The complete flow: the provider tries the token cache first and only prompts the user when
    /// MSAL says interaction is required - the same silent-then-interactive sequence the
    /// plain-MSAL sample spells out by hand, here performed inside one call.
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
            The provider runs AcquireTokenSilent first and falls back to
            AcquireTokenInteractive(...).WithUnoHelpers() only when MSAL reports that interaction
            is required. The redirect URI below is derived by the provider and must be registered
            on the app registration for this platform.

            Redirect URI {RedirectUri}
            Web UI       {InteractiveUiDescription}
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
                Log.Warning("Sign-in did not complete", "LoginAsync returned false without an error.");
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
    /// Removes the cached MSAL account and clears <see cref="ITokenCache"/>. It does not sign the
    /// user out of the browser session at the identity provider.
    /// </summary>
    public async Task SignOutAsync(IDispatcher dispatcher, CancellationToken ct = default)
    {
        Log.Call(
            "IAuthenticationService.LogoutAsync()",
            "The provider removes the cached MSAL account, and the ITokenCache is cleared.");

        try
        {
            await _auth.LogoutAsync(dispatcher, ct);
            await PublishAsync(signedIn: false, ct);

            Log.Success(
                "Signed out locally",
                "Tokens were removed from the cache. The identity provider's browser session is untouched, "
                + "so the next interactive sign-in may not ask for credentials again.");
        }
        catch (Exception ex)
        {
            LogException("Sign-out failed", ex);
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

    private static string InteractiveUiDescription => PlatformSupport.Current switch
    {
        AppPlatform.Android => "Chrome custom tab, returning through the msal{ClientId}:// intent filter",
        AppPlatform.AppleUIKit => "ASWebAuthenticationSession, returning through the msauth.{BundleId} URL scheme",
        AppPlatform.WebAssembly => "a browser popup opened by Uno's ICustomWebUi, returning to the "
            + "registered single-page-application redirect URI",
        _ => "the system browser, returning to the http://localhost loopback listener"
    };

    private bool EnsureConfigured()
    {
        if (IsConfigured)
        {
            return true;
        }

        Log.Error(
            "No client ID configured",
            $"""
            The MsalAuthentication section does not contain a valid ClientId
            (currently: {(string.IsNullOrEmpty(ClientId) ? "(empty)" : ClientId)}).

            Register an app in the Microsoft Entra admin center, then put its Application
            (client) ID in appsettings.development.json under MsalAuthentication, and register
            this platform's redirect URI: {RedirectUri}

            The Platform setup page walks through it.
            """);

        return false;
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
        var detail = exception switch
        {
            MsalServiceException service =>
                $"""
                {service.GetType().Name}
                Error code  {service.ErrorCode}
                HTTP status {service.StatusCode}
                {service.Message}
                """,
            MsalClientException client =>
                $"""
                {client.GetType().Name}
                Error code {client.ErrorCode}
                {client.Message}
                """,
            _ => $"{exception.GetType().Name}{Environment.NewLine}{exception.Message}"
        };

        Log.Error(title, detail + Environment.NewLine + Environment.NewLine + Troubleshoot(exception));
    }

    /// <summary>
    /// Maps the MSAL error codes this sample runs into most often onto the fix, since these are
    /// exactly the ones that cost time when wiring up a new platform. The provider rethrows MSAL
    /// exceptions untouched, so the error codes are the same as with plain MSAL.
    /// </summary>
    private string Troubleshoot(Exception exception) => exception switch
    {
        MsalException { ErrorCode: "authentication_canceled" } =>
            "The user closed the sign-in UI - or, on desktop, the provider's InteractiveTimeout "
            + "elapsed (closing the system browser is undetectable, so an abandoned sign-in is "
            + "cancelled after 5 minutes by default).",

        MsalException { ErrorCode: "invalid_request" } =>
            $"Usually a redirect URI mismatch. Register exactly: {RedirectUri}",

        MsalException { ErrorCode: "unauthorized_client" } =>
            "The client ID does not exist in this tenant, or the app is not a public client. "
            + "Enable 'Allow public client flows' on the registration.",

        MsalException { ErrorCode: "invalid_client" } =>
            "The registration expects a client secret, so it is a confidential client. This sample "
            + "needs a public client registration.",

        MsalException { ErrorCode: "access_denied" } =>
            "The user or an administrator declined consent for the requested scopes.",

        MsalClientException { ErrorCode: "redirect_uri_validation_failed" } =>
            $"MSAL rejected the redirect URI before contacting the service. It must be exactly "
            + $"{RedirectUri} on {PlatformSupport.PlatformName}.",

        _ when PlatformSupport.Current == AppPlatform.WebAssembly =>
            "Check that the popup was not blocked by the browser, and that the redirect URI above "
            + "is registered under the Single-page application platform - only that type has CORS "
            + "enabled on the token endpoint. See the Platform setup page.",

        _ => "See the Platform setup page for what this head needs."
    };
}
