using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace NativeFrameNav.Skia.Tizen
{
	class Program
{
	static void Main(string[] args)
	{
		var host = new TizenHost(() => new NativeFrameNav.App(), args);
		host.Run();
	}
}
}
