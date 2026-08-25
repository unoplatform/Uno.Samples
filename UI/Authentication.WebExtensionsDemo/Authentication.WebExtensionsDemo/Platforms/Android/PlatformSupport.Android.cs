namespace Authentication.WebExtensionsDemo.AuthFlow;

public static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.Android;

    private static partial string GetPlatformName() => "Android";

    private static partial string GetSignInSurface() =>
        "a Chrome custom tab, returning through the custom-scheme intent filter declared on "
        + "WebAuthenticationBrokerActivity (web-ext-demo://)";
}
