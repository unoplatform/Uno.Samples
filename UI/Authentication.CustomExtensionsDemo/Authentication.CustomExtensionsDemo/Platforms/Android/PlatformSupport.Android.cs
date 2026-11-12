using Android.OS;

namespace Authentication.CustomExtensionsDemo.Authentication;

/// <summary>
/// Android specifics: the emulator reaches the development machine at 10.0.2.2, not localhost,
/// and Resources/xml/network_security_config.xml allows plain HTTP to that address only.
/// </summary>
public static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.Android;

    private static partial IHostBuilder ConfigureHostCore(IHostBuilder host) =>
        UseServer(host, PlatformGuide.AndroidEmulatorServer);

    private static partial string GetPlatformName() =>
        $"Android {Build.VERSION.Release} (API {(int)Build.VERSION.SdkInt})";

    private static partial string GetServerSource() =>
        "Overridden for this head: the emulator reaches the development machine at 10.0.2.2, and network_security_config.xml allows plain HTTP to it.";
}
