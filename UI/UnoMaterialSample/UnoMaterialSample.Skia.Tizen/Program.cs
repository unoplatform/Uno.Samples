using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace UnoMaterialSample.Skia.Tizen
{
	class Program
{
	static void Main(string[] args)
	{
		var host = new TizenHost(() => new UnoMaterialSample.App(), args);
		host.Run();
	}
}
}
