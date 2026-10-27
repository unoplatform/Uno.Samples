using Microsoft.Extensions.Configuration;

namespace Authentication.OidcExtensionsDemo.Authentication;

/// <summary>
/// Desktop (Skia) specifics. Since Uno.Extensions 8.0.0-dev.135, <c>AddOidc</c> registers a
/// loopback <c>WebAuthenticationBroker</c> on Windows, macOS and Linux
/// (<c>DesktopWebAuthenticationBrokerProvider</c>): it opens the sign-in page in the system
/// browser and receives the redirect on an HTTP listener bound to the redirect URI. The only
/// thing this head has to do differently is use a loopback HTTP redirect URI instead of the
/// mobile heads' custom scheme.
/// </summary>
public static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.Desktop;

    /// <summary>
    /// One desktop-only addition: the loopback redirect URIs. They are overridden through
    /// configuration rather than through <c>AddOidc(oidc => oidc.RedirectUri(...))</c>, because
    /// code options replace the whole <c>OidcAuthentication</c> section instead of merging with
    /// it, so a code override would also have to repeat the authority, client ID and scope.
    /// </summary>
    /// <remarks>
    /// Nothing registers a browser here. The provider's default <c>IBrowser</c> goes through
    /// Uno's <c>WebAuthenticationBroker</c>, and <c>AddOidc</c> registers the desktop loopback
    /// broker as its implementation. That registration is first-wins, and the broker keeps
    /// whichever provider it first resolved, so nothing in the app may touch
    /// <c>WebAuthenticationBroker</c> before <c>UseAuthentication</c> has run.
    /// </remarks>
    private static partial IHostBuilder ConfigureHostCore(IHostBuilder host) => host
        .ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            [$"{OidcFlowService.ProviderName}:RedirectUri"] = PlatformGuide.DesktopRedirectUri,
            [$"{OidcFlowService.ProviderName}:PostLogoutRedirectUri"] = PlatformGuide.DesktopRedirectUri,
        }));

    private static partial string GetPlatformName() =>
        $"Desktop (Skia) on {(OperatingSystem.IsMacOS() ? "macOS" : OperatingSystem.IsWindows() ? "Windows" : "Linux")}";

    private static partial string GetEffectiveRedirectUri(string? configured) => ConfiguredOrPlaceholder(configured);

    private static partial string GetRedirectUriSource() =>
        """
        From configuration. On the Skia desktop head the OIDC provider registers a loopback
        WebAuthenticationBroker (Uno.Extensions 8.0.0-dev.135 and later): it opens the default
        browser on the sign-in page and listens on the redirect URI above for the authorization
        code, so the URI has to be a loopback HTTP address. The port is fixed by configuration;
        register exactly this URI with the identity provider.
        """;
}
