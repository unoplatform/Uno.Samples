using UIKit;
using Uno.UI.Hosting;

namespace Authentication.WebExtensionsDemo.iOS;

public class EntryPoint
{
    // This is the main entry point of the application.
    public static void Main(string[] args)
    {
        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            // No custom UIApplicationDelegate needed: ASWebAuthenticationSession hands the
            // web-ext-demo:// callback straight back to Uno's WebAuthenticationBroker.
            .UseAppleUIKit()
            .Build();

        host.Run();
    }
}
