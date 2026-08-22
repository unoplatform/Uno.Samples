namespace Authentication.OidcExtensionsDemo.Authentication;

public static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.WebAssembly;

    private static partial string GetPlatformName() => "WebAssembly";

    private static partial string GetSignInSurface() =>
        "a browser popup, returning to the app's own origin (make sure popups are allowed)";
}
