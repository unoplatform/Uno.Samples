namespace Authentication.CustomExtensionsDemo.Authentication;

/// <summary>
/// iOS specifics: none in code; the simulator reaches the Mac at localhost, which appsettings
/// already names. Info.plist allows plain HTTP to it, and Entitlements.plist grants the keychain
/// the token store needs.
/// </summary>
public static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.AppleUIKit;

    private static partial IHostBuilder ConfigureHostCore(IHostBuilder host) => host;

    private static partial string GetPlatformName() =>
        $"{UIKit.UIDevice.CurrentDevice.SystemName} {UIKit.UIDevice.CurrentDevice.SystemVersion}";

    private static partial string GetServerSource() =>
        "From appsettings: the simulator reaches the Mac at localhost. Info.plist sets NSAllowsLocalNetworking so App Transport Security allows plain HTTP to it.";
}
