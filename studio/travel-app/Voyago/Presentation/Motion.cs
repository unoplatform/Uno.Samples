using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace Voyago.Presentation;

/// <summary>
/// A one-orchestrated staggered entrance: each direct child of the panel fades in and rises 14px,
/// ~280ms on the house EaseSmooth curve, offset by index. Honours reduced motion (jumps to the end
/// state when system animations are off). Attach with <c>local:Motion.Stagger="True"</c> on a Panel.
/// </summary>
public static class Motion
{
    public static readonly DependencyProperty StaggerProperty =
        DependencyProperty.RegisterAttached(
            "Stagger", typeof(bool), typeof(Motion), new PropertyMetadata(false, OnStaggerChanged));

    public static bool GetStagger(DependencyObject o) => (bool)o.GetValue(StaggerProperty);
    public static void SetStagger(DependencyObject o, bool value) => o.SetValue(StaggerProperty, value);

    private static void OnStaggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Panel panel && e.NewValue is true)
        {
            panel.Loaded += (_, _) => Play(panel);
        }
    }

    private static bool AnimationsEnabled()
    {
        try { return new UISettings().AnimationsEnabled; }
        catch { return true; }
    }

    private static void Play(Panel panel)
    {
        var animate = AnimationsEnabled();
        var index = 0;
        foreach (var child in panel.Children)
        {
            if (child is not FrameworkElement fe)
            {
                continue;
            }

            if (!animate)
            {
                fe.Opacity = 1;
                continue;
            }

            var transform = new TranslateTransform { Y = 14 };
            fe.RenderTransform = transform;
            fe.Opacity = 0;

            var ease = new KeySpline { ControlPoint1 = new Point(0.22, 1), ControlPoint2 = new Point(0.36, 1) };
            var begin = TimeSpan.FromMilliseconds(index * 70);

            var fade = new DoubleAnimationUsingKeyFrames { BeginTime = begin, EnableDependentAnimation = true };
            fade.KeyFrames.Add(new SplineDoubleKeyFrame { KeyTime = TimeSpan.Zero, Value = 0, KeySpline = ease });
            fade.KeyFrames.Add(new SplineDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(280), Value = 1, KeySpline = ease });
            Storyboard.SetTarget(fade, fe);
            Storyboard.SetTargetProperty(fade, "Opacity");

            var rise = new DoubleAnimationUsingKeyFrames { BeginTime = begin, EnableDependentAnimation = true };
            rise.KeyFrames.Add(new SplineDoubleKeyFrame { KeyTime = TimeSpan.Zero, Value = 14, KeySpline = ease });
            rise.KeyFrames.Add(new SplineDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(280), Value = 0, KeySpline = ease });
            Storyboard.SetTarget(rise, transform);
            Storyboard.SetTargetProperty(rise, "Y");

            var storyboard = new Storyboard();
            storyboard.Children.Add(fade);
            storyboard.Children.Add(rise);
            storyboard.Begin();

            index++;
        }
    }
}
