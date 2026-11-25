using Authentication.CustomExtensionsDemo.Client;
using Authentication.CustomExtensionsDemo.Client.Models;
using Authentication.CustomExtensionsDemo.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Kiota.Abstractions;
using Uno.Extensions.Authentication.Custom;
using ITokenCache = Uno.Extensions.Authentication.ITokenCache;
using TokenCacheExtensions = Uno.Extensions.Authentication.TokenCacheExtensions;

namespace Authentication.CustomExtensionsDemo.Authentication;

/// <summary>
/// Wraps Uno.Extensions' <see cref="IAuthenticationService"/>, supplies the Custom provider's
/// callbacks, and narrates every step into <see cref="Log"/> so the whole flow is visible in the
/// app.
/// </summary>
/// <remarks>
/// <para>
/// The Custom provider (<c>AddCustom</c>) has no protocol of its own: the app's <c>Login</c>,
/// <c>Refresh</c> and <c>Logout</c> callbacks (<see cref="Configure"/>) call the backend, and the
/// provider stores what <c>Login</c> and <c>Refresh</c> return in <see cref="ITokenCache"/>,
/// applies the access token to HTTP calls and raises <c>LoggedOut</c> when the tokens are cleared.
/// Whatever <c>Login</c> returns is stored as-is, which is why it returns a new dictionary holding
/// only the tokens, never the credentials it was given.
/// </para>
/// <para>
/// Registered as a singleton in <c>App.xaml.cs</c> so the log and sign-in state survive
/// switching between the app's sections, and so there is exactly one <c>LoggedOut</c>
/// subscription for the life of the app.
/// </para>
/// </remarks>
public sealed class CustomFlowService
{
    /// <summary>The name passed to <c>auth.AddCustom(name: ...)</c>; only matters with several providers.</summary>
    public const string ProviderName = "CustomAuth";

    /// <summary>The <c>name</c> passed to <c>AddKiotaClient</c>: also the configuration section of the client.</summary>
    public const string ClientName = "DemoServer";

    /// <summary>The credential keys the login page passes to <c>LoginAsync</c>.</summary>
    public const string UsernameKey = "Username";
    public const string PasswordKey = "Password";

    /// <summary>
    /// The header that keeps a call out of the token-refresh loop. Refit takes
    /// <c>Uno.Extensions.Authentication.Headers.NoRefresh</c> ("No-Refresh:true") in a
    /// <c>[Headers]</c> attribute; a Kiota request adds it as a name and a value.
    /// </summary>
    private const string NoRefreshHeader = "No-Refresh";

    private readonly IAuthenticationService _auth;
    private readonly ITokenCache _tokens;
    private readonly IConfiguration _configuration;

    public CustomFlowService(
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
    /// The three callbacks. <c>AddCustom&lt;DemoServerClient&gt;</c> resolves the Kiota client
    /// registered in <c>UseHttp</c> and hands it to each one.
    /// </summary>
    public static void Configure(ICustomAuthenticationBuilder<DemoServerClient> custom) => custom
        .Login((client, services, dispatcher, tokens, credentials, ct) =>
            services.GetRequiredService<CustomFlowService>().LoginCallbackAsync(client, credentials, ct))
        .Refresh((client, services, tokens, stored, ct) =>
            services.GetRequiredService<CustomFlowService>().RefreshCallbackAsync(client, stored, ct))
        .Logout((client, services, dispatcher, tokens, stored, ct) =>
            services.GetRequiredService<CustomFlowService>().LogoutCallbackAsync(client, stored, ct));

    /// <summary>Step-by-step narration of the flow, bound to the UI.</summary>
    public AuthFlowLog Log { get; }

    /// <summary>Whether the last authentication call left the app signed in.</summary>
    public bool IsSignedIn { get; private set; }

    /// <summary>The access token currently held in <see cref="ITokenCache"/>, if any.</summary>
    public string? AccessToken { get; private set; }

    /// <summary>Raised whenever the sign-in state changes.</summary>
    public event EventHandler? StateChanged;

    /// <summary>The demo server, from the <c>DemoServer</c> section.</summary>
    public string? ApiUrl => _configuration[$"{ClientName}:Url"];

    /// <summary>Whether the client has an address to call.</summary>
    public bool IsConfigured => Uri.TryCreate(ApiUrl, UriKind.Absolute, out _);

    /// <summary>
    /// What the app runs once at launch, before the first page shows: a narrated silent refresh.
    /// </summary>
    public Task<bool> StartupAsync(CancellationToken ct = default)
    {
        Log.Info(
            $"Ready on {PlatformSupport.PlatformName}",
            $"""
            Demo server {ApiUrl}

            At startup the app runs the silent path first, exactly like a production app: renew
            the stored tokens when there are any, and only ask the user when that fails.
            """);

        return RefreshAsync(ct);
    }

    /// <summary>
    /// The silent path a real app should run at startup: trade the stored refresh token for new
    /// tokens, and never show UI.
    /// </summary>
    public async Task<bool> RefreshAsync(CancellationToken ct = default)
    {
        Log.Call(
            "IAuthenticationService.RefreshAsync()",
            """
            Silent only. With tokens stored, the provider runs the Refresh callback (POST
            /auth/refresh with the refresh token); with none, it returns false without calling
            anything. A refresh that yields no tokens clears the cache, which raises LoggedOut.
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
                    No tokens are stored, or the server rejected the refresh token, so the user
                    has to sign in. This is normal on first run, after Sign out, and after the
                    demo server restarts (it keeps its tokens in memory).
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
    /// The interactive flow: the credentials go to the <c>Login</c> callback, which posts them to
    /// the server and returns the tokens to store.
    /// </summary>
    public async Task<bool> SignInAsync(IDispatcher dispatcher, string username, string password, CancellationToken ct = default)
    {
        if (!EnsureConfigured())
        {
            return false;
        }

        Log.Call(
            "IAuthenticationService.LoginAsync(dispatcher, credentials)",
            $$"""
            credentials = { "{{UsernameKey}}": "{{username}}", "{{PasswordKey}}": "(hidden)" }

            The provider hands them to the Login callback and stores whatever it returns.
            """);

        try
        {
            var success = await _auth.LoginAsync(
                dispatcher,
                new Dictionary<string, string> { [UsernameKey] = username, [PasswordKey] = password },
                cancellationToken: ct);

            if (success)
            {
                await PublishAsync(signedIn: true, ct);
                Log.Success("Sign-in succeeded", await DescribeTokensAsync(ct));
            }
            else
            {
                await PublishAsync(signedIn: false, ct);
                Log.Warning("Sign-in failed", "LoginAsync returned false: the Login callback returned null.");
            }

            return success;
        }
        catch (Exception ex)
        {
            await PublishAsync(signedIn: false, ct, fromTokens: true);
            LogException("Sign-in threw", ex);
            return false;
        }
    }

    /// <summary>Revokes the refresh token at the server and clears <see cref="ITokenCache"/>.</summary>
    public async Task SignOutAsync(IDispatcher dispatcher, CancellationToken ct = default)
    {
        Log.Call(
            "IAuthenticationService.LogoutAsync(dispatcher)",
            """
            The provider runs the Logout callback (POST /auth/logout, which revokes the refresh
            token); when it returns true, the tokens are cleared, which raises LoggedOut.
            """);

        try
        {
            var loggedOut = await _auth.LogoutAsync(dispatcher, ct);
            await PublishAsync(signedIn: false, ct, fromTokens: true);

            if (loggedOut)
            {
                Log.Success("Signed out", "The server revoked the refresh token and the tokens were cleared.");
            }
            else
            {
                Log.Warning("Sign-out cancelled", "The Logout callback returned false, so the tokens were kept.");
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
    /// <c>Login</c>: posts the credentials, and returns a new dictionary holding only the tokens.
    /// Returning the <paramref name="credentials"/> dictionary (what the template does) would
    /// store the password. A 401 is a wrong password, so <c>null</c>; anything else propagates
    /// and makes <c>LoginAsync</c> throw.
    /// </summary>
    private async ValueTask<IDictionary<string, string>?> LoginCallbackAsync(DemoServerClient client, IDictionary<string, string> credentials, CancellationToken ct)
    {
        var request = new LoginRequest
        {
            UserName = credentials.TryGetValue(UsernameKey, out var username) ? username : "",
            Password = credentials.TryGetValue(PasswordKey, out var password) ? password : "",
        };

        try
        {
            var response = await client.Auth.Login.PostAsync(request, cancellationToken: ct);
            Log.Info("Login: POST /auth/login returned tokens", $"The access token lives {response?.ExpiresIn} s. Only the tokens are returned to the provider.");
            return Tokens(response);
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == 401)
        {
            Log.Warning("Login: POST /auth/login answered 401", "Wrong user name or password. Returning null fails the sign-in.");
            return null;
        }
    }

    /// <summary>
    /// <c>Refresh</c>: trades the stored refresh token for new tokens. <c>null</c> ends the session
    /// (the service clears the cache and raises <c>LoggedOut</c>). The request carries the
    /// no-refresh header, so a 401 from it cannot loop back into another refresh. A network
    /// failure propagates: <c>RefreshAsync</c> throws and the tokens are kept.
    /// </summary>
    private async ValueTask<IDictionary<string, string>?> RefreshCallbackAsync(DemoServerClient client, IDictionary<string, string> stored, CancellationToken ct)
    {
        if (!stored.TryGetValue(TokenCacheExtensions.RefreshTokenKey, out var refreshToken) || string.IsNullOrEmpty(refreshToken))
        {
            Log.Warning("Refresh: no refresh token stored", "Ending the session.");
            return null;
        }

        try
        {
            var response = await client.Auth.Refresh.PostAsync(
                new RefreshRequest { RefreshToken = refreshToken },
                config => config.Headers.Add(NoRefreshHeader, "true"),
                ct);
            Log.Info("Refresh: POST /auth/refresh returned new tokens", $"The access token lives {response?.ExpiresIn} s; the refresh token rotated.");
            return Tokens(response);
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == 401)
        {
            Log.Warning("Refresh: POST /auth/refresh answered 401", "The server no longer knows this refresh token (revoked, spent, or the server restarted). Ending the session.");
            return null;
        }
    }

    /// <summary>
    /// <c>Logout</c>: revokes the refresh token at the server. Returning <c>true</c> lets the
    /// provider clear the tokens; <c>false</c> would cancel the sign-out and keep them.
    /// </summary>
    private async ValueTask<bool> LogoutCallbackAsync(DemoServerClient client, IDictionary<string, string> stored, CancellationToken ct)
    {
        if (stored.TryGetValue(TokenCacheExtensions.RefreshTokenKey, out var refreshToken) && !string.IsNullOrEmpty(refreshToken))
        {
            await client.Auth.Logout.PostAsync(
                new RefreshRequest { RefreshToken = refreshToken },
                config => config.Headers.Add(NoRefreshHeader, "true"),
                ct);
            Log.Info("Logout: POST /auth/logout revoked the refresh token");
        }

        return true;
    }

    /// <summary>The dictionary the provider stores: the tokens and nothing else.</summary>
    private static IDictionary<string, string>? Tokens(TokenResponse? response) =>
        response is { AccessToken: { Length: > 0 } accessToken, RefreshToken: { Length: > 0 } refreshToken }
            ? new Dictionary<string, string>
            {
                [TokenCacheExtensions.AccessTokenKey] = accessToken,
                [TokenCacheExtensions.RefreshTokenKey] = refreshToken,
            }
            : null;

    private void OnLoggedOut(object? sender, EventArgs e)
    {
        IsSignedIn = false;
        AccessToken = null;
        StateChanged?.Invoke(this, EventArgs.Empty);

        Log.Info(
            "IAuthenticationService.LoggedOut raised",
            """
            The tokens were cleared. This fires on Sign out, on a refresh the server rejected,
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
            "No server configured",
            $"""
            The {ClientName} section has no absolute Url (currently: {(string.IsNullOrEmpty(ApiUrl) ? "(empty)" : ApiUrl)}).
            Put the demo server's address in appsettings.development.json.
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
        HttpRequestException =>
            $"The server could not be reached at {ApiUrl}. Start the server project "
            + "(Authentication.CustomExtensionsDemo.Server) and check the address this head uses.",

        ApiException { ResponseStatusCode: var status } =>
            $"The server answered {status}.",

        _ => "See the Platform setup page for what this head needs."
    };
}
