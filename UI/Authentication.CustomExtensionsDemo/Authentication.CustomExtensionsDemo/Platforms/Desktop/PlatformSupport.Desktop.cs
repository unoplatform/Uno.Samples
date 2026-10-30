namespace Authentication.CustomExtensionsDemo.Authentication;

/// <summary>Desktop (Skia) specifics: none; appsettings names the server on localhost.</summary>
public static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.Desktop;

    private static partial IHostBuilder ConfigureHostCore(IHostBuilder host) => host;

    private static partial string GetPlatformName() =>
        $"Desktop (Skia) on {(OperatingSystem.IsMacOS() ? "macOS" : OperatingSystem.IsWindows() ? "Windows" : "Linux")}";

    private static partial string GetServerSource() => "From appsettings: the server runs on this machine.";
}
