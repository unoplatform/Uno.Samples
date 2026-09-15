using Uno.UI.Hosting;
using Uno.UI.Runtime.Skia;

namespace WindowingSamples;
public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
#if (!useDependencyInjection && useLoggingFallback)
        App.InitializeLogging();

#endif
        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            .UseX11()
            .UseLinuxFrameBuffer()
            .UseMacOS()
            .UseWin32()
            .Build();

        host.Run();
    }
}
