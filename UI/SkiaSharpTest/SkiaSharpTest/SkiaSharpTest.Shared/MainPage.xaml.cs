using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using SkiaSharp;
using SkiaSharp.Views.Windows;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SkiaSharpTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Point _currentPosition;

        public MainPage()
        {
#if !WINDOWS
            SKSwapChainPanel.RaiseOnUnsupported = false;
#endif

            this.InitializeComponent();
        }

        private Visibility Not(bool? value) => (!value ?? false) ? Visibility.Visible : Visibility.Collapsed;

		private void OnPaintSwapChain(object sender, SKPaintGLSurfaceEventArgs e)
		{
			// the the canvas and properties
			var canvas = e.Surface.Canvas;
			var info = e.Info;

			Render(canvas, new Size(info.Width, info.Height), SKColors.Green, "SkiaSharp Hardware Rendering");
		}

		private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
		{
			// the the canvas and properties
			var canvas = e.Surface.Canvas;
			var info = e.Info;

			Render(canvas, new Size(info.Width, info.Height), SKColors.Yellow, "SkiaSharp Software Rendering");
		}

		private void OnSurfacePointerMoved(object sender, PointerRoutedEventArgs e)
        {
            _currentPosition = e.GetCurrentPoint(panelGrid).Position;
            currentPositionText.Text = _currentPosition.ToString();
            
            if (hwAcceleration.IsChecked ?? false)
            {
#if !WINDOWS
				swapChain.Invalidate();
				swapChain2.Invalidate();
				swapChain3.Invalidate();
				swapChain4.Invalidate();
#endif
			}
            else
			{
				canvas.Invalidate();
				canvas2.Invalidate();
				canvas3.Invalidate();
				canvas4.Invalidate();
			}
        }

        private void Render(SKCanvas canvas, Size size, SKColor color, string text)
        {
            // get the screen density for scaling
            var scale = (float)this.XamlRoot.RasterizationScale;
            var scaledSize = new SKSize((float)size.Width / scale, (float)size.Height / scale);

            // handle the device screen density
            canvas.Scale(scale);

            // make sure the canvas is blank
            canvas.Clear(color);
			//canvas.Clear(SKColors.Transparent);

			var paint2 = new SKPaint
			{
				Color = SKColors.Blue,
				IsAntialias = true,
				Style = SKPaintStyle.Fill,
			};

			//Test Rect
			var rect0 = new SKRect((float)_currentPosition.X, (float)_currentPosition.Y, (float)size.Width / 2, (float)size.Height / 2);
			canvas.DrawRect(rect0, paint2);

			//Test Rect Radii
			CornerRadius = new CornerRadius(50);
			var rect = new SKRect(0, 0, (float)size.Width / 2, (float)size.Height / 2);
			var radii = new SKPoint[] {
				new SKPoint((float)CornerRadius.TopLeft, (float)CornerRadius.TopLeft),
				new SKPoint((float)CornerRadius.TopRight, (float)CornerRadius.TopRight),
				new SKPoint((float)CornerRadius.BottomRight,(float)CornerRadius.BottomRight),
				new SKPoint((float)CornerRadius.BottomLeft, (float)CornerRadius.BottomLeft)
			};
			var shape = new SKRoundRect();
			shape.SetRectRadii(rect, radii);
			canvas.DrawRoundRect(shape, paint2);

			//Test a text
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

			//Test a point
			var circlePaint = new SKPaint
			{
				Style = SKPaintStyle.Fill,
				IsAntialias = true,
				Color = SKColors.Red
			};
			canvas.DrawRect((float)_currentPosition.X, (float)_currentPosition.Y, (float)50, (float)50, circlePaint);
		}

    }
}
