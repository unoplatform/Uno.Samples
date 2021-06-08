using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace UnoCupertinoSample.Skia.Tizen
{
	class Program
	{
		static void Main(string[] args)
		{
			var host = new TizenHost(() => new UnoCupertinoSample.App(), args);
			host.Run();
		}
	}
}
