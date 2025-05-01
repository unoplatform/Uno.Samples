using Uno.UI.Runtime.Skia;

namespace TubePlayer;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = SkiaHostBuilder.Create()
            .App(() => new App())
            .UseX11()
            .UseLinuxFrameBuffer()
            .UseMacOS()
            .UseWin32()
            .Build();

        host.Run();
    }
}
