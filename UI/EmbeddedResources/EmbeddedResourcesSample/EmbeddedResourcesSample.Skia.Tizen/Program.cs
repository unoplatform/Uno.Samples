using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace EmbeddedResourcesSample.Skia.Tizen
{
	class Program
	{
		static void Main(string[] args)
		{
			var host = new TizenHost(() => new EmbeddedResourcesSample.App(), args);
			host.Run();
		}
	}
}
