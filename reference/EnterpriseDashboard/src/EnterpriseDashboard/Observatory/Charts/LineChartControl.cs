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

public sealed class LineChartControl : ChartViewport
{
    public static readonly DependencyProperty SeriesAProperty = DependencyProperty.Register(
        nameof(SeriesA), typeof(DataPoint[]), typeof(LineChartControl),
        new PropertyMetadata(null, (d, _) => ((LineChartControl)d).EnsureBuilt()));

    public DataPoint[]? SeriesA { get => (DataPoint[]?)GetValue(SeriesAProperty); set => SetValue(SeriesAProperty, value); }

    public static readonly DependencyProperty SeriesBProperty = DependencyProperty.Register(
        nameof(SeriesB), typeof(DataPoint[]), typeof(LineChartControl),
        new PropertyMetadata(null, (d, _) => ((LineChartControl)d).EnsureBuilt()));

    public DataPoint[]? SeriesB { get => (DataPoint[]?)GetValue(SeriesBProperty); set => SetValue(SeriesBProperty, value); }

    private const double PadLeft = 34, PadRight = 12, PadTop = 12, PadBottom = 28;
    private Path? _pathA;
    private Path? _pathB;
    private Canvas? _drawLayer;
    private readonly List<UIElement> _dots = new();

    protected override void BuildScene()
    {
        Surface.Children.Clear();
        _dots.Clear();
        _pathA = null;
        _pathB = null;
        _drawLayer = null;
        if (SeriesA == null || SeriesB == null) return;

        var plotW = SurfaceWidth - PadLeft - PadRight;
        var plotH = SurfaceHeight - PadTop - PadBottom;
        var grid = (SolidColorBrush)Application.Current.Resources["ObsGridBrush"];
        var grey = (SolidColorBrush)Application.Current.Resources["ObsGreyBrush"];
        var white = (SolidColorBrush)Application.Current.Resources["ObsWhiteBrush"];

        for (int i = 0; i <= 4; i++)
        {
            double y = PadTop + plotH * i / 4.0;
            Surface.Children.Add(new Line
            {
                X1 = PadLeft, X2 = PadLeft + plotW, Y1 = y, Y2 = y,
                Stroke = grid, StrokeThickness = 0.5
            });
            AddLabel($"{100 - i * 25}", PadLeft - 6, y - 5, grey, alignRight: true);
        }

        var aPts = MapPoints(SeriesA, plotW, plotH);
        var bPts = MapPoints(SeriesB, plotW, plotH);

        // Reference line B (dashed dim)
        _pathB = new Path
        {
            Data = SplineBuilder.Build(bPts),
            Stroke = grey, StrokeThickness = 1,
            StrokeDashArray = new DoubleCollection { 3, 3 }
        };
        Surface.Children.Add(_pathB);

        // Primary line A + dots live in a draw-layer that scales in from the left so the
        // series "draws on" left -> right. (Uno doesn't implement StrokeDashOffset, so the
        // original pen-stroke reveal never ran; a ScaleX wipe is the supported equivalent.)
        _drawLayer = new Canvas
        {
            RenderTransform = new ScaleTransform { ScaleX = 0 },
            RenderTransformOrigin = new Point(0, 0.5)
        };
        Surface.Children.Add(_drawLayer);

        _pathA = new Path
        {
            Data = SplineBuilder.Build(aPts),
            Stroke = white, StrokeThickness = 1.5
        };
        _drawLayer.Children.Add(_pathA);

        for (int i = 0; i < aPts.Count; i++)
        {
            var p = aPts[i];
            var brush = BrightnessMapper.FromValue(SeriesA![i].Y);
            var glow = MakeDot(p, 6, brush, 0.06);
            var dot = MakeDot(p, 2.5, brush, 1.0);
            _drawLayer.Children.Add(glow);
            _drawLayer.Children.Add(dot);
            _dots.Add(glow);
            _dots.Add(dot);
        }

        var annot = new TextBlock
        {
            Text = "Peak observed in Sep.\nPrimary channel diverges\nfrom reference baseline.",
            Style = (Style)Application.Current.Resources["ObsAnnotation"]
        };
        // Top-left: the series sits low on the left, so the annotation stays clear of the line.
        Canvas.SetLeft(annot, PadLeft + 6);
        Canvas.SetTop(annot, PadTop + 2);
        Surface.Children.Add(annot);

        string[] months = { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
        for (int i = 0; i < months.Length; i++)
        {
            double x = PadLeft + plotW * i / (months.Length - 1.0);
            AddLabel(months[i], x - 7, PadTop + plotH + 8, grey, axisTick: true);
        }
    }

    protected override Storyboard? CreateLoopStoryboard()
    {
        if (_drawLayer == null) return base.CreateLoopStoryboard();
        Surface.Opacity = 1;   // axes / grid / reference line are static; the series draws on
        var sb = new Storyboard { RepeatBehavior = RepeatBehavior.Forever };
        sb.Children.Add(LoopAnimations.KeyFramesLoop(
            _drawLayer, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)",
            from: 0, to: 1, delayMs: 0, inMs: 1150, cycleSec: 5.5, easeIn: EasingPreset.Out));
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

    private static Ellipse MakeDot(Point p, double r, Brush fill, double opacity)
    {
        var e = new Ellipse { Width = r * 2, Height = r * 2, Fill = fill, Opacity = opacity };
        Canvas.SetLeft(e, p.X - r);
        Canvas.SetTop(e, p.Y - r);
        return e;
    }

    private void AddLabel(string text, double x, double y, Brush brush, bool alignRight = false, bool axisTick = false)
    {
        var tb = new TextBlock
        {
            Text = text,
            Style = (Style)Application.Current.Resources[axisTick ? "ObsAxisTick" : "ObsAxisLabel"]
        };
        if (alignRight)
        {
            tb.TextAlignment = TextAlignment.Right;
            tb.Width = 24;
            Canvas.SetLeft(tb, x - 24);
        }
        else
        {
            Canvas.SetLeft(tb, x);
        }
        Canvas.SetTop(tb, y);
        Surface.Children.Add(tb);
    }
}
