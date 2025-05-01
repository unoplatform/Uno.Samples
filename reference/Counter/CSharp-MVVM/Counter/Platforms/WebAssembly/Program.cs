using Uno.UI.Runtime.Skia.WebAssembly.Browser;

namespace Counter;

public class Program
{
    private static App? _app;

    public static async Task<int> Main(string[] args)
    {
        App.InitializeLogging();

        var host = new WebAssemblyBrowserHost(() => _app = new App());
        await host.Run();

        return 0;
    }
}
