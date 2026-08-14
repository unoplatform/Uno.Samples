using UIKit;
using Uno.UI.Hosting;

namespace Authentication.MsalExtensionsDemo.iOS;

public class EntryPoint
{
    public static void Main(string[] args)
    {
        App.InitializeLogging();

        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            .UseAppleUIKit(builder => builder.UseUIApplicationDelegate<MsalAppDelegate>())
            .Build();

        host.Run();
    }
}
