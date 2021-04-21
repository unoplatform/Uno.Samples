using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace SplashScreenSample.Skia.Tizen
{
	class Program
{
	static void Main(string[] args)
	{
		var host = new TizenHost(() => new SplashScreenSample.App(), args);
		host.Run();
	}
}
}
