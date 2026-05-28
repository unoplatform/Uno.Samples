using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
using EnterpriseDashboard.Observatory.Animation;
using EnterpriseDashboard.Observatory.Models;

namespace EnterpriseDashboard.Observatory.Charts;

public sealed class BoxPlotChartControl : ChartViewport
{
    public static readonly DependencyProperty GroupsProperty = DependencyProperty.Register(
        nameof(Groups), typeof(BoxData[]), typeof(BoxPlotChartControl),
        new PropertyMetadata(null, (d, _) => ((BoxPlotChartControl)d).EnsureBuilt()));

    public BoxData[]? Groups { get => (BoxData[]?)GetValue(GroupsProperty); set => SetValue(GroupsProperty, value); }

    private const double PadLeft = 28, PadRight = 12, PadTop = 12, PadBottom = 24;
    private readonly List<Line> _medians = new();
    private readonly List<Rectangle> _boxes = new();
    private readonly List<Line> _whiskers = new();
    private readonly List<Ellipse> _outliers = new();

    protected override void BuildScene()
    {
        Surface.Children.Clear();
        _medians.Clear();
        _boxes.Clear();
        _whiskers.Clear();
        _outliers.Clear();
        if (Groups == null || Groups.Length == 0) return;

        var grid = (SolidColorBrush)Application.Current.Resources["ObsGridBrush"];
        var grey = (SolidColorBrush)Application.Current.Resources["ObsGreyBrush"];
        var light = (SolidColorBrush)Application.Current.Resources["ObsLightBrush"];
        var mid = (SolidColorBrush)Application.Current.Resources["ObsMidBrush"];
        var white = (SolidColorBrush)Application.Current.Resources["ObsWhiteBrush"];

        double plotW = SurfaceWidth - PadLeft - PadRight;
        double plotH = SurfaceHeight - PadTop - PadBottom;

        for (int i = 0; i <= 4; i++)
        {
            double x = PadLeft + plotW * i / 4.0;
            Surface.Children.Add(new Line
            {
                X1 = x, X2 = x, Y1 = PadTop, Y2 = PadTop + plotH,
                Stroke = grid, StrokeThickness = 0.5
            });
        }

        double slot = plotH / Groups.Length;
        double boxH = slot * 0.40;

        for (int i = 0; i < Groups.Length; i++)
        {
            var g = Groups[i];
            double rowY = PadTop + slot * i + slot / 2;

            double xMin = MapX(g.Min, plotW);
            double xQ1 = MapX(g.Q1, plotW);
            double xMed = MapX(g.Median, plotW);
            double xQ3 = MapX(g.Q3, plotW);
            double xMax = MapX(g.Max, plotW);

            var whisker = new Line
            {
                X1 = xMed, X2 = xMed, Y1 = rowY, Y2 = rowY,
                Stroke = mid, StrokeThickness = 1,
                RenderTransform = new ScaleTransform { ScaleX = 0, CenterX = xMed }
            };
            // Pre-compute desired endpoints to animate scale-X around the median
            // We'll instead draw with the final endpoints and scale around the median.
            whisker.X1 = xMin;
            whisker.X2 = xMax;
            whisker.RenderTransform = new ScaleTransform { ScaleX = 0, CenterX = xMed };
            Surface.Children.Add(whisker);
            _whiskers.Add(whisker);

            var box = new Rectangle
            {
                Width = xQ3 - xQ1,
                Height = boxH,
                Fill = new SolidColorBrush(light.Color) { Opacity = 0.10 },
                Stroke = light,
                StrokeThickness = 1,
                RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5),
                RenderTransform = new ScaleTransform { ScaleX = 0 }
            };
            Canvas.SetLeft(box, xQ1);
            Canvas.SetTop(box, rowY - boxH / 2);
            Surface.Children.Add(box);
            _boxes.Add(box);

            var median = new Line
            {
                X1 = xMed, X2 = xMed,
                Y1 = rowY - boxH / 2, Y2 = rowY + boxH / 2,
                Stroke = white, StrokeThickness = 2,
                RenderTransform = new ScaleTransform { ScaleY = 0, CenterX = xMed, CenterY = rowY }
            };
            Surface.Children.Add(median);
            _medians.Add(median);

            foreach (var o in g.Outliers)
            {
                double ox = MapX(o, plotW);
                var dot = new Ellipse
                {
                    Width = 4, Height = 4, Stroke = mid, StrokeThickness = 1, Fill = null,
                    Opacity = 0
                };
                Canvas.SetLeft(dot, ox - 2);
                Canvas.SetTop(dot, rowY - 2);
                Surface.Children.Add(dot);
                _outliers.Add(dot);
            }

            var lbl = new TextBlock { Text = g.Label, Style = (Style)Application.Current.Resources["ObsAxisTick"] };
            Canvas.SetLeft(lbl, 6);
            Canvas.SetTop(lbl, rowY - 5);
            Surface.Children.Add(lbl);
        }
    }

    // Unfold: medians first → boxes expand around medians → whiskers extend → outliers pop.
    protected override Storyboard? CreateLoopStoryboard()
    {
        if (_medians.Count == 0) return base.CreateLoopStoryboard();
        Surface.Opacity = 1;
        const double cycle = 6.0;
        var sb = new Storyboard { RepeatBehavior = RepeatBehavior.Forever };

        // Brief metaphor: medians pop (Spring), boxes & whiskers extend outward (Out), outliers pop (Spring).
        for (int i = 0; i < _medians.Count; i++)
            sb.Children.Add(LoopAnimations.KeyFramesLoop(
                _medians[i], "(UIElement.RenderTransform).(ScaleTransform.ScaleY)",
                0, 1, delayMs: 300 + i * 80, inMs: 400, cycleSec: cycle, easeIn: EasingPreset.Spring));

        for (int i = 0; i < _boxes.Count; i++)
            sb.Children.Add(LoopAnimations.KeyFramesLoop(
                _boxes[i], "(UIElement.RenderTransform).(ScaleTransform.ScaleX)",
                0, 1, delayMs: 900 + i * 60, inMs: 600, cycleSec: cycle, easeIn: EasingPreset.Out));

        for (int i = 0; i < _whiskers.Count; i++)
            sb.Children.Add(LoopAnimations.KeyFramesLoop(
                _whiskers[i], "(UIElement.RenderTransform).(ScaleTransform.ScaleX)",
                0, 1, delayMs: 1500 + i * 40, inMs: 500, cycleSec: cycle, easeIn: EasingPreset.Out));

        for (int j = 0; j < _outliers.Count; j++)
            sb.Children.Add(LoopAnimations.KeyFramesLoop(
                _outliers[j], "Opacity",
                0, 1, delayMs: 2000 + j * 100, inMs: 350, cycleSec: cycle, easeIn: EasingPreset.Spring));

        return sb;
    }

    private static double MapX(double v, double plotW) => PadLeft + plotW * Math.Clamp(v, 0, 100) / 100.0;
}
