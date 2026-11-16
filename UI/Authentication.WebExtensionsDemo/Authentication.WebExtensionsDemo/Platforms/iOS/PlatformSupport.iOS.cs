namespace Authentication.WebExtensionsDemo.Authentication;

/// <summary>
/// iOS specifics. The callback must use a custom scheme that is declared under
/// <c>CFBundleURLTypes</c> in <c>Info.plist</c>; the broker refuses to start the sign-in
/// otherwise. appsettings is written for this head: the simulator reaches the development
/// machine at localhost.
/// </summary>
public static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.AppleUIKit;

    /// <summary>Nothing to add: appsettings already names localhost and the custom scheme.</summary>
    private static partial IHostBuilder ConfigureHostCore(IHostBuilder host) => host;

    private static partial string GetPlatformName() =>
        $"{UIKit.UIDevice.CurrentDevice.SystemName} {UIKit.UIDevice.CurrentDevice.SystemVersion}";

    private static partial string GetCallbackSource() =>
        $"""
        From appsettings. Sign-in runs in an ASWebAuthenticationSession, which hands the redirect
        back to the app by its scheme. Uno's WebAuthenticationBroker reads the schemes declared
        under CFBundleURLTypes in Platforms/iOS/Info.plist and refuses to start when the
        callback's scheme is not one of them. No app-delegate or scene-delegate code is involved.

        Bundle ID: {BundleId}
        """;

    private static string BundleId =>
        Foundation.NSBundle.MainBundle.BundleIdentifier ?? PlatformGuide.ApplicationId;
}
