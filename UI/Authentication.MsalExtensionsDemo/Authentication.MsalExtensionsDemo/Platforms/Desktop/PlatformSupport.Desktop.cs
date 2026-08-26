namespace Authentication.MsalExtensionsDemo.Authentication;

/// <summary>
/// Desktop (Skia) specifics. There is nothing to wire up here: the MSAL provider calls MSAL.NET's
/// <c>WithDefaultRedirectUri()</c>, and MSAL.NET on .NET uses the default system browser with a
/// loopback listener.
/// </summary>
public static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.Desktop;

    private static partial string GetPlatformName() =>
        $"Desktop (Skia) on {(OperatingSystem.IsMacOS() ? "macOS" : OperatingSystem.IsWindows() ? "Windows" : "Linux")}";

    /// <summary>
    /// The loopback redirect URI. MSAL starts a local HTTP listener on a free port and the browser
    /// posts the authorization code back to it, so no port is specified here - Entra ID accepts any
    /// port for <c>http://localhost</c>.
    /// </summary>
    private static partial string GetRedirectUri(string? clientId) => "http://localhost";

    private static partial string GetRedirectUriSource() =>
        "Provider default: MSAL's WithDefaultRedirectUri(). MSAL picks a free port at runtime; "
        + "Entra ID allows any port on localhost.";
}
