using Uno.UI.Hosting;

namespace QuoteCraft;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
#if DEBUG
            // Verification runs under Xvfb: QC_X11_RENDERER=software draws with Skia's CPU backend instead of Mesa's
            // llvmpipe OpenGL, which left the window unpainted beyond its first width after a live resize.
            .UseX11(x11 =>
            {
                if (Environment.GetEnvironmentVariable("QC_X11_RENDERER") == "software")
                {
                    x11.ForceRenderingBackend(X11RenderingBackend.Software);
                }
            })
#else
            .UseX11()
#endif
            .UseLinuxFrameBuffer()
            .UseMacOS()
            .UseWin32()
            .Build();

        host.Run();
    }
}
