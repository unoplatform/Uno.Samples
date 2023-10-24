using AppKit;

namespace WCTTestbed2
{
    // This is the main entry point of the application.
    public class EntryPoint
    {
        static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.SharedApplication.Delegate = new AppHead();
            NSApplication.Main(args);
        }
    }
}

