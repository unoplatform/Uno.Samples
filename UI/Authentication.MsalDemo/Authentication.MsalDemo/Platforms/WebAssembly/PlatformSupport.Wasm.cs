using Uno.Foundation;

namespace Authentication.MsalDemo.Authentication;

/// <summary>
/// WebAssembly specifics. The redirect URI has to be on the app's own origin, so it is resolved
/// from <c>window.location</c> at runtime rather than hard-coded.
/// </summary>
internal static partial class PlatformSupport
{
    private static string? _origin;

    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.WebAssembly;

    private static partial string GetPlatformName() => "WebAssembly";

    /// <summary>
    /// The browser cannot hand the authorization code to a page on another origin, so the redirect
    /// URI is the current origin plus a static callback page that exists purely to be landed on.
    /// </summary>
    private static partial string GetRedirectUri() => $"{Origin}{MsalConfig.WasmRedirectPath}";

    private static partial string GetRedirectUriSource() =>
        $"""
        Resolved from window.location at runtime.

        Origin: {Origin}

        Register this exact string as a Single-page application redirect URI. Protocol, host and
        port must match; only the path is free. Serving the app on a different port makes it a
        different origin and sign-in will fail with a redirect URI mismatch.
        """;

    /// <summary>The scheme, host and port the app is being served from, without a trailing slash.</summary>
    private static string Origin => _origin ??= ReadOrigin();

    private static string ReadOrigin()
    {
        try
        {
            return WebAssemblyRuntime.InvokeJS("window.location.origin").TrimEnd('/');
        }
        catch (Exception)
        {
            // Should not happen in a browser, but the UI must never fail because of a diagnostic.
            return "http://localhost:5000";
        }
    }
}
