using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;
using EnterpriseDashboard.Observatory.Animation;
using EnterpriseDashboard.Observatory.Helpers;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace EnterpriseDashboard.Observatory.Charts;

public sealed class HistogramChartControl : ChartViewport
{
    public static readonly DependencyProperty BinsProperty = DependencyProperty.Register(
        nameof(Bins), typeof(double[]), typeof(HistogramChartControl),
        new PropertyMetadata(null, (d, _) => ((HistogramChartControl)d).EnsureBuilt()));

    public double[]? Bins { get => (double[]?)GetValue(BinsProperty); set => SetValue(BinsProperty, value); }

    private const double PadLeft = 34, PadRight = 16, PadTop = 16, PadBottom = 36;
    private readonly List<Rectangle> _binRects = new();
    private Path? _density;

    public HistogramChartControl() : base(740, 320) { }

    protected override void BuildScene()
    {
        Surface.Children.Clear();
        _binRects.Clear();
        _density = null;
        if (Bins == null) return;

        var plotW = SurfaceWidth - PadLeft - PadRight;
        var plotH = SurfaceHeight - PadTop - PadBottom;
        var grid = (SolidColorBrush)Application.Current.Resources["ObsGridBrush"];
        var grey = (SolidColorBrush)Application.Current.Resources["ObsGreyBrush"];
        var white = (SolidColorBrush)Application.Current.Resources["ObsWhiteBrush"];

        double max = 0;
        foreach (var v in Bins) if (v > max) max = v;
        if (max <= 0) max = 1;

        for (int i = 0; i <= 4; i++)
        {
            double y = PadTop + plotH * i / 4.0;
            Surface.Children.Add(new Line
            {
                X1 = PadLeft, X2 = PadLeft + plotW, Y1 = y, Y2 = y,
                Stroke = grid, StrokeThickness = 0.5
            });
            var lbl = new TextBlock
            {
                Text = ((int)Math.Round(max * (4 - i) / 4)).ToString(),
                Style = (Style)Application.Current.Resources["ObsAxisTick"]
            };
            Canvas.SetLeft(lbl, 6);
            Canvas.SetTop(lbl, y - 5);
            Surface.Children.Add(lbl);
        }

        int n = Bins.Length;
        double slot = plotW / n;
        double barW = slot - 2;
        var curvePts = new List<Point>(n);

        for (int i = 0; i < n; i++)
        {
            double v = Bins[i];
            double h = plotH * v / max;
            double x = PadLeft + slot * i + 1;
            double y = PadTop + plotH - h;
            double valPct = v / max * 100;

            var rect = new Rectangle
            {
                Width = barW,
                Height = h,
                Fill = BrightnessMapper.FromValue(valPct),
                RenderTransformOrigin = new Point(0.5, 1.0),
                RenderTransform = new ScaleTransform { ScaleY = 0 }
            };
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            Surface.Children.Add(rect);
            _binRects.Add(rect);

            curvePts.Add(new Point(x + barW / 2, y));
        }

        _density = new Path
        {
            Data = SplineBuilder.Build(curvePts),
            Stroke = white,
            StrokeThickness = 1.5,
            Opacity = 0
        };
        Surface.Children.Add(_density);

        double meanX = PadLeft + plotW * 0.5;
        Surface.Children.Add(new Line
        {
            X1 = meanX, X2 = meanX, Y1 = PadTop, Y2 = PadTop + plotH,
            Stroke = grey, StrokeThickness = 1, StrokeDashArray = new DoubleCollection { 4, 4 }, Opacity = 0.5
        });
        var muLabel = new TextBlock { Text = "μ = 50.2", Style = (Style)Application.Current.Resources["ObsAxisLabel"] };
        Canvas.SetLeft(muLabel, meanX + 4);
        Canvas.SetTop(muLabel, PadTop + 4);
        Surface.Children.Add(muLabel);

        for (int t = 0; t <= 4; t++)
        {
            double x = PadLeft + plotW * t / 4.0;
            var tb = new TextBlock { Text = $"{t * 25}", Style = (Style)Application.Current.Resources["ObsAxisTick"] };
            Canvas.SetLeft(tb, x - 6);
            Canvas.SetTop(tb, PadTop + plotH + 10);
            Surface.Children.Add(tb);
        }
        var axisLabel = new TextBlock { Text = "OBSERVED VALUE", Style = (Style)Application.Current.Resources["ObsAxisTick"] };
        Canvas.SetLeft(axisLabel, PadLeft + plotW / 2 - 40);
        Canvas.SetTop(axisLabel, PadTop + plotH + 24);
        Surface.Children.Add(axisLabel);

        var annot = new TextBlock
        {
            Text = "≈68% of observations fall\nwithin one standard deviation\nof the mean.",
            Style = (Style)Application.Current.Resources["ObsAnnotation"]
        };
        Canvas.SetLeft(annot, PadLeft + plotW - 180);
        Canvas.SetTop(annot, PadTop + 8);
        Surface.Children.Add(annot);
    }

    // Build: bins rise left-to-right (Riemann sum), then density curve fades in.
    protected override Storyboard? CreateLoopStoryboard()
    {
        if (_binRects.Count == 0) return base.CreateLoopStoryboard();
        Surface.Opacity = 1;
        var sb = new Storyboard { RepeatBehavior = RepeatBehavior.Forever };
        for (int i = 0; i < _binRects.Count; i++)
        {
            sb.Children.Add(LoopAnimations.KeyFramesLoop(
                _binRects[i], "(UIElement.RenderTransform).(ScaleTransform.ScaleY)",
                from: 0, to: 1, delayMs: 200 + i * 60, inMs: 600, cycleSec: 6.0,
                easeIn: EasingPreset.Out));
        }
        if (_density != null)
        {
            sb.Children.Add(LoopAnimations.KeyFramesLoop(
                _density, "Opacity",
                from: 0, to: 1, delayMs: 1800, inMs: 1200, cycleSec: 6.0));
        }
        return sb;
    }
}
