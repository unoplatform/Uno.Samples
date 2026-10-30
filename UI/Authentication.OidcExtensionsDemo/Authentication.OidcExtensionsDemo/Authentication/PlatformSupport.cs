namespace Authentication.OidcExtensionsDemo.Authentication;

/// <summary>
/// The platform heads this sample covers.
/// </summary>
public enum AppPlatform
{
    Android,
    AppleUIKit,
    WebAssembly,
    Desktop,
    Windows
}

/// <summary>
/// Runtime facts about the platform the app is currently running on.
/// </summary>
/// <remarks>
/// <para>
/// Each head implements this in its own <c>Platforms/&lt;platform&gt;/PlatformSupport.*.cs</c> file.
/// Uno's single project only compiles the folder matching the target framework being built, so
/// there are no <c>#if</c> blocks here - and because these are <c>partial</c> methods, adding a
/// new head without describing its redirect URI becomes a compile error rather than a runtime
/// surprise.
/// </para>
/// <para>
/// Unlike MSAL, the OIDC provider does not derive a redirect URI per platform: it sends the
/// <c>RedirectUri</c> from configuration everywhere except WebAssembly, where it always uses the
/// <c>WebAuthenticationBroker</c> callback on the app's own origin. What differs per head is how
/// that URI finds its way back into the app, which is what these files describe.
/// </para>
/// </remarks>
public static partial class PlatformSupport
{
    /// <summary>Which head is running.</summary>
    public static AppPlatform Current => GetCurrentPlatform();

    /// <summary>Display name of the running head, for example "Android".</summary>
    public static string PlatformName => GetPlatformName();

    /// <summary>
    /// The redirect URI the OIDC provider sends on this platform: the configured value, or the
    /// broker's callback on WebAssembly. This exact string must be registered with the identity
    /// provider (the Duende demo server accepts any value for its public demo clients).
    /// </summary>
    /// <param name="configured">The <c>RedirectUri</c> from the <c>OidcAuthentication</c> section.</param>
    public static string EffectiveRedirectUri(string? configured) => GetEffectiveRedirectUri(configured);

    /// <summary>
    /// How the redirect comes back into the app on this platform (custom scheme, broker page,
    /// loopback listener), shown next to the URI in the UI.
    /// </summary>
    public static string RedirectUriSource => GetRedirectUriSource();

    /// <summary>
    /// The head's own additions to the host: nothing on the mobile and WebAssembly heads, where
    /// Uno's <c>WebAuthenticationBroker</c> does the work; on desktop, the loopback redirect URI
    /// the provider's own broker listens on.
    /// </summary>
    public static IHostBuilder ConfigureHost(IHostBuilder host) => ConfigureHostCore(host);

    private static partial AppPlatform GetCurrentPlatform();

    private static partial IHostBuilder ConfigureHostCore(IHostBuilder host);

    private static partial string GetPlatformName();

    private static partial string GetEffectiveRedirectUri(string? configured);

    private static partial string GetRedirectUriSource();

    /// <summary>A placeholder when configuration has no redirect URI yet.</summary>
    internal static string ConfiguredOrPlaceholder(string? configured) =>
        string.IsNullOrWhiteSpace(configured) ? "{RedirectUri from appsettings}" : configured;
}
