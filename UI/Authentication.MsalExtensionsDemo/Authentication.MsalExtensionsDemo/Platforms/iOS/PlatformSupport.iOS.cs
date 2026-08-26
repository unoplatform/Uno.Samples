namespace Authentication.MsalExtensionsDemo.Authentication;

/// <summary>
/// iOS specifics. The MSAL provider derives the redirect URI from the bundle identifier, which
/// must also be declared as a URL scheme in <c>Info.plist</c>.
/// </summary>
public static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.AppleUIKit;

    private static partial string GetPlatformName() =>
        $"{UIKit.UIDevice.CurrentDevice.SystemName} {UIKit.UIDevice.CurrentDevice.SystemVersion}";

    /// <summary>
    /// <c>msauth.{BundleId}://auth</c> - the form MSAL requires on Apple platforms. Read from the
    /// bundle at runtime so it is always the value the running app actually reports.
    /// </summary>
    private static partial string GetRedirectUri(string? clientId) => $"msauth.{BundleId}://auth";

    private static partial string GetRedirectUriSource() =>
        $"""
        Provider default: built from the bundle identifier at runtime.

        Bundle ID: {BundleId}

        The same identifier must appear as a CFBundleURLSchemes entry named msauth.{BundleId}
        in Platforms/iOS/Info.plist, and it comes from <ApplicationId> in
        Authentication.MsalExtensionsDemo.csproj.
        """;

    private static string BundleId =>
        Foundation.NSBundle.MainBundle.BundleIdentifier ?? PlatformGuide.ApplicationId;
}
