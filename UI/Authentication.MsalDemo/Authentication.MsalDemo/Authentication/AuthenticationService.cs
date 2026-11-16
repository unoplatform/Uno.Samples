using Microsoft.Identity.Client;
using Uno.UI.MSAL;

namespace Authentication.MsalDemo.Authentication;

/// <summary>
/// Wraps MSAL.NET's <see cref="IPublicClientApplication"/> and narrates every step it takes into
/// <see cref="Log"/> so the whole flow is visible in the app.
/// </summary>
/// <remarks>
/// <para>
/// This is the only type in the sample that talks to MSAL. Two things need a platform-specific
/// answer - who is the parent of the sign-in UI on mobile, and what shows the web UI in the
/// browser - and <c>.WithUnoHelpers()</c> supplies both. It is the only Uno-specific line in the
/// flow, called once on the builder in <see cref="GetOrCreateApp"/> and once on the interactive
/// request in <see cref="AcquireInteractiveAsync"/>.
/// </para>
/// <para>
/// It is not one implementation: <c>Uno.WinUI.MSAL</c> ships a different <c>Uno.UI.MSAL.dll</c> per
/// runtime flavour and the build deploys the one matching the head, independently of the renderer.
/// What it does per head:
/// </para>
/// <list type="bullet">
/// <item><description>Android - <c>WithParentActivityOrWindow</c> from <c>ContextHelper.Current</c>.
/// This is the parent <c>Activity</c> hosting the sign-in UI; without it
/// <c>AcquireTokenInteractive</c> crashes.</description></item>
/// <item><description>iOS / Mac Catalyst - <c>WithParentActivityOrWindow</c> from the key window's
/// <c>RootViewController</c>.</description></item>
/// <item><description>WebAssembly - an <c>ICustomWebUi</c> that opens a popup with
/// <c>window.open</c> and polls its URL, plus an <c>IMsalHttpClientFactory</c> that hands MSAL a
/// browser-backed <c>HttpClient</c>.</description></item>
/// <item><description>Desktop (Skia) and Windows - nothing, and nothing is needed. MSAL.NET
/// launches the system browser and collects the code on an <c>http://localhost</c> loopback
/// listener.</description></item>
/// </list>
/// <para>
/// A singleton is used rather than dependency injection to keep the sample free of any host
/// builder or Uno.Extensions dependency. In a real app, register this as a singleton service.
/// </para>
/// </remarks>
internal sealed class AuthenticationService
{
    public static AuthenticationService Instance { get; } = new();

    private IPublicClientApplication? _app;

    private AuthenticationService()
    {
    }

    /// <summary>Step-by-step narration of the flow, bound to the UI.</summary>
    public AuthFlowLog Log { get; } = new();

    /// <summary>Result of the last successful token acquisition, if any.</summary>
    public AuthenticationResult? LastResult { get; private set; }

    /// <summary>Whether a token has been acquired in this session.</summary>
    public bool IsSignedIn => LastResult is not null;

    /// <summary>Raised whenever <see cref="LastResult"/> changes.</summary>
    public event EventHandler? StateChanged;

    /// <summary>
    /// Builds the <see cref="IPublicClientApplication"/> on first use.
    /// </summary>
    /// <remarks>
    /// Creation is deferred so an unconfigured app can still start and explain itself instead of
    /// throwing at startup.
    /// </remarks>
    private IPublicClientApplication GetOrCreateApp()
    {
        if (_app is not null)
        {
            return _app;
        }

        Log.Call(
            "PublicClientApplicationBuilder.Create(...)",
            $"""
            Client ID    {MsalConfig.ClientId}
            Authority    https://login.microsoftonline.com/{MsalConfig.Tenant}
            Redirect URI {PlatformSupport.RedirectUri}
            """);

        _app = PublicClientApplicationBuilder
            .Create(MsalConfig.ClientId)
            .WithAuthority(AzureCloudInstance.AzurePublic, MsalConfig.Tenant)
            .WithRedirectUri(PlatformSupport.RedirectUri)
            .WithUnoHelpers()
            .Build();

        Log.Success("IPublicClientApplication ready", $"Running on {PlatformSupport.PlatformName}.");

        return _app;
    }

    /// <summary>
    /// The complete flow a real app should use: try the token cache first, and only prompt the
    /// user when MSAL says interaction is required.
    /// </summary>
    public async Task<AuthenticationResult?> SignInAsync(CancellationToken ct = default)
    {
        if (!EnsureConfigured())
        {
            return null;
        }

        var app = GetOrCreateApp();
        var account = await FindCachedAccountAsync(app);

        if (account is not null)
        {
            var silent = await TryAcquireSilentAsync(app, account, ct);
            if (silent is not null)
            {
                return Publish(silent);
            }
        }

        var interactive = await AcquireInteractiveAsync(app, account, ct);
        return interactive is null ? null : Publish(interactive);
    }

    /// <summary>
    /// Silent-only acquisition. Useful to show what happens on app start, and what MSAL throws
    /// when there is nothing usable in the cache.
    /// </summary>
    public async Task<AuthenticationResult?> SignInSilentlyAsync(CancellationToken ct = default)
    {
        if (!EnsureConfigured())
        {
            return null;
        }

        var app = GetOrCreateApp();
        var account = await FindCachedAccountAsync(app);

        if (account is null)
        {
            Log.Warning(
                "Nothing to do",
                "Silent acquisition needs an account in the cache. Sign in interactively first.");
            return null;
        }

        var result = await TryAcquireSilentAsync(app, account, ct);
        return result is null ? null : Publish(result);
    }

    /// <summary>
    /// Removes every cached account, which is MSAL's local sign-out. It does not sign the user out
    /// of the browser session at the identity provider.
    /// </summary>
    public async Task SignOutAsync()
    {
        if (_app is null)
        {
            Log.Info("Not signed in", "No MSAL application has been created yet.");
            return;
        }

        Log.Call("IPublicClientApplication.GetAccountsAsync()", "Enumerating cached accounts to remove.");
        var accounts = (await _app.GetAccountsAsync()).ToList();

        if (accounts.Count == 0)
        {
            Log.Info("No cached accounts", "Nothing to remove.");
        }

        foreach (var account in accounts)
        {
            Log.Call("IPublicClientApplication.RemoveAsync(account)", account.Username);
            await _app.RemoveAsync(account);
        }

        LastResult = null;
        StateChanged?.Invoke(this, EventArgs.Empty);

        Log.Success(
            "Signed out locally",
            "Tokens were removed from the cache. The identity provider's browser session is untouched, "
            + "so the next interactive sign-in may not ask for credentials again.");
    }

    /// <summary>Accounts currently held in MSAL's token cache.</summary>
    public async Task<IReadOnlyList<IAccount>> GetAccountsAsync()
    {
        if (_app is null || !MsalConfig.IsConfigured)
        {
            return [];
        }

        return (await _app.GetAccountsAsync()).ToList();
    }

    private async Task<IAccount?> FindCachedAccountAsync(IPublicClientApplication app)
    {
        Log.Call(
            "IPublicClientApplication.GetAccountsAsync()",
            "Step 1 - look for an account already in the token cache.");

        var accounts = (await app.GetAccountsAsync()).ToList();

        if (accounts.Count == 0)
        {
            Log.Info(
                "Cache is empty",
                """
                No account found, so a silent call cannot be attempted and interaction is required.
                Note that MSAL's cache is in memory only on WebAssembly and Desktop, so a restart
                always lands here. Android and iOS persist it in platform secure storage.
                """);
            return null;
        }

        var account = accounts[0];
        Log.Success(
            $"Found {accounts.Count} cached account(s)",
            $"Using {account.Username} (environment {account.Environment}).");

        return account;
    }

    private async Task<AuthenticationResult?> TryAcquireSilentAsync(
        IPublicClientApplication app,
        IAccount account,
        CancellationToken ct)
    {
        Log.Call(
            "AcquireTokenSilent(scopes, account).ExecuteAsync()",
            "Step 2 - serve from the cache, refreshing with the refresh token if needed. "
            + "No UI is shown.");

        try
        {
            var result = await app
                .AcquireTokenSilent(MsalConfig.Scopes, account)
                .ExecuteAsync(ct);

            LogResult(result, "Silent acquisition succeeded");
            return result;
        }
        catch (MsalUiRequiredException ex)
        {
            // This is the expected, documented signal to fall back to an interactive call.
            Log.Warning(
                "MsalUiRequiredException - interaction required",
                $"""
                Error code   {ex.ErrorCode}
                Classification {ex.Classification}
                {ex.Message}

                This is normal: it means consent, MFA, a password change or an expired refresh
                token needs the user. Falling back to AcquireTokenInteractive.
                """);
            return null;
        }
        catch (Exception ex)
        {
            LogException("Silent acquisition failed", ex);
            return null;
        }
    }

    private async Task<AuthenticationResult?> AcquireInteractiveAsync(
        IPublicClientApplication app,
        IAccount? account,
        CancellationToken ct)
    {
        Log.Call(
            "AcquireTokenInteractive(scopes).WithUnoHelpers().ExecuteAsync()",
            $"""
            Step 3 - show the sign-in UI. The redirect URI below must be registered on the
            app registration for this platform, or the identity provider rejects the request.

            Redirect URI {PlatformSupport.RedirectUri}
            Web UI       {InteractiveUiDescription}
            """);

        try
        {
            var request = app
                .AcquireTokenInteractive(MsalConfig.Scopes)
                .WithPrompt(Prompt.SelectAccount)
                // Uno Platform: supplies the popup-based ICustomWebUi on WebAssembly, and the
                // parent activity/view controller on Android and iOS - redundantly there, since
                // the builder already set it. A no-op on Desktop and Windows.
                .WithUnoHelpers();

            if (!string.IsNullOrWhiteSpace(account?.Username))
            {
                // Pre-fills the account picker. Only set it when a username is actually known.
                request = request.WithLoginHint(account.Username);
            }

            var result = await request.ExecuteAsync(ct);

            LogResult(result, "Interactive sign-in succeeded");
            return result;
        }
        catch (Exception ex)
        {
            LogException("Interactive sign-in failed", ex);
            return null;
        }
    }

    private static string InteractiveUiDescription => PlatformSupport.Current switch
    {
        AppPlatform.Android => "Chrome custom tab, returning through the msal{ClientId}:// intent filter",
        AppPlatform.AppleUIKit => "ASWebAuthenticationSession, returning through the msauth.{BundleId} URL scheme",
        AppPlatform.WebAssembly => "a browser popup opened by Uno's ICustomWebUi, returning to the "
            + "registered single-page-application redirect URI",
        AppPlatform.Windows => "the system browser, returning to the http://localhost loopback listener",
        _ => "the system browser, returning to the http://localhost loopback listener"
    };

    private bool EnsureConfigured()
    {
        if (MsalConfig.IsConfigured)
        {
            return true;
        }

        Log.Error(
            "No client ID configured",
            $"""
            MsalConfig.ClientId is still {MsalConfig.UnconfiguredClientId}.

            Register an app in the Microsoft Entra admin center, then put its Application
            (client) ID in Authentication/MsalConfig.cs and register this platform's redirect
            URI: {PlatformSupport.RedirectUri}

            MSAL-SETUP.md walks through it.
            """);

        return false;
    }

    private AuthenticationResult Publish(AuthenticationResult result)
    {
        LastResult = result;
        StateChanged?.Invoke(this, EventArgs.Empty);
        return result;
    }

    private void LogResult(AuthenticationResult result, string title)
    {
        var lifetime = result.ExpiresOn - DateTimeOffset.Now;

        Log.Success(
            title,
            $"""
            Account       {result.Account?.Username ?? "(none)"}
            Tenant        {result.TenantId}
            Scopes        {string.Join(", ", result.Scopes)}
            Token source  {result.AuthenticationResultMetadata.TokenSource}
            Expires       {result.ExpiresOn.ToLocalTime():HH:mm:ss} (in {lifetime.TotalMinutes:F0} min)
            Access token  {result.AccessToken.Length} chars
            ID token      {(string.IsNullOrEmpty(result.IdToken) ? "(none)" : $"{result.IdToken.Length} chars")}
            """);
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
    /// exactly the ones that cost time when wiring up a new platform.
    /// </summary>
    private static string Troubleshoot(Exception exception) => exception switch
    {
        MsalException { ErrorCode: "authentication_canceled" } =>
            "The user closed the sign-in UI.",

        MsalException { ErrorCode: "invalid_request" } =>
            $"Usually a redirect URI mismatch. Register exactly: {PlatformSupport.RedirectUri}",

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
            + $"{PlatformSupport.RedirectUri} on {PlatformSupport.PlatformName}.",

        _ when PlatformSupport.Current == AppPlatform.WebAssembly =>
            "Check that the popup was not blocked by the browser, and that the redirect URI above "
            + "is registered under the Single-page application platform - only that type has CORS "
            + "enabled on the token endpoint. See the Platform setup page or MSAL-SETUP.md.",

        _ => "See the Platform setup page for what this head needs."
    };
}
