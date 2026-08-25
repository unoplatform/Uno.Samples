namespace Authentication.WebExtensionsDemo.AuthFlow;

public static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.Desktop;

    private static partial string GetPlatformName() =>
        OperatingSystem.IsWindows() ? "Desktop (Windows, Skia)"
        : OperatingSystem.IsMacOS() ? "Desktop (macOS, Skia)"
        : "Desktop (Linux, Skia)";

    private static partial string GetSignInSurface() =>
        "the system browser, returning to the http://localhost loopback listener that "
        + "Uno.Extensions registers as the desktop WebAuthenticationBroker";
}
