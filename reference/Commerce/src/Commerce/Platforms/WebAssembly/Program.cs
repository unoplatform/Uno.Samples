using Uno.UI.Hosting;

namespace Commerce;

public class Program
{
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
