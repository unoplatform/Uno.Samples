namespace Authentication.OidcSteve.AuthFlow;

/// <summary>
/// The platform heads this sample covers.
/// </summary>
public enum AppPlatform
{
    Android,
    AppleUIKit,
    WebAssembly,
    Desktop
}

/// <summary>
/// Runtime facts about the platform the app is currently running on.
/// </summary>
/// <remarks>
/// Each head implements this in its own <c>Platforms/&lt;platform&gt;/PlatformSupport.*.cs</c> file.
/// Uno's single project only compiles the folder matching the target framework being built, so
/// there are no <c>#if</c> blocks here - and because these are <c>partial</c> methods, adding a
/// new head without describing its sign-in surface becomes a compile error rather than a runtime
/// surprise.
/// </remarks>
public static partial class PlatformSupport
{
    /// <summary>Which head is running.</summary>
    public static AppPlatform Current => GetCurrentPlatform();

    /// <summary>Display name of the running head, for example "Android".</summary>
    public static string PlatformName => GetPlatformName();

    /// <summary>
    /// How the interactive sign-in shows up on this platform, and how the redirect finds its way
    /// back into the app.
    /// </summary>
    public static string SignInSurface => GetSignInSurface();

    private static partial AppPlatform GetCurrentPlatform();

    private static partial string GetPlatformName();

    private static partial string GetSignInSurface();
}
