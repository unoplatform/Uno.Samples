using UIKit;
using Uno.UI.Hosting;

namespace Authentication.OidcExtensionsDemo.iOS;

public class EntryPoint
{
    // This is the main entry point of the application.
    public static void Main(string[] args)
    {
        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            // No custom UIApplicationDelegate needed: ASWebAuthenticationSession hands the
            // oidc-ext-demo:// callback straight back to Uno's WebAuthenticationBroker.
            .UseAppleUIKit()
            .Build();

        host.Run();
    }
}
