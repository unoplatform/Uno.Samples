using Uno.UI.Hosting;

namespace VTrack;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {

        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            .UseX11(h => h.PreloadMediaPlayer(true))
            .UseLinuxFrameBuffer()
            .UseMacOS()
            .UseWin32(h => h.PreloadMediaPlayer(true))
            .Build();

        host.Run();
    }
}
