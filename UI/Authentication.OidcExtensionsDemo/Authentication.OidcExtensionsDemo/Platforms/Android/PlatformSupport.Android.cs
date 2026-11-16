using Android.OS;

namespace Authentication.OidcExtensionsDemo.Authentication;

/// <summary>
/// Android specifics. The configured redirect URI must use a custom scheme, and an activity
/// deriving from <c>WebAuthenticationBrokerActivityBase</c> must declare an intent filter for
/// that scheme - see <c>WebAuthenticationBrokerActivity.Android.cs</c>.
/// </summary>
public static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.Android;

    /// <summary>Nothing to add: Uno's WebAuthenticationBroker handles the custom-scheme redirect.</summary>
    private static partial IHostBuilder ConfigureHostCore(IHostBuilder host) => host;

    private static partial string GetPlatformName() =>
        $"Android {Build.VERSION.Release} (API {(int)Build.VERSION.SdkInt})";

    private static partial string GetEffectiveRedirectUri(string? configured) => ConfiguredOrPlaceholder(configured);

    private static partial string GetRedirectUriSource() =>
        $"""
        From configuration. The system browser (a Chrome custom tab) opens the sign-in page and
        the identity provider redirects to this URI; Android routes it to the activity in
        Platforms/Android/WebAuthenticationBrokerActivity.Android.cs whose intent filter declares
        the same scheme. Intent filters are compile-time attributes, so that scheme cannot follow
        appsettings automatically.

        Package name: {PackageName}
        """;

    private static string PackageName =>
        Android.App.Application.Context.PackageName ?? PlatformGuide.ApplicationId;
}
