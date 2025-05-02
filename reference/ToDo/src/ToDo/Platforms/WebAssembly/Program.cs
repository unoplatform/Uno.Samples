using Uno.UI.Hosting;

namespace ToDo;

public class Program
{
    private static App? _app;

    public static async Task<int> Main(string[] args)
    {
        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            .UseWebAssembly()
            .Build();

        await host.RunAsync();

        return 0;
    }
}
