using SkiaSharp;
using SkiaSharp.Views.UWP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SkiaSharpTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

		private Visibility Not(bool? value) => (!value ?? false) ? Visibility.Visible : Visibility.Collapsed;

        private void OnPaintSwapChain(object sender, SKPaintGLSurfaceEventArgs e)
		{
			// the the canvas and properties
			var canvas = e.Surface.Canvas;

			Render(canvas, new Size(e.BackendRenderTarget.Width, e.BackendRenderTarget.Height), SKColors.Green, "SkiaSharp Hardware Rendering");
		}

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
		{
			// the the canvas and properties
			var canvas = e.Surface.Canvas;
			var info = e.Info;

			Render(canvas, new Size(info.Width, info.Height), SKColors.Yellow, "SkiaSharp Software Rendering");
		}

		private static void Render(SKCanvas canvas, Size size, SKColor color, string text)
		{
			// get the screen density for scaling
			var display = DisplayInformation.GetForCurrentView();
			var scale = display.LogicalDpi / 96.0f;
			var scaledSize = new SKSize((float)size.Width / scale, (float)size.Height / scale);

			// handle the device screen density
			canvas.Scale(scale);

			// make sure the canvas is blank
			canvas.Clear(color);

			// draw some text
			var paint = new SKPaint
			{
				Color = SKColors.Black,
				IsAntialias = true,
				Style = SKPaintStyle.Fill,
				TextAlign = SKTextAlign.Center,
				TextSize = 24
			};
			var coord = new SKPoint(scaledSize.Width / 2, (scaledSize.Height + paint.TextSize) / 2);
			canvas.DrawText(text, coord, paint);

			// Width 41.6587026 => 144.34135
			// Height 56 => 147
		}
	}
}
