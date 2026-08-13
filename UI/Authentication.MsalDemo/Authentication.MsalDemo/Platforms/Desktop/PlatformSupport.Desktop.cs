namespace Authentication.MsalDemo.Authentication;

/// <summary>
/// Desktop (Skia) specifics. There is nothing to wire up here: <c>.WithUnoHelpers()</c> is a no-op
/// in the skia build of <c>Uno.UI.MSAL</c>, and MSAL.NET on .NET uses the default system browser
/// with a loopback listener.
/// </summary>
internal static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.Desktop;

    private static partial string GetPlatformName() =>
        $"Desktop (Skia) on {(OperatingSystem.IsMacOS() ? "macOS" : OperatingSystem.IsWindows() ? "Windows" : "Linux")}";

    /// <summary>
    /// The loopback redirect URI. MSAL starts a local HTTP listener on a free port and the browser
    /// posts the authorization code back to it, so no port is specified here - Entra ID accepts any
    /// port for <c>http://localhost</c>.
    /// </summary>
    private static partial string GetRedirectUri() => "http://localhost";

    private static partial string GetRedirectUriSource() =>
        "Fixed loopback address. MSAL picks a free port at runtime; Entra ID allows any port on localhost.";
}
