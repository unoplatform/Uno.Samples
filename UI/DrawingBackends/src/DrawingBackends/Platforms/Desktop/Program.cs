using Uno.UI.Composition.Skia;
using Uno.UI.Composition.WebGpu;
using Uno.UI.Hosting;

namespace DrawingBackends;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        App.InitializeLogging();

        var useWebGpu = ParseBackend(args).Equals("webgpu", StringComparison.OrdinalIgnoreCase);
        BackendInfo.Name = useWebGpu ? "WebGPU" : "Skia";

        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            .GraphicsBackend(useWebGpu ? WebGpuBackend.CreateGraphicsProvider() : SkiaBackend.CreateGraphicsProvider())
            .UseX11()
            .UseLinuxFrameBuffer()
            .UseMacOS()
            .UseWin32()
            .Build();

        host.Run();
    }

    private static string ParseBackend(string[] args)
    {
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == "--backend" && i + 1 < args.Length)
            {
                return args[i + 1];
            }

            if (args[i].StartsWith("--backend=", StringComparison.Ordinal))
            {
                return args[i]["--backend=".Length..];
            }
        }

        return "skia";
    }
}
