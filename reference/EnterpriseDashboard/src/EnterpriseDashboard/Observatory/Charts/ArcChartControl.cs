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

public sealed class ArcChartControl : ChartViewport
{
    public static readonly DependencyProperty MetricsProperty = DependencyProperty.Register(
        nameof(Metrics), typeof(RingMetric[]), typeof(ArcChartControl),
        new PropertyMetadata(null, (d, _) => ((ArcChartControl)d).EnsureBuilt()));

    public RingMetric[]? Metrics { get => (RingMetric[]?)GetValue(MetricsProperty); set => SetValue(MetricsProperty, value); }

    private readonly List<Microsoft.UI.Xaml.Shapes.Path> _filledArcs = new();

    protected override void BuildScene()
    {
        Surface.Children.Clear();
        _filledArcs.Clear();
        if (Metrics == null) return;

        var grid = (SolidColorBrush)Application.Current.Resources["ObsGridBrush"];
        var cx = SurfaceWidth / 2;
        var cy = SurfaceHeight / 2 + 6;

        foreach (var m in Metrics)
        {
            Surface.Children.Add(MakeArc(cx, cy, m.Radius, 0, 359.99, grid, 1.2, null));
        }

        for (int i = 0; i < Metrics.Length; i++)
        {
            var m = Metrics[i];
            double sweep = 359.99 * Math.Clamp(m.Value, 0, 100) / 100.0;
            var brush = BrightnessMapper.FromValue(m.Value);
            var rotate = new RotateTransform { CenterX = cx, CenterY = cy, Angle = -(80 + i * 25) };
            var arc = MakeArc(cx, cy, m.Radius, -90, -90 + sweep, brush, 1.6, rotate);
            Surface.Children.Add(arc);
            _filledArcs.Add(arc);
        }

        var pct = new TextBlock
        {
            Text = $"{(int)Metrics[0].Value}%",
            Style = (Style)Application.Current.Resources["ObsStatValue"]
        };
        pct.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        Canvas.SetLeft(pct, cx - pct.DesiredSize.Width / 2);
        Canvas.SetTop(pct, cy - 22);
        Surface.Children.Add(pct);
    }

    // Breathe: each ring rotates from a scattered starting angle to 0, staggered.
    protected override Storyboard? CreateLoopStoryboard()
    {
        if (_filledArcs.Count == 0) return base.CreateLoopStoryboard();
        Surface.Opacity = 1;
        var sb = new Storyboard { RepeatBehavior = RepeatBehavior.Forever };
        for (int i = 0; i < _filledArcs.Count; i++)
        {
            sb.Children.Add(LoopAnimations.KeyFramesLoop(
                _filledArcs[i], "(UIElement.RenderTransform).(RotateTransform.Angle)",
                from: -(80 + i * 25), to: 0,
                delayMs: 300 + i * 220, inMs: 1300, cycleSec: 5.5,
                easeIn: EasingPreset.Smooth));
        }
        return sb;
    }

    private static Microsoft.UI.Xaml.Shapes.Path MakeArc(double cx, double cy, double r, double a0, double a1, Brush stroke, double thickness, RotateTransform? transform)
    {
        double rad0 = a0 * Math.PI / 180;
        double rad1 = a1 * Math.PI / 180;
        var p0 = new Point(cx + r * Math.Cos(rad0), cy + r * Math.Sin(rad0));
        var p1 = new Point(cx + r * Math.Cos(rad1), cy + r * Math.Sin(rad1));
        bool large = Math.Abs(a1 - a0) > 180;

        var fig = new PathFigure { StartPoint = p0, IsClosed = false };
        fig.Segments.Add(new ArcSegment
        {
            Point = p1,
            Size = new Size(r, r),
            SweepDirection = SweepDirection.Clockwise,
            IsLargeArc = large
        });
        var geo = new PathGeometry();
        geo.Figures.Add(fig);

        return new Microsoft.UI.Xaml.Shapes.Path
        {
            Data = geo,
            Stroke = stroke,
            StrokeThickness = thickness,
            RenderTransform = transform
        };
    }
}
