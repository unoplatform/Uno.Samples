namespace Authentication.MsalExtensionsDemo.Authentication;

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
/// The values shown here mirror what <c>Uno.Extensions.Authentication.MSAL</c> derives internally
/// when no <c>RedirectUri</c> is supplied in configuration. The redirect URI is the single most
/// common cause of MSAL failures, which is why the app shows the value on screen: paste exactly
/// that into the app registration.
/// </para>
/// </remarks>
public static partial class PlatformSupport
{
    /// <summary>Which head is running.</summary>
    public static AppPlatform Current => GetCurrentPlatform();

    /// <summary>Display name of the running head, for example "Android".</summary>
    public static string PlatformName => GetPlatformName();

    /// <summary>
    /// The redirect URI the MSAL provider derives on this platform when configuration does not
    /// supply one. This exact string must be registered on the app registration.
    /// </summary>
    /// <param name="clientId">
    /// The configured client ID; only Android derives its redirect URI from it.
    /// </param>
    public static string RedirectUri(string? clientId) => GetRedirectUri(clientId);

    /// <summary>
    /// Where the value of <see cref="RedirectUri"/> comes from on this platform (provider
    /// default, package name, bundle ID, browser origin, ...), shown next to it in the UI.
    /// </summary>
    public static string RedirectUriSource => GetRedirectUriSource();

    private static partial AppPlatform GetCurrentPlatform();

    private static partial string GetPlatformName();

    private static partial string GetRedirectUri(string? clientId);

    private static partial string GetRedirectUriSource();
}
