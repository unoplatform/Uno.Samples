using Microsoft.UI.Xaml.Controls;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using System.IO;
using Windows.ApplicationModel;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

namespace XamlBrewerUnoApp
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;

            canvas.Clear(SKColors.Transparent);

            // Read and draw an SVG file
            var svg = new SKSvg();
            svg.Load(SkiaSharpSample.SampleMedia.Embedded.Load("winuilogo.svg"));
            var coord = new SKPoint(80, (e.Info.Height - svg.ViewBox.Height) / 2); // Origin = Top Left
            //var coord = new SKPoint(80, (e.Info.Height - svg.Picture.CullRect.Height) / 2);
            canvas.DrawPicture(svg.Picture, coord);

            // Draw some text
            string text = "SkiaSharp on WinUI";
            var paint = new SKPaint
            {
                Color = SKColors.Gold,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2,
                TextAlign = SKTextAlign.Center,
                TextSize = 58
            };
            var bounds = new SKRect();
            paint.MeasureText(text, ref bounds);
            coord = new SKPoint(e.Info.Width / 2, (e.Info.Height + bounds.Height) / 2); // Origin = Bottom Left
            canvas.DrawText(text, coord, paint);

            // Overlapping transparent SVG sample
            //svg = new SKSvg();
            //svg.Load(SampleMedia.Images.OpacitySvg);
            //coord = new SKPoint(e.Info.Width - svg.Picture.CullRect.Width, 0);
            //canvas.DrawPicture(svg.Picture, coord);
            //coord = new SKPoint(e.Info.Width - svg.Picture.CullRect.Width, e.Info.Height - svg.Picture.CullRect.Height);
            //canvas.DrawPicture(svg.Picture, coord);
        }
    }
}

