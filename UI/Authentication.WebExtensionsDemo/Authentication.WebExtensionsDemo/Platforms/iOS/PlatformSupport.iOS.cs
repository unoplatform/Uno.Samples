namespace Authentication.WebExtensionsDemo.AuthFlow;

public static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.AppleUIKit;

    private static partial string GetPlatformName() =>
        OperatingSystem.IsMacCatalyst() ? "Mac Catalyst" : "iOS";

    private static partial string GetSignInSurface() =>
        "ASWebAuthenticationSession, returning through the web-ext-demo:// URL scheme declared "
        + "in Info.plist";
}
