using GLib;
using System;
using Uno.UI.Runtime.Skia.Gtk;

namespace Uno.Sample.ControlLibrary.Skia.Gtk
{
    public sealed class Program
    {
        static void Main(string[] args)
        {
            ExceptionManager.UnhandledException += delegate (UnhandledExceptionArgs expArgs)
            {
                Console.WriteLine("GLIB UNHANDLED EXCEPTION" + expArgs.ExceptionObject.ToString());
                expArgs.ExitApplication = true;
            };

            var host = new GtkHost(() => new AppHead());

            host.Run();
        }
    }
}
