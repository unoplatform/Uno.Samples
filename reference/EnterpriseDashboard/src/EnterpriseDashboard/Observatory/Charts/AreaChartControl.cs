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
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace EnterpriseDashboard.Observatory.Charts;

public sealed class AreaChartControl : ChartViewport
{
    public static readonly DependencyProperty LayerAProperty = DependencyProperty.Register(
        nameof(LayerA), typeof(DataPoint[]), typeof(AreaChartControl),
        new PropertyMetadata(null, (d, _) => ((AreaChartControl)d).EnsureBuilt()));
    public DataPoint[]? LayerA { get => (DataPoint[]?)GetValue(LayerAProperty); set => SetValue(LayerAProperty, value); }

    public static readonly DependencyProperty LayerBProperty = DependencyProperty.Register(
        nameof(LayerB), typeof(DataPoint[]), typeof(AreaChartControl),
        new PropertyMetadata(null, (d, _) => ((AreaChartControl)d).EnsureBuilt()));
    public DataPoint[]? LayerB { get => (DataPoint[]?)GetValue(LayerBProperty); set => SetValue(LayerBProperty, value); }

    private const double PadLeft = 28, PadRight = 12, PadTop = 12, PadBottom = 28;
    private readonly List<FrameworkElement> _wipeTargets = new();

    protected override void BuildScene()
    {
        Surface.Children.Clear();
        _wipeTargets.Clear();
        if (LayerA == null || LayerB == null) return;

        var plotW = SurfaceWidth - PadLeft - PadRight;
        var plotH = SurfaceHeight - PadTop - PadBottom;
        var grid = (SolidColorBrush)Application.Current.Resources["ObsGridBrush"];
        var mid = (SolidColorBrush)Application.Current.Resources["ObsMidBrush"];
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

        var stacked = new DataPoint[LayerA.Length];
        for (int i = 0; i < stacked.Length; i++)
            stacked[i] = new DataPoint(LayerA[i].X, LayerA[i].Y + LayerB[i].Y);

        var lightArea = DrawArea(MapPoints(stacked, plotW, plotH), light, 0.18);
        var midArea = DrawArea(MapPoints(LayerA, plotW, plotH), mid, 0.35);
        _wipeTargets.Add(lightArea);
        _wipeTargets.Add(midArea);

        var topPts = MapPoints(LayerA, plotW, plotH);
        var stroke = new Path
        {
            Data = SplineBuilder.Build(topPts),
            Stroke = light, StrokeThickness = 1.2,
            RenderTransformOrigin = new Point(0, 0.5),
            RenderTransform = new ScaleTransform { ScaleX = 0 }
        };
        Surface.Children.Add(stroke);
        _wipeTargets.Add(stroke);
    }

    // Reveal: paths wipe from left to right (ScaleX 0 → 1, origin (0, 0.5)).
    protected override Storyboard? CreateLoopStoryboard()
    {
        if (_wipeTargets.Count == 0) return base.CreateLoopStoryboard();
        Surface.Opacity = 1;
        var sb = new Storyboard { RepeatBehavior = RepeatBehavior.Forever };
        foreach (var t in _wipeTargets)
        {
            sb.Children.Add(LoopAnimations.KeyFramesLoop(
                t, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)",
                from: 0, to: 1, delayMs: 200, inMs: 2400, cycleSec: 5.5,
                easeIn: EasingPreset.Snap));
        }
        return sb;
    }

    private List<Point> MapPoints(DataPoint[] data, double plotW, double plotH)
    {
        var list = new List<Point>(data.Length);
        for (int i = 0; i < data.Length; i++)
        {
            double x = PadLeft + plotW * i / (data.Length - 1.0);
            double y = PadTop + plotH * (1 - Math.Clamp(data[i].Y, 0, 100) / 100.0);
            list.Add(new Point(x, y));
        }
        return list;
    }

    private Path DrawArea(List<Point> pts, SolidColorBrush fill, double opacity)
    {
        var geo = SplineBuilder.Build(pts);
        var fig = (PathFigure)geo.Figures[0];
        fig.IsFilled = true;
        fig.IsClosed = true;
        var last = pts[^1];
        var first = pts[0];
        fig.Segments.Add(new LineSegment { Point = new Point(last.X, PadTop + (SurfaceHeight - PadTop - PadBottom)) });
        fig.Segments.Add(new LineSegment { Point = new Point(first.X, PadTop + (SurfaceHeight - PadTop - PadBottom)) });

        var path = new Path
        {
            Data = geo,
            Fill = new SolidColorBrush(fill.Color) { Opacity = opacity },
            RenderTransformOrigin = new Point(0, 0.5),
            RenderTransform = new ScaleTransform { ScaleX = 0 }
        };
        Surface.Children.Add(path);
        return path;
    }
}
