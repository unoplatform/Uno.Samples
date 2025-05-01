using Uno.UI.Runtime.Skia.WebAssembly.Browser;

namespace TubePlayer;

public class Program
{
    private static App? _app;

    public static async Task<int> Main(string[] args)
    {
        var host = new WebAssemblyBrowserHost(() => _app = new App());
        await host.Run();

        return 0;
    }
}
