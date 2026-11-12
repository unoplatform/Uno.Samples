using System.Runtime.InteropServices.JavaScript;

namespace Authentication.CustomExtensionsDemo.Authentication;

/// <summary>
/// WebAssembly specifics: the demo server hosts this head, so the server is the page's own
/// origin, and no CORS is needed.
/// </summary>
public static partial class PlatformSupport
{
    private static partial AppPlatform GetCurrentPlatform() => AppPlatform.WebAssembly;

    private static partial IHostBuilder ConfigureHostCore(IHostBuilder host) =>
        // Always true on this head; the check is what tells the platform analyzer so.
        OperatingSystem.IsBrowser()
            ? UseServer(host, JSHost.GlobalThis.GetPropertyAsJSObject("location")!.GetPropertyAsString("origin")!)
            : host;

    private static partial string GetPlatformName() => "WebAssembly";

    private static partial string GetServerSource() =>
        "Overridden for this head: the page's own origin, since the demo server hosts the app.";
}
