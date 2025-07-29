using Uno.UI.Hosting;

namespace Navigation;
public class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		var host = UnoPlatformHostBuilder.Create()
			.App(() => new App())
			.UseX11()
			.UseLinuxFrameBuffer()
			.UseMacOS()
			.UseWindows()
			.Build();

		host.Run();
	}
}
