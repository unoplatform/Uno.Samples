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

public sealed class SlopeChartControl : ChartViewport
{
    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
        nameof(Items), typeof(SlopeItem[]), typeof(SlopeChartControl),
        new PropertyMetadata(null, (d, _) => ((SlopeChartControl)d).EnsureBuilt()));

    public SlopeItem[]? Items { get => (SlopeItem[]?)GetValue(ItemsProperty); set => SetValue(ItemsProperty, value); }

    public SlopeChartControl() : base(740, 300) { }

    private const double PadLeft = 60, PadRight = 60, PadTop = 36, PadBottom = 36;
    private readonly List<Ellipse> _leftDots = new();
    private readonly List<Line> _bridges = new();
    private readonly List<Ellipse> _rightDots = new();

    protected override void BuildScene()
    {
        Surface.Children.Clear();
        _leftDots.Clear();
        _bridges.Clear();
        _rightDots.Clear();
        if (Items == null || Items.Length == 0) return;

        var grey = (SolidColorBrush)Application.Current.Resources["ObsGreyBrush"];
        var light = (SolidColorBrush)Application.Current.Resources["ObsLightBrush"];
        var mid = (SolidColorBrush)Application.Current.Resources["ObsMidBrush"];
        var white = (SolidColorBrush)Application.Current.Resources["ObsWhiteBrush"];

        double xLeft = PadLeft + 80;
        double xRight = SurfaceWidth - PadRight - 80;
        double yTop = PadTop;
        double yBottom = SurfaceHeight - PadBottom;
        double plotH = yBottom - yTop;

        var lblBefore = new TextBlock { Text = "BEFORE", Style = (Style)Application.Current.Resources["ObsMonoTag"], Foreground = mid };
        Canvas.SetLeft(lblBefore, xLeft - 30);
        Canvas.SetTop(lblBefore, yTop - 24);
        Surface.Children.Add(lblBefore);

        var lblAfter = new TextBlock { Text = "AFTER", Style = (Style)Application.Current.Resources["ObsMonoTag"], Foreground = mid };
        Canvas.SetLeft(lblAfter, xRight - 8);
        Canvas.SetTop(lblAfter, yTop - 24);
        Surface.Children.Add(lblAfter);

        foreach (var it in Items)
        {
            double yBefore = yTop + plotH * (1 - it.Before / 100.0);
            double yAfter = yTop + plotH * (1 - it.After / 100.0);
            bool rising = it.After > it.Before;
            var lineBrush = rising ? light : grey;
            double lineOpacity = rising ? 0.7 : 0.35;

            // Line: scale from left point (origin (0, 0.5) won't work since shapes use Canvas pos).
            // Use ScaleX with CenterX at left endpoint.
            var line = new Line
            {
                X1 = xLeft, X2 = xRight, Y1 = yBefore, Y2 = yAfter,
                Stroke = lineBrush, StrokeThickness = 1, Opacity = lineOpacity,
                RenderTransform = new ScaleTransform { ScaleX = 0, CenterX = xLeft, CenterY = yBefore }
            };
            Surface.Children.Add(line);
            _bridges.Add(line);

            var leftDot = MakeDot(xLeft, yBefore, rising ? white : mid);
            Surface.Children.Add(leftDot);
            _leftDots.Add(leftDot);

            var lblItem = new TextBlock { Text = it.Label, Style = (Style)Application.Current.Resources["ObsAxisLabel"], Foreground = rising ? light : grey };
            Canvas.SetLeft(lblItem, xLeft - 70);
            Canvas.SetTop(lblItem, yBefore - 6);
            Surface.Children.Add(lblItem);

            var lblBeforeVal = new TextBlock { Text = it.Before.ToString("0"), Style = (Style)Application.Current.Resources["ObsAxisLabel"], Foreground = mid };
            Canvas.SetLeft(lblBeforeVal, xLeft - 30);
            Canvas.SetTop(lblBeforeVal, yBefore - 6);
            Surface.Children.Add(lblBeforeVal);

            var rightDot = MakeDot(xRight, yAfter, rising ? white : mid);
            Surface.Children.Add(rightDot);
            _rightDots.Add(rightDot);

            var lblAfterVal = new TextBlock { Text = it.After.ToString("0"), Style = (Style)Application.Current.Resources["ObsAxisLabel"], Foreground = mid };
            Canvas.SetLeft(lblAfterVal, xRight + 12);
            Canvas.SetTop(lblAfterVal, yAfter - 6);
            Surface.Children.Add(lblAfterVal);
        }

        var annot = new TextBlock
        {
            Text = "Lines rising left-to-right\nindicate improvement.\nDeclining slopes show regression.",
            Style = (Style)Application.Current.Resources["ObsAnnotation"]
        };
        Canvas.SetLeft(annot, SurfaceWidth / 2 - 80);
        Canvas.SetTop(annot, yBottom + 6);
        Surface.Children.Add(annot);
    }

    // Bridge: left dots first → lines reach across → right dots arrive.
    protected override Storyboard? CreateLoopStoryboard()
    {
        if (_leftDots.Count == 0) return base.CreateLoopStoryboard();
        Surface.Opacity = 1;
        const double cycle = 6.5;
        var sb = new Storyboard { RepeatBehavior = RepeatBehavior.Forever };

        for (int i = 0; i < _leftDots.Count; i++)
        {
            _leftDots[i].Opacity = 0;
            _rightDots[i].Opacity = 0;
            // Brief metaphor: dots pop (Spring), bridge line stretches with gravity-settle (Out).
            sb.Children.Add(LoopAnimations.KeyFramesLoop(
                _leftDots[i], "Opacity",
                0, 1, delayMs: 300 + i * 80, inMs: 400, cycleSec: cycle, easeIn: EasingPreset.Spring));
            sb.Children.Add(LoopAnimations.KeyFramesLoop(
                _bridges[i], "(UIElement.RenderTransform).(ScaleTransform.ScaleX)",
                0, 1, delayMs: 900 + i * 80, inMs: 800, cycleSec: cycle, easeIn: EasingPreset.Out));
            sb.Children.Add(LoopAnimations.KeyFramesLoop(
                _rightDots[i], "Opacity",
                0, 1, delayMs: 1700 + i * 80, inMs: 400, cycleSec: cycle, easeIn: EasingPreset.Spring));
        }
        return sb;
    }

    private static Ellipse MakeDot(double x, double y, SolidColorBrush fill)
    {
        var e = new Ellipse { Width = 6, Height = 6, Fill = fill };
        Canvas.SetLeft(e, x - 3);
        Canvas.SetTop(e, y - 3);
        return e;
    }
}
