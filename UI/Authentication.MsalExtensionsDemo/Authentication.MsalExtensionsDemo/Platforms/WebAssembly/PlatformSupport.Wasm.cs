namespace Authentication.MsalExtensionsDemo.Authentication;

/// <summary>
/// WebAssembly specifics. The MSAL provider derives the redirect URI from Uno's
/// <c>WebAuthenticationBroker</c>, which is the app's own origin plus
/// <c>/authentication-callback</c>. It is resolved at runtime rather than hard-coded, because it
/// has to be on the origin the app is actually served from.
/// </summary>
public static partial class PlatformSupport
{
    private static string? _redirectUri;

    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.WebAssembly;

    private static partial string GetPlatformName() => "WebAssembly";

    /// <summary>
    /// The same value the provider computes: WebAuthenticationBroker's callback URI on the
    /// current origin.
    /// </summary>
    private static partial string GetRedirectUri(string? clientId) => _redirectUri ??= ReadRedirectUri();

    private static partial string GetRedirectUriSource() =>
        """
        Provider default: Uno's WebAuthenticationBroker callback URI on the current origin.

        Register this exact string as a Single-page application redirect URI. Entra ID ignores
        the port for localhost, but the path must match and the platform type must be SPA -
        only SPA redirect URIs get CORS enabled on the token endpoint.
        """;

    private static string ReadRedirectUri()
    {
        try
        {
            // Exactly what the provider passes to WithRedirectUri (see WithWebRedirectUri):
            // OriginalString, untrimmed. This string is pasted into the app registration, so a
            // one-character difference here would send someone hunting a redirect URI mismatch.
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
