using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.Foundation;

namespace EnterpriseDashboard.Observatory.Animation;

// Easing presets per Animation Brief §1.1. Spring/Bounce overshoot control points
// outside 0..1 are clamped here for use with SplineDoubleKeyFrame; Composition could
// express them exactly but Uno doesn't support that cleanly on Skia/desktop today.
public enum EasingPreset
{
    Smooth, // brief: (0.22, 0.61, 0.36, 1.0) — gentle deceleration
    Spring, // brief: (0.34, 1.56, 0.64, 1.0) — overshoot; clamped Spline approx via control points
    Bounce, // brief: (0.68, -0.55, 0.27, 1.55) — playful, used only on bars
    Snap,   // brief: (0.85, 0.0, 0.15, 1.0) — mechanical precision (Area clip wipe)
    Out     // brief: (0.16, 1.0, 0.30, 1.0) — gravity settle (Rainfall, Slope)
}

public static class LoopAnimations
{
    private static KeySpline Spline(EasingPreset ease) => ease switch
    {
        // Clamp Spring control points into 0..1 — produces no overshoot but still feels lively.
        EasingPreset.Spring => new KeySpline { ControlPoint1 = new Point(0.34, 1.0), ControlPoint2 = new Point(0.64, 1.0) },
        EasingPreset.Bounce => new KeySpline { ControlPoint1 = new Point(0.68, 0.0), ControlPoint2 = new Point(0.27, 1.0) },
        EasingPreset.Snap   => new KeySpline { ControlPoint1 = new Point(0.85, 0.0), ControlPoint2 = new Point(0.15, 1.0) },
        EasingPreset.Out    => new KeySpline { ControlPoint1 = new Point(0.16, 1.0), ControlPoint2 = new Point(0.30, 1.0) },
        _                   => new KeySpline { ControlPoint1 = new Point(0.22, 0.61), ControlPoint2 = new Point(0.36, 1.0) }
    };

    // Per-shape scalar loop. Per impeccable:animate, exit is faster than entrance (~75% by default).
    // We animate from→to (in), hold at to, then animate to→from (out) before cycle restart.
    public static DoubleAnimationUsingKeyFrames KeyFramesLoop(
        DependencyObject target, string property,
        double from, double to,
        double delayMs, double inMs, double cycleSec,
        EasingPreset easeIn = EasingPreset.Smooth,
        double outMs = -1)
    {
        if (outMs < 0) outMs = inMs * 0.6;
        var holdEnd = cycleSec * 1000.0 - outMs;

        var anim = new DoubleAnimationUsingKeyFrames();
        anim.KeyFrames.Add(new DiscreteDoubleKeyFrame { KeyTime = TimeSpan.Zero, Value = from });
        if (delayMs > 0)
            anim.KeyFrames.Add(new DiscreteDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(delayMs), Value = from });
        anim.KeyFrames.Add(new SplineDoubleKeyFrame
        {
            KeyTime = TimeSpan.FromMilliseconds(delayMs + inMs),
            Value = to,
            KeySpline = Spline(easeIn)
        });
        anim.KeyFrames.Add(new DiscreteDoubleKeyFrame
        {
            KeyTime = TimeSpan.FromMilliseconds(holdEnd),
            Value = to
        });
        // Smooth fade out — impeccable:animate principle (no abrupt snaps).
        anim.KeyFrames.Add(new SplineDoubleKeyFrame
        {
            KeyTime = TimeSpan.FromSeconds(cycleSec),
            Value = from,
            KeySpline = Spline(EasingPreset.Smooth)
        });
        Storyboard.SetTarget(anim, target);
        Storyboard.SetTargetProperty(anim, property);
        return anim;
    }

    public static Storyboard StaggeredFadeLoop(IList<DependencyObject> children, double staggerMs = 60, double cycleSec = 5.5, EasingPreset easeIn = EasingPreset.Smooth)
    {
        var sb = new Storyboard { RepeatBehavior = RepeatBehavior.Forever };
        for (int i = 0; i < children.Count; i++)
        {
            sb.Children.Add(KeyFramesLoop(
                children[i], "Opacity",
                0, 1, delayMs: i * staggerMs, inMs: 450, cycleSec: cycleSec, easeIn));
        }
        return sb;
    }

    public static Storyboard ScalarLoop(
        DependencyObject target, string property,
        double from, double to,
        double cycleSec = 5.5, double inSec = 1.2,
        EasingPreset easeIn = EasingPreset.Smooth)
    {
        var sb = new Storyboard { RepeatBehavior = RepeatBehavior.Forever };
        sb.Children.Add(KeyFramesLoop(target, property, from, to, 0, inSec * 1000, cycleSec, easeIn));
        return sb;
    }
}
