using Microsoft.Extensions.Configuration;

namespace Authentication.CustomExtensionsDemo.Authentication;

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
/// there are no <c>#if</c> blocks here.
/// </para>
/// <para>
/// The Custom provider involves no browser and no redirect, so the authentication code is the
/// same on every head. The one value that differs is where the demo server is reachable from;
/// the heads that differ from appsettings override it through configuration
/// (<see cref="UseServer"/>).
/// </para>
/// </remarks>
public static partial class PlatformSupport
{
    /// <summary>Which head is running.</summary>
    public static AppPlatform Current => GetCurrentPlatform();

    /// <summary>Display name of the running head, for example "Android".</summary>
    public static string PlatformName => GetPlatformName();

    /// <summary>Where this head's server address comes from, shown next to it in the UI.</summary>
    public static string ServerSource => GetServerSource();

    /// <summary>The head's own additions to the host: its server address when it differs from appsettings.</summary>
    public static IHostBuilder ConfigureHost(IHostBuilder host) => ConfigureHostCore(host);

    private static partial AppPlatform GetCurrentPlatform();

    private static partial IHostBuilder ConfigureHostCore(IHostBuilder host);

    private static partial string GetPlatformName();

    private static partial string GetServerSource();

    /// <summary>
    /// Points the Kiota client at <paramref name="server"/>. Configuration rather than code, so
    /// appsettings stays the one place a reader looks; the in-memory source is added after the
    /// embedded appsettings, so it wins.
    /// </summary>
    internal static IHostBuilder UseServer(IHostBuilder host, string server) => host
        .ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            [$"{CustomFlowService.ClientName}:Url"] = server,
        }));
}
