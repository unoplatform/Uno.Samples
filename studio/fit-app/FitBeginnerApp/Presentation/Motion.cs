using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace FitBeginnerApp.Presentation;

/// <summary>
/// Shared entrance motion so a page reveals in one orchestrated load — a fade plus a short rise on
/// the house EaseSmooth curve, with a per-section stagger. Opacity + TranslateY are independent
/// (render-thread) animations, so they stay smooth; the whole thing is skipped when the OS asks for
/// reduced motion, leaving content at its natural, visible state.
/// </summary>
public static class Motion
{
    private static readonly bool AnimationsEnabled = new UISettings().AnimationsEnabled;

    private static KeySpline EaseSmooth() => new()
    {
        ControlPoint1 = new Point(0.22, 1),
        ControlPoint2 = new Point(0.36, 1),
    };

    /// <summary>Fade in + 8px rise on <paramref name="element"/>, beginning after <paramref name="delayMs"/>.</summary>
    public static void Entrance(FrameworkElement element, int delayMs = 0)
    {
        if (element is null || !AnimationsEnabled)
        {
            return;
        }

        var translate = new TranslateTransform { Y = 8 };
        element.RenderTransform = translate;
        element.Opacity = 0;

        var storyboard = new Storyboard { BeginTime = TimeSpan.FromMilliseconds(delayMs) };

        var fade = new DoubleAnimationUsingKeyFrames();
        fade.KeyFrames.Add(new SplineDoubleKeyFrame
        {
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(280)),
            Value = 1,
            KeySpline = EaseSmooth(),
        });
        Storyboard.SetTarget(fade, element);
        Storyboard.SetTargetProperty(fade, "Opacity");

        var rise = new DoubleAnimationUsingKeyFrames();
        rise.KeyFrames.Add(new SplineDoubleKeyFrame
        {
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(280)),
            Value = 0,
            KeySpline = EaseSmooth(),
        });
        Storyboard.SetTarget(rise, translate);
        Storyboard.SetTargetProperty(rise, "Y");

        storyboard.Children.Add(fade);
        storyboard.Children.Add(rise);
        storyboard.Begin();
    }
}
