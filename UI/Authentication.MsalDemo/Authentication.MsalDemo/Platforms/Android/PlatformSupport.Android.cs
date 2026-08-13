using Android.OS;

namespace Authentication.MsalDemo.Authentication;

/// <summary>
/// Android specifics. The redirect URI must be exactly <c>msal{ClientId}://auth</c> for MSAL.NET's
/// browser flow, and an intent filter for that scheme has to exist - see
/// <c>MsalActivity.Android.cs</c>.
/// </summary>
internal static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.Android;

    private static partial string GetPlatformName() =>
        $"Android {Build.VERSION.Release} (API {(int)Build.VERSION.SdkInt})";

    private static partial string GetRedirectUri() => $"{MsalConfig.AndroidRedirectScheme}://auth";

    private static partial string GetRedirectUriSource() =>
        $"""
        Built from MsalConfig.ClientId. The same constant declares the intent filter in
        MsalActivity.Android.cs, so the manifest can never drift from the configured client ID.

        Package name: {PackageName}
        """;

    /// <summary>
    /// The application ID Android reports at runtime. It is not part of the redirect URI in this
    /// (browser-based) flow, but it is what the portal's Android platform option would ask for.
    /// </summary>
    private static string PackageName =>
        Android.App.Application.Context.PackageName ?? PlatformGuide.ApplicationId;
}
