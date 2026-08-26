using Android.OS;

namespace Authentication.MsalExtensionsDemo.Authentication;

/// <summary>
/// Android specifics. The MSAL provider derives the redirect URI <c>msal{ClientId}://auth</c>
/// from the configured client ID, and an intent filter for that scheme has to exist - see
/// <c>MsalActivity.Android.cs</c>.
/// </summary>
public static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.Android;

    private static partial string GetPlatformName() =>
        $"Android {Build.VERSION.Release} (API {(int)Build.VERSION.SdkInt})";

    private static partial string GetRedirectUri(string? clientId) =>
        $"msal{(string.IsNullOrEmpty(clientId) ? "{ClientId}" : clientId)}://auth";

    private static partial string GetRedirectUriSource() =>
        $"""
        Provider default: derived from the ClientId in the MsalAuthentication configuration
        section. The intent filter in MsalActivity.Android.cs must declare the same
        msal-plus-client-ID scheme - intent filters are compile-time attributes, so it cannot
        follow appsettings.json automatically.

        Package name: {PackageName}
        """;

    /// <summary>
    /// The application ID Android reports at runtime. It is not part of the redirect URI in this
    /// (browser-based) flow, but it is what the portal's Android platform option would ask for.
    /// </summary>
    private static string PackageName =>
        Android.App.Application.Context.PackageName ?? PlatformGuide.ApplicationId;
}
