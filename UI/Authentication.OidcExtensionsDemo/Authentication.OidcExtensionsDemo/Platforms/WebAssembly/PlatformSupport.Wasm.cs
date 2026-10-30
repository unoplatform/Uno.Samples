namespace Authentication.OidcExtensionsDemo.Authentication;

/// <summary>
/// WebAssembly specifics. The OIDC provider ignores the configured redirect URI here and always
/// uses Uno's <c>WebAuthenticationBroker</c> callback, which is the app's own origin plus
/// <c>/authentication-callback</c>. It is resolved at runtime because it has to be on the origin
/// the app is actually served from.
/// </summary>
public static partial class PlatformSupport
{
    private static string? _redirectUri;

    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.WebAssembly;

    /// <summary>Nothing to add: the provider already switches to the broker's callback here.</summary>
    private static partial IHostBuilder ConfigureHostCore(IHostBuilder host) => host;

    private static partial string GetPlatformName() => "WebAssembly";

    private static partial string GetEffectiveRedirectUri(string? configured) => _redirectUri ??= ReadRedirectUri();

    private static partial string GetRedirectUriSource() =>
        """
        Provider override: on WebAssembly the OIDC provider replaces the configured RedirectUri
        and PostLogoutRedirectUri with WebAuthenticationBroker.GetCurrentApplicationCallbackUri(),
        the current origin plus /authentication-callback. Sign-in opens in a popup whose URL Uno
        polls until it lands on that path, so the host must serve index.html for it (the
        navigationFallback rewrite in wwwroot/staticwebapp.config.json) and must not block
        popups or set Cross-Origin-Opener-Policy.
        """;

    private static string ReadRedirectUri()
    {
        try
        {
            // Exactly what the provider assigns to OidcClientOptions.RedirectUri: OriginalString,
            // untrimmed, so what is shown is what is sent.
            return Windows.Security.Authentication.Web.WebAuthenticationBroker
                .GetCurrentApplicationCallbackUri()
                .OriginalString;
        }
        catch (Exception)
        {
            // Should not happen in a browser, but the UI must never fail because of a diagnostic.
            return "http://localhost:5000/authentication-callback";
        }
    }
}
