using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.UITest.Helpers.Queries;

namespace ToDo.UI.Tests
{
    public class Constants
    {
        public readonly static string WebAssemblyDefaultUri = "https://localhost:54599";
        public readonly static string iOSAppName = "com.example.app";
        public readonly static string AndroidAppName = "com.example.app";
        public readonly static string iOSDeviceNameOrId = "iPad Pro (12.9-inch) (3rd generation)";

        public readonly static Platform CurrentPlatform = Platform.Browser;
    }
}
