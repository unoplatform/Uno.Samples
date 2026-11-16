using Android.OS;

namespace Authentication.WebExtensionsDemo.Authentication;

/// <summary>
/// Android specifics. The callback must use a custom scheme, and an activity deriving from
/// <c>WebAuthenticationBrokerActivityBase</c> must declare an intent filter for that scheme - see
/// <c>WebAuthenticationBrokerActivity.Android.cs</c>. The emulator reaches the development
/// machine at 10.0.2.2, not localhost.
/// </summary>
public static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.Android;

    private static partial IHostBuilder ConfigureHostCore(IHostBuilder host) =>
        UseBackend(host, PlatformGuide.AndroidEmulatorServer, PlatformGuide.MobileCallbackUri);

    private static partial string GetPlatformName() =>
        $"Android {Build.VERSION.Release} (API {(int)Build.VERSION.SdkInt})";

    private static partial string GetCallbackSource() =>
        $"""
        The system browser (a Chrome custom tab) opens the login page and the backend redirects
        to this URI; Android routes it to the activity in
        Platforms/Android/WebAuthenticationBrokerActivity.Android.cs whose intent filter declares
        the same scheme. The server address is overridden for this head: the emulator reaches the
        development machine at 10.0.2.2.

        Package name: {PackageName}
        """;

    private static string PackageName =>
        Android.App.Application.Context.PackageName ?? PlatformGuide.ApplicationId;
}
