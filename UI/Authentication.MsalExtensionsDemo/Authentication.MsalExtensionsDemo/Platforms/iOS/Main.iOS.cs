using UIKit;
using Uno.UI.Hosting;

namespace Authentication.MsalExtensionsDemo.iOS;

public class EntryPoint
{
    // This is the main entry point of the application.
    public static void Main(string[] args)
    {
        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            // MsalAppDelegate overrides OpenUrl so the msauth.{BundleId}:// sign-in callback
            // reaches MSAL. See Platforms/iOS/MsalAppDelegate.iOS.cs.
            .UseAppleUIKit(builder => builder.UseUIApplicationDelegate<MsalAppDelegate>())
            .Build();

        host.Run();
    }
}
