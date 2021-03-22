using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace Authentication.OidcDemo.Skia.Tizen
{
	class Program
	{
		static void Main(string[] args)
		{
			var host = new TizenHost(() => new Authentication.OidcDemo.App(), args);
			host.Run();
		}
	}
}
