using System;

namespace EnterpriseDashboard.Observatory.Animation;

// CPU-side cubic-bezier easing evaluator for timer-driven chart animations.
public static class EasingCurves
{
    // CPU-side bezier evaluator for SkiaSharp timer-driven animations.
    public static double Evaluate(double t, double x1, double y1, double x2, double y2)
    {
        double s = t;
        for (int i = 0; i < 6; i++)
        {
            double cx = 3 * (1 - s) * (1 - s) * s * x1 + 3 * (1 - s) * s * s * x2 + s * s * s;
            double d = 3 * (1 - s) * (1 - s) * (x1 - 0) + 6 * (1 - s) * s * (x2 - x1) + 3 * s * s * (1 - x2);
            if (Math.Abs(d) < 1e-6) break;
            s -= (cx - t) / d;
            s = Math.Clamp(s, 0, 1);
        }
        return 3 * (1 - s) * (1 - s) * s * y1 + 3 * (1 - s) * s * s * y2 + s * s * s;
    }

    public static double SpringT(double t) => Evaluate(t, 0.34, 1.56, 0.64, 1.0);
    public static double SmoothT(double t) => Evaluate(t, 0.22, 0.61, 0.36, 1.0);
    public static double BounceT(double t) => Evaluate(t, 0.68, -0.55, 0.27, 1.55);
    public static double SnapT(double t) => Evaluate(t, 0.85, 0.0, 0.15, 1.0);
    public static double OutT(double t) => Evaluate(t, 0.16, 1.0, 0.30, 1.0);
}
