using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
using EnterpriseDashboard.Observatory.Animation;
using EnterpriseDashboard.Observatory.Helpers;
using EnterpriseDashboard.Observatory.Models;

namespace EnterpriseDashboard.Observatory.Charts;

public sealed class HBarChartControl : ChartViewport
{
    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
        nameof(Items), typeof(HBarItem[]), typeof(HBarChartControl),
        new PropertyMetadata(null, (d, _) => ((HBarChartControl)d).EnsureBuilt()));

    public HBarItem[]? Items { get => (HBarItem[]?)GetValue(ItemsProperty); set => SetValue(ItemsProperty, value); }

    private const double LabelCol = 60, PadRight = 36, PadTop = 8, PadBottom = 8;
    private readonly List<Rectangle> _bars = new();

    protected override void BuildScene()
    {
        Surface.Children.Clear();
        _bars.Clear();
        if (Items == null || Items.Length == 0) return;

        var grey = (SolidColorBrush)Application.Current.Resources["ObsGreyBrush"];
        var plotW = SurfaceWidth - LabelCol - PadRight;
        var plotH = SurfaceHeight - PadTop - PadBottom;
        double slot = plotH / Items.Length;
        double barH = slot * 0.5;
        double maxVal = Items.Max(i => i.Value);

        for (int i = 0; i < Items.Length; i++)
        {
            var it = Items[i];
            double rowY = PadTop + slot * i + (slot - barH) / 2.0;

            var lbl = new TextBlock { Text = it.Label, Style = (Style)Application.Current.Resources["ObsAxisTick"] };
            Canvas.SetLeft(lbl, 4);
            Canvas.SetTop(lbl, rowY + barH / 2 - 5);
            Surface.Children.Add(lbl);

            double w = plotW * it.Value / Math.Max(1, maxVal);
            var rect = new Rectangle
            {
                Width = w,
                Height = barH,
                Fill = BrightnessMapper.FromValue(it.Value),
                RenderTransformOrigin = new Windows.Foundation.Point(0, 0.5),
                RenderTransform = new ScaleTransform { ScaleX = 0 }
            };
            Canvas.SetLeft(rect, LabelCol);
            Canvas.SetTop(rect, rowY);
            Surface.Children.Add(rect);
            _bars.Add(rect);

            var val = new TextBlock { Text = it.Value.ToString("0"), Style = (Style)Application.Current.Resources["ObsAxisLabel"] };
            Canvas.SetLeft(val, LabelCol + w + 6);
            Canvas.SetTop(val, rowY + barH / 2 - 5);
            Surface.Children.Add(val);
        }
    }

    // Race: bars extend left-to-right with top-down stagger.
    protected override Storyboard? CreateLoopStoryboard()
    {
        if (_bars.Count == 0) return base.CreateLoopStoryboard();
        Surface.Opacity = 1;
        var sb = new Storyboard { RepeatBehavior = RepeatBehavior.Forever };
        for (int i = 0; i < _bars.Count; i++)
        {
            sb.Children.Add(LoopAnimations.KeyFramesLoop(
                _bars[i], "(UIElement.RenderTransform).(ScaleTransform.ScaleX)",
                from: 0, to: 1, delayMs: 150 + i * 100, inMs: 700, cycleSec: 5.5,
                easeIn: EasingPreset.Bounce));
        }
        return sb;
    }
}
