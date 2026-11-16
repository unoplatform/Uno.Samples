namespace Authentication.MsalDemo.Authentication;

/// <summary>
/// The platform heads this sample covers.
/// </summary>
internal enum AppPlatform
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
/// new head without telling MSAL what its redirect URI is becomes a compile error rather than a
/// runtime surprise.
/// </para>
/// <para>
/// The redirect URI is the single most common cause of MSAL failures, which is why the app shows
/// the value it computed on screen: paste exactly that into the app registration.
/// </para>
/// </remarks>
internal static partial class PlatformSupport
{
    /// <summary>Which head is running.</summary>
    public static AppPlatform Current => GetCurrentPlatform();

    /// <summary>Display name of the running head, for example "Android".</summary>
    public static string PlatformName => GetPlatformName();

    /// <summary>
    /// The redirect URI MSAL is configured with on this platform. This exact string must be
    /// registered on the app registration.
    /// </summary>
    public static string RedirectUri => GetRedirectUri();

    /// <summary>
    /// Where the value of <see cref="RedirectUri"/> comes from on this platform (package name,
    /// bundle ID, browser origin, ...), shown next to it in the UI.
    /// </summary>
    public static string RedirectUriSource => GetRedirectUriSource();

    private static partial AppPlatform GetCurrentPlatform();

    private static partial string GetPlatformName();

    private static partial string GetRedirectUri();

    private static partial string GetRedirectUriSource();
}
