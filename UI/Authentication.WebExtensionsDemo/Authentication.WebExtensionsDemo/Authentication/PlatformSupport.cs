using Microsoft.Extensions.Configuration;

namespace Authentication.WebExtensionsDemo.Authentication;

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
/// new head without describing its callback becomes a compile error rather than a runtime
/// surprise.
/// </para>
/// <para>
/// The Web provider sends whatever <c>LoginStartUri</c> says, on every head; it has no
/// per-platform redirect of its own. What differs per head is the address the demo server is
/// reachable at and the callback the browser can return to, so the heads that differ from
/// appsettings override the URIs through configuration (<see cref="UseBackend"/>).
/// </para>
/// </remarks>
public static partial class PlatformSupport
{
    /// <summary>Which head is running.</summary>
    public static AppPlatform Current => GetCurrentPlatform();

    /// <summary>Display name of the running head, for example "Android".</summary>
    public static string PlatformName => GetPlatformName();

    /// <summary>
    /// How the callback comes back into the app on this platform (custom scheme, broker popup,
    /// loopback listener), shown next to the URI in the UI.
    /// </summary>
    public static string CallbackSource => GetCallbackSource();

    /// <summary>
    /// The head's own additions to the host: its demo server address and callback when they
    /// differ from appsettings.
    /// </summary>
    public static IHostBuilder ConfigureHost(IHostBuilder host) => ConfigureHostCore(host);

    private static partial AppPlatform GetCurrentPlatform();

    private static partial IHostBuilder ConfigureHostCore(IHostBuilder host);

    private static partial string GetPlatformName();

    private static partial string GetCallbackSource();

    /// <summary>
    /// Points the Web provider and both API clients at <paramref name="server"/>, with
    /// <paramref name="callback"/> as the <c>redirect_uri</c> of the login and logout pages.
    /// Configuration rather than code, so appsettings stays the one place a reader looks; the
    /// in-memory source is added after the embedded appsettings, so it wins.
    /// </summary>
    internal static IHostBuilder UseBackend(IHostBuilder host, string server, string callback) => host
        .ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            [$"{WebFlowService.ProviderName}:LoginStartUri"] = $"{server}/auth/login?redirect_uri={callback}",
            [$"{WebFlowService.ProviderName}:LogoutStartUri"] = $"{server}/auth/logout?redirect_uri={callback}",
            ["AuthApi:Url"] = server,
            ["ProfileApi:Url"] = server,
        }));
}
