using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
using EnterpriseDashboard.Observatory.Animation;
using EnterpriseDashboard.Observatory.Helpers;

namespace EnterpriseDashboard.Observatory.Charts;

public sealed class BarChartControl : ChartViewport
{
    public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register(
        nameof(Values), typeof(double[]), typeof(BarChartControl),
        new PropertyMetadata(null, (d, _) => ((BarChartControl)d).EnsureBuilt()));

    public double[]? Values { get => (double[]?)GetValue(ValuesProperty); set => SetValue(ValuesProperty, value); }

    private const double PadLeft = 28, PadRight = 12, PadTop = 12, PadBottom = 28;
    private readonly List<Rectangle> _bars = new();

    protected override void BuildScene()
    {
        Surface.Children.Clear();
        _bars.Clear();
        if (Values == null) return;

        var plotW = SurfaceWidth - PadLeft - PadRight;
        var plotH = SurfaceHeight - PadTop - PadBottom;
        var grid = (SolidColorBrush)Application.Current.Resources["ObsGridBrush"];
        var grey = (SolidColorBrush)Application.Current.Resources["ObsGreyBrush"];

        for (int i = 0; i <= 4; i++)
        {
            double y = PadTop + plotH * i / 4.0;
            Surface.Children.Add(new Line
            {
                X1 = PadLeft, X2 = PadLeft + plotW, Y1 = y, Y2 = y,
                Stroke = grid, StrokeThickness = 0.5
            });
        }

        var n = Values.Length;
        double slot = plotW / n;
        double barW = slot * 0.62;

        for (int i = 0; i < n; i++)
        {
            double v = Math.Clamp(Values[i], 0, 100);
            double h = plotH * v / 100.0;
            double x = PadLeft + slot * i + (slot - barW) / 2.0;
            double y = PadTop + plotH - h;

            var rect = new Rectangle
            {
                Width = barW,
                Height = h,
                Fill = BrightnessMapper.FromValue(v),
                RenderTransformOrigin = new Windows.Foundation.Point(0.5, 1.0),
                RenderTransform = new ScaleTransform { ScaleY = 0 }
            };
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            Surface.Children.Add(rect);
            _bars.Add(rect);
        }

        string[] months = { "J", "F", "M", "A", "M", "J", "J", "A", "S", "O", "N", "D" };
        for (int i = 0; i < n && i < months.Length; i++)
        {
            double x = PadLeft + slot * i + slot / 2.0;
            var tb = new TextBlock { Text = months[i], Style = (Style)Application.Current.Resources["ObsAxisTick"] };
            Canvas.SetLeft(tb, x - 4);
            Canvas.SetTop(tb, PadTop + plotH + 8);
            Surface.Children.Add(tb);
        }
    }

    // Emerge: bars rise from baseline, center-outward stagger.
    protected override Storyboard? CreateLoopStoryboard()
    {
        if (_bars.Count == 0) return base.CreateLoopStoryboard();
        Surface.Opacity = 1;
        var sb = new Storyboard { RepeatBehavior = RepeatBehavior.Forever };
        double center = (_bars.Count - 1) / 2.0;
        for (int i = 0; i < _bars.Count; i++)
        {
            double delay = 150 + Math.Abs(i - center) * 80;
            sb.Children.Add(LoopAnimations.KeyFramesLoop(
                _bars[i], "(UIElement.RenderTransform).(ScaleTransform.ScaleY)",
                from: 0, to: 1, delayMs: delay, inMs: 900, cycleSec: 5.5,
                easeIn: EasingPreset.Bounce));
        }
        return sb;
    }
}
