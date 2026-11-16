namespace Authentication.WebExtensionsDemo.Authentication;

/// <summary>
/// Desktop (Skia) specifics. Since Uno.Extensions 8.0.0-dev.135, <c>AddWeb</c> registers a
/// loopback <c>WebAuthenticationBroker</c> on Windows, macOS and Linux
/// (<c>DesktopWebAuthenticationBrokerProvider</c>): it opens the login page in the system browser
/// and receives the redirect on an HTTP listener bound to the callback URI. The only thing this
/// head has to do differently is use a loopback HTTP callback instead of the mobile heads'
/// custom scheme.
/// </summary>
/// <remarks>
/// Nothing may touch <c>WebAuthenticationBroker</c> before <c>UseAuthentication</c> has run: the
/// registration is first-wins, and the broker keeps whichever provider it first resolved.
/// </remarks>
public static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.Desktop;

    private static partial IHostBuilder ConfigureHostCore(IHostBuilder host) =>
        UseBackend(host, PlatformGuide.LocalServer, PlatformGuide.DesktopCallbackUri);

    private static partial string GetPlatformName() =>
        $"Desktop (Skia) on {(OperatingSystem.IsMacOS() ? "macOS" : OperatingSystem.IsWindows() ? "Windows" : "Linux")}";

    private static partial string GetCallbackSource() =>
        """
        Overridden for this head in Platforms/Desktop/PlatformSupport.Desktop.cs. AddWeb registers
        a loopback WebAuthenticationBroker here (Uno.Extensions 8.0.0-dev.135 and later): it opens
        the default browser on the login page and listens on the callback above, so the callback
        has to be a loopback HTTP address. The port is fixed so the backend can allow it once.
        The listener accepts the first request to that path from any local page, which is why
        the app sends a state and checks it in PostLogin.
        """;
}
