using Tizen.Applications;

using Uno.UI.Runtime.Skia;

namespace DemoApp.Skia.Tizen
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new TizenHost(() => new DemoApp.App(), args);
            host.Run();
        }
    }
}
