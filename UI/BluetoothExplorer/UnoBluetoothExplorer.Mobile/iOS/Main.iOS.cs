using Foundation;
using Network;
using System.Diagnostics;
using UIKit;

namespace UnoBluetoothExplorer
{
    public class EntryPoint
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            var hasInfoKey = CoreFoundation.CFBundle.GetMain().InfoDictionary.ContainsKey(new NSString("NSBluetoothAlwaysUsageDescription"));
            Debug.WriteLine(hasInfoKey);
            
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, typeof(AppHead));
        }
    }
}
