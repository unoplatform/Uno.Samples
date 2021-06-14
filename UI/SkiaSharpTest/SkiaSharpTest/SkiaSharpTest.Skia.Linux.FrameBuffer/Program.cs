using SkiaSharp.Views.UWP;
using System;
using Uno.UI.Runtime.Skia;

namespace SkiaSharpTest
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				Console.CursorVisible = false;
				
				// SKSwapChain Panel is not supported inside of Uno yet.
				SKSwapChainPanel.RaiseOnUnsupported = false;


				var host = new FrameBufferHost(() => new App(), args);
				host.Run();
			}
			finally
			{
				Console.CursorVisible = true;
			}
		}
	}
}
