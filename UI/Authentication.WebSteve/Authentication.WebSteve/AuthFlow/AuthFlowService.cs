using Microsoft.Extensions.Configuration;
using Windows.Security.Authentication.Web;
using ITokenCache = Uno.Extensions.Authentication.ITokenCache;
using TokenCacheExtensions = Uno.Extensions.Authentication.TokenCacheExtensions;

namespace Authentication.WebSteve.AuthFlow;

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
    /// <summary>The Web provider's default name - also the configuration section AddWeb binds.</summary>
    public const string ProviderName = "Web";

    private readonly IAuthenticationService _auth;
    private readonly ITokenCache _tokens;
    private readonly IConfiguration _configuration;

    public AuthFlowService(
        IAuthenticationService auth,
        ITokenCache tokens,
        IConfiguration configuration)
    {
        _auth = auth;
        _tokens = tokens;
        _configuration = configuration;
    }

    /// <summary>Step-by-step narration of the flow, bound to the UI on both pages.</summary>
    public AuthFlowLog Log { get; } = new();

    /// <summary>Whether the last authentication call left the app signed in.</summary>
    public bool IsSignedIn { get; private set; }

    /// <summary>Raised whenever the sign-in state changes.</summary>
    public event EventHandler? StateChanged;

    public string LoginStartUri => _configuration[$"{ProviderName}:LoginStartUri"] ?? "(not configured)";

    /// <summary>
    /// The effective callback: configured value when present, otherwise the platform default the
    /// provider derives from WebAuthenticationBroker - which is also what fills the
    /// {RedirectUri} placeholder in LoginStartUri.
    /// </summary>
    public string LoginCallbackUri
    {
        get
        {
            if (_configuration[$"{ProviderName}:LoginCallbackUri"] is { Length: > 0 } configured)
            {
                return configured;
            }

            try
            {
                return $"{WebAuthenticationBroker.GetCurrentApplicationCallbackUri().OriginalString} (platform default)";
            }
            catch (Exception ex)
            {
                return $"(unavailable: {ex.GetType().Name})";
            }
        }
    }

    public string TokenKeys =>
        $"{_configuration[$"{ProviderName}:AccessTokenKey"] ?? "access_token"} / {_configuration[$"{ProviderName}:RefreshTokenKey"] ?? "refresh_token"}";

    /// <summary>Whether the Web section still holds the template's placeholder endpoints.</summary>
    public bool IsConfigured =>
        !LoginStartUri.Contains("YOUR-IDENTITY-PROVIDER") && LoginStartUri != "(not configured)";

    /// <summary>
    /// What the app runs once at launch, before the first page shows: a narrated silent refresh.
    /// </summary>
    public Task<bool> StartupAsync(CancellationToken ct = default)
    {
        Log.Info(
            $"Ready on {PlatformSupport.PlatformName}",
            $"""
            Login page    {LoginStartUri}
            Callback      {LoginCallbackUri}
            Token keys    {TokenKeys}

            The Web provider opens the login page in the platform's browser surface and reads
            the tokens off the redirect's query (or fragment) using the configured keys.
            """);

        if (!IsConfigured)
        {
            Log.Warning(
                "No identity provider configured",
                """
                The Web section of appsettings.json still points at the placeholder endpoints.
                Put your identity provider's login page in LoginStartUri (it must redirect back
                to LoginCallbackUri with the tokens), then sign in. For an endpoint that speaks
                OpenID Connect, prefer the Oidc provider - see Authentication.OidcExtensionsDemo.
                """);
        }

        return RefreshAsync(ct);
    }

    /// <summary>
    /// The silent path a real app should run at startup: with no Refresh callback configured,
    /// the Web provider re-serves the tokens already stored.
    /// </summary>
    public async Task<bool> RefreshAsync(CancellationToken ct = default)
    {
        Log.Call(
            "IAuthenticationService.RefreshAsync()",
            "Silent only - with no Refresh callback configured, the Web provider re-serves the "
            + "tokens already in ITokenCache. False comes back when there are none.");

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
                    "No tokens in ITokenCache, so interaction is required. This is normal on first run.");
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
    /// The interactive flow: the provider opens the configured login page and parses the tokens
    /// off the redirect - one provider call, no callbacks.
    /// </summary>
    public async Task<bool> SignInAsync(IDispatcher? dispatcher, CancellationToken ct = default)
    {
        Log.Call(
            "IAuthenticationService.LoginAsync(dispatcher)",
            $"""
            The provider opens LoginStartUri in {PlatformSupport.SignInSurface}, waits for the
            redirect to LoginCallbackUri, and stores the tokens found on it under the configured
            keys ({TokenKeys}).
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
                    "LoginAsync returned false - the browser flow was dismissed, or the redirect "
                    + "carried no tokens under the configured keys.");
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
    /// Full sign-out: the provider opens LogoutStartUri - the identity provider's end-session
    /// endpoint - in the same browser surface the sign-in used, so the cookie that keeps the
    /// provider's own session alive is in scope and gets cleared. The cached tokens go with it.
    /// </summary>
    /// <remarks>
    /// The local cache is cleared whatever the round trip does. Getting control back depends on
    /// the provider honouring post_logout_redirect_uri, which it only does for a redirect the
    /// client has registered; when it does not, the browser stops on "you are now logged out" and
    /// the user dismisses the sheet. The provider reports that as a failed logout - but the
    /// session is gone by then, and tokens for a dead session are worse than no tokens at all.
    /// </remarks>
    public async Task<bool> SignOutEverywhereAsync(IDispatcher? dispatcher, CancellationToken ct = default)
    {
        Log.Call(
            "IAuthenticationService.LogoutAsync(dispatcher)",
            $"""
            The provider opens LogoutStartUri in {PlatformSupport.SignInSurface}, with
            id_token_hint naming the session to end and post_logout_redirect_uri pointing back at
            the app. This is what signs the browser out - a local clear leaves the provider's
            session alone, which is why the next sign-in then completes without a prompt.
            """);

        var loggedOut = false;

        try
        {
            loggedOut = await _auth.LogoutAsync(dispatcher, ct);

            if (loggedOut)
            {
                Log.Success("Signed out", "The end-session round trip completed and the token cache was cleared.");
            }
            else
            {
                Log.Warning(
                    "Sign-out did not round-trip",
                    "The end-session page did not redirect back - the browser session is signed "
                    + "out, but the provider could not confirm it. Clearing the tokens anyway.");
            }
        }
        catch (OperationCanceledException)
        {
            Log.Warning(
                "Sign-out dismissed",
                "The browser was closed before the end-session redirect returned. Clearing the "
                + "tokens anyway.");
        }
        catch (Exception ex)
        {
            LogException("Sign-out failed", ex);
        }
        finally
        {
            if (!loggedOut)
            {
                // CancellationToken.None deliberately: an already-cancelled token would make this
                // cleanup the no-op it exists to prevent.
                await _tokens.ClearAsync(CancellationToken.None);
            }

            Publish(false);
        }

        return loggedOut;
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
