using Microsoft.Extensions.Configuration;

namespace Authentication.OidcExtensionsDemo.Authentication;

/// <summary>
/// Desktop (Skia) specifics. Uno's <c>WebAuthenticationBroker</c> has no desktop implementation,
/// so this head supplies its own <c>IBrowser</c> to the OIDC provider: it opens the system
/// browser and collects the redirect on a loopback HTTP listener - see
/// <c>Platforms/Desktop/SystemBrowser.cs</c>.
/// </summary>
public static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.Desktop;

    /// <summary>
    /// Two desktop-only additions. The redirect URIs are overridden through configuration
    /// rather than through <c>AddOidc(oidc => oidc.RedirectUri(...))</c>: code options replace
    /// the whole <c>OidcAuthentication</c> section instead of merging with it, so a code
    /// override would also have to repeat the authority, client ID and scope.
    /// </summary>
    private static partial IHostBuilder ConfigureHostCore(IHostBuilder host) => host
        .ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            [$"{OidcFlowService.ProviderName}:RedirectUri"] = PlatformGuide.DesktopRedirectUri,
            [$"{OidcFlowService.ProviderName}:PostLogoutRedirectUri"] = PlatformGuide.DesktopRedirectUri,
        }))
        // The provider resolves IBrowser from the container, so this replaces its default
        // (which needs the WebAuthenticationBroker that Skia desktop does not implement).
        .ConfigureServices(services => services.AddTransient<Duende.IdentityModel.OidcClient.Browser.IBrowser, SystemBrowser>());

    private static partial string GetPlatformName() =>
        $"Desktop (Skia) on {(OperatingSystem.IsMacOS() ? "macOS" : OperatingSystem.IsWindows() ? "Windows" : "Linux")}";

    private static partial string GetEffectiveRedirectUri(string? configured) => ConfiguredOrPlaceholder(configured);

    private static partial string GetRedirectUriSource() =>
        """
        From configuration. On the Skia desktop head Uno's WebAuthenticationBroker is not
        implemented, so the provider's default browser cannot complete a sign-in. This sample
        registers its own IBrowser (Platforms/Desktop/SystemBrowser.cs): it opens the default
        browser and listens on the loopback redirect URI above for the authorization code. The
        port is fixed by configuration, so register exactly this URI with the identity provider.
        """;
}
