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
using EnterpriseDashboard.Observatory.Models;

namespace EnterpriseDashboard.Observatory.Charts;

public sealed class ScatterChartControl : ChartViewport
{
    public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
        nameof(Points), typeof(DataPoint[]), typeof(ScatterChartControl),
        new PropertyMetadata(null, (d, _) => ((ScatterChartControl)d).EnsureBuilt()));

    public DataPoint[]? Points { get => (DataPoint[]?)GetValue(PointsProperty); set => SetValue(PointsProperty, value); }

    private const double PadLeft = 28, PadRight = 12, PadTop = 12, PadBottom = 28;
    private readonly List<Ellipse> _dots = new();

    protected override void BuildScene()
    {
        Surface.Children.Clear();
        _dots.Clear();
        if (Points == null) return;

        var plotW = SurfaceWidth - PadLeft - PadRight;
        var plotH = SurfaceHeight - PadTop - PadBottom;
        var grid = (SolidColorBrush)Application.Current.Resources["ObsGridBrush"];
        var grey = (SolidColorBrush)Application.Current.Resources["ObsGreyBrush"];
        var light = (SolidColorBrush)Application.Current.Resources["ObsLightBrush"];

        for (int i = 0; i <= 4; i++)
        {
            double y = PadTop + plotH * i / 4.0;
            Surface.Children.Add(new Line
            {
                X1 = PadLeft, X2 = PadLeft + plotW, Y1 = y, Y2 = y,
                Stroke = grid, StrokeThickness = 0.5
            });
        }

        // Linear regression
        double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;
        foreach (var p in Points) { sumX += p.X; sumY += p.Y; sumXY += p.X * p.Y; sumX2 += p.X * p.X; }
        int n = Points.Length;
        double slope = (n * sumXY - sumX * sumY) / Math.Max(1e-6, n * sumX2 - sumX * sumX);
        double intercept = (sumY - slope * sumX) / n;

        var p1 = new Point(PadLeft, PadTop + plotH * (1 - intercept / 100));
        var p2 = new Point(PadLeft + plotW, PadTop + plotH * (1 - (slope * 100 + intercept) / 100));
        Surface.Children.Add(new Line
        {
            X1 = p1.X, X2 = p2.X, Y1 = p1.Y, Y2 = p2.Y,
            Stroke = light, StrokeThickness = 1, StrokeDashArray = new DoubleCollection { 4, 4 }
        });

        for (int i = 0; i < Points.Length; i++)
        {
            var p = Points[i];
            double x = PadLeft + plotW * Math.Clamp(p.X, 0, 100) / 100;
            double y = PadTop + plotH * (1 - Math.Clamp(p.Y, 0, 100) / 100);
            var brush = BrightnessMapper.FromValue(p.Y);
            var glow = MakeDot(new Point(x, y), 5, brush, 0.04);
            var dot = MakeDot(new Point(x, y), 2, brush, 0.85);
            dot.RenderTransform = new TranslateTransform { Y = -200 };
            glow.RenderTransform = new TranslateTransform { Y = -200 };
            Surface.Children.Add(glow);
            Surface.Children.Add(dot);
            _dots.Add(dot);
        }

        var annot = new TextBlock
        {
            Text = "Positive linear trend.\nObservations cluster\naround the regression line.",
            Style = (Style)Application.Current.Resources["ObsAnnotation"]
        };
        // Top-left: positive trend clusters toward the bottom-right, leaving this corner clear.
        Canvas.SetLeft(annot, PadLeft + 6);
        Canvas.SetTop(annot, PadTop + 6);
        Surface.Children.Add(annot);
    }

    // Rainfall: dots fall from above with deterministic stagger.
    protected override Storyboard? CreateLoopStoryboard()
    {
        if (_dots.Count == 0) return base.CreateLoopStoryboard();
        Surface.Opacity = 1;
        var sb = new Storyboard { RepeatBehavior = RepeatBehavior.Forever };
        for (int i = 0; i < _dots.Count; i++)
        {
            double delay = (i * 37) % 1800;
            sb.Children.Add(LoopAnimations.KeyFramesLoop(
                _dots[i], "(UIElement.RenderTransform).(TranslateTransform.Y)",
                from: -200, to: 0, delayMs: 400 + delay, inMs: 600, cycleSec: 5.5,
                easeIn: EasingPreset.Out));
        }
        return sb;
    }

    private static Ellipse MakeDot(Point p, double r, Brush fill, double opacity)
    {
        var e = new Ellipse { Width = r * 2, Height = r * 2, Fill = fill, Opacity = opacity };
        Canvas.SetLeft(e, p.X - r);
        Canvas.SetTop(e, p.Y - r);
        return e;
    }
}
