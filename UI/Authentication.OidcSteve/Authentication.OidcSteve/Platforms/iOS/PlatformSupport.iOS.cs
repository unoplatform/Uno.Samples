namespace Authentication.OidcSteve.AuthFlow;

public static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.AppleUIKit;

    private static partial string GetPlatformName() =>
        OperatingSystem.IsMacCatalyst() ? "Mac Catalyst" : "iOS";

    private static partial string GetSignInSurface() =>
        "ASWebAuthenticationSession, returning through the oidc-steve:// URL scheme declared "
        + "in Info.plist";
}
