using Uno.UI.Runtime.Skia.AppleUIKit;

namespace TubePlayer.iOS;

public class EntryPoint
{
    // This is the main entry point of the application.
    public static void Main(string[] args)
    {
        // if you want to use a different Application Delegate class from "AppDelegate"
        // you can specify it here.
        var host = new AppleUIKitHost(() => new App());
        host.Run();
    }
}
