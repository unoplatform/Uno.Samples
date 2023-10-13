using Microsoft.UI.Xaml.Controls;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using System;

namespace XamlBrewerUnoApp
{
    public sealed partial class PythagorasTreePage : Page
    {
        public PythagorasTreePage()
        {
            this.InitializeComponent();
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;

            canvas.Clear(SKColors.Transparent);

            // Configure and draw a Pythagoras Tree

            var side = 120f;
            var angle = 36f;
            var paint = new SKPaint
            {
                Color = SKColors.Brown,
                IsAntialias = true
            };

            canvas.Translate(e.Info.Width / 2, e.Info.Height - side);

            var r = new SKRect(0, 0, side, side);
            DrawNode(canvas, paint, r, angle, 15);
        }

        private static void DrawNode(SKCanvas canvas, SKPaint paint, SKRect rect, float angle, int steps)
        {
            // Recursion control
            steps--;
            if (steps == 0)
            {
                return;
            }

            // Trigonometrics
            var sine = (float)Math.Sin(angle * 2 * Math.PI / 360);
            var cosine = (float)Math.Cos(angle * 2 * Math.PI / 360);

            // Trunk
            canvas.DrawRect(rect, paint);

            // Left branch
            canvas.Save();
            var leftSide = (float)(rect.Width * cosine);
            canvas.Translate((float)(-leftSide * sine), (float)(-leftSide * cosine));
            canvas.RotateDegrees(-angle);
            canvas.Scale(cosine);
            DrawNode(canvas, paint, rect, angle, steps);
            canvas.Restore();

            // Right branch
            canvas.Save();
            var rightSide = (float)(rect.Width * sine);
            canvas.Translate((float)((rightSide + leftSide) * cosine), (float)(-(rightSide + leftSide) * sine));
            canvas.RotateDegrees(90 - angle);
            canvas.Scale(sine);
            DrawNode(canvas, paint, rect, angle, steps);
            canvas.Restore();
        }
    }
}
