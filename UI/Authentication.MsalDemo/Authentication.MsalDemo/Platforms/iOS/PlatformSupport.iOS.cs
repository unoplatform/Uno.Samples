namespace Authentication.MsalDemo.Authentication;

/// <summary>
/// iOS and Mac Catalyst specifics. The redirect URI is derived from the bundle identifier, which
/// must also be declared as a URL scheme in <c>Info.plist</c>.
/// </summary>
internal static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.AppleUIKit;

    private static partial string GetPlatformName() =>
        $"{UIKit.UIDevice.CurrentDevice.SystemName} {UIKit.UIDevice.CurrentDevice.SystemVersion}";

    /// <summary>
    /// <c>msauth.{BundleId}://auth</c> - the form MSAL requires on Apple platforms. Read from the
    /// bundle at runtime so it is always the value the running app actually reports.
    /// </summary>
    private static partial string GetRedirectUri() => $"msauth.{BundleId}://auth";

    private static partial string GetRedirectUriSource() =>
        $"""
        Built from the bundle identifier at runtime.

        Bundle ID: {BundleId}

        The same identifier must appear as a CFBundleURLSchemes entry named msauth.{BundleId}
        in Platforms/iOS/Info.plist, and it comes from <ApplicationId> in Authentication.MsalDemo.csproj.
        """;

    private static string BundleId =>
        Foundation.NSBundle.MainBundle.BundleIdentifier ?? PlatformGuide.ApplicationId;
}
