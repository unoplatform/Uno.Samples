using Uno.UI.Hosting;
using NativeElementHostingDesktop;

App.InitializeLogging();

var host = UnoPlatformHostBuilder.Create()
    .App(() => new App())
    .UseX11()
    .UseLinuxFrameBuffer()
    .UseMacOS()
    .UseWin32()
    .Build();

host.Run();
