using System;
using GLib;
using SkiaSharp.Views.UWP;
using Uno.UI.Runtime.Skia;

namespace SkiaSharpTest.Skia.Gtk
{
	class Program
	{
		static void Main(string[] args)
		{
			// SKSwapChain Panel is not supported inside of Uno yet.
			SKSwapChainPanel.RaiseOnUnsupported = false;

			ExceptionManager.UnhandledException += delegate (UnhandledExceptionArgs expArgs)
			{
				Console.WriteLine("GLIB UNHANDLED EXCEPTION" + expArgs.ExceptionObject.ToString());
				expArgs.ExitApplication = true;
			};

			var host = new GtkHost(() => new App(), args);

			host.Run();
		}
	}
}
