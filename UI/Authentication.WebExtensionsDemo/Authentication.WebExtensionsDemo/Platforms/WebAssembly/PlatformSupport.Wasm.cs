namespace Authentication.WebExtensionsDemo.Authentication;

/// <summary>
/// WebAssembly specifics. Unlike the OIDC provider, the Web provider does not switch to the
/// broker's callback by itself: it sends the configured <c>redirect_uri</c>, so this head sets it
/// to <c>WebAuthenticationBroker.GetCurrentApplicationCallbackUri()</c>, the app's own origin plus
/// <c>/authentication-callback</c>. The demo server hosts this head, so its origin is also the
/// backend's address.
/// </summary>
public static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.WebAssembly;

    private static partial IHostBuilder ConfigureHostCore(IHostBuilder host)
    {
        // Safe this early on WebAssembly: the first-use rule only matters on desktop, where
        // AddWeb has to register its broker before anything resolves one.
        var callback = Windows.Security.Authentication.Web.WebAuthenticationBroker.GetCurrentApplicationCallbackUri();
        return UseBackend(host, callback.GetLeftPart(UriPartial.Authority), callback.OriginalString);
    }

    private static partial string GetPlatformName() => "WebAssembly";

    private static partial string GetCallbackSource() =>
        """
        Overridden for this head in Platforms/WebAssembly/PlatformSupport.Wasm.cs with
        WebAuthenticationBroker.GetCurrentApplicationCallbackUri(): the current origin plus
        /authentication-callback. Sign-in opens in a popup whose URL Uno polls until it lands on
        that path, so the host must serve index.html for it (the demo server's fallback route)
        and must not block popups or set Cross-Origin-Opener-Policy.
        """;
}
