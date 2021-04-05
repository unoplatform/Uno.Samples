using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace UnoLocalization.Skia.Tizen
{
	class Program
{
	static void Main(string[] args)
	{
		var host = new TizenHost(() => new UnoLocalization.App(), args);
		host.Run();
	}
}
}
