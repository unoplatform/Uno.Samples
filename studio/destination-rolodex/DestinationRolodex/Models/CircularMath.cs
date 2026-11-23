namespace DestinationRolodex.Models;

public readonly record struct RowPose(double Y, double Z, double RotationX, double Scale, double Opacity, bool Visible);

/// <summary>
/// Circular index arithmetic shared by the picker, indicators and tests.
/// Ported from the approved prototype; see qa/math-test-vectors.json in the handoff kit.
/// </summary>
public static class CircularMath
{
    public static int PositiveModulo(long value, int count)
    {
        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        return (int)(((value % count) + count) % count);
    }

    /// <summary>JavaScript Math.round: ties go toward positive infinity, unlike C# Math.Round's banker's rounding.</summary>
    public static long RoundLikeJavaScript(double position)
    {
        if (!double.IsFinite(position) || Math.Abs(position) > 1e12)
        {
            throw new ArgumentOutOfRangeException(nameof(position), "Normalize the circular position before it grows this large.");
        }

        return checked((long)Math.Floor(position + .5));
    }

    public static int SelectedIndex(double position, int count) => PositiveModulo(RoundLikeJavaScript(position), count);

    /// <summary>Equal forward/back distances resolve forward, matching the prototype's goTo().</summary>
    public static int ShortestDelta(int from, int to, int count)
    {
        if (count <= 0 || from < 0 || from >= count || to < 0 || to >= count)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        var delta = PositiveModulo((long)to - from, count);
        return 2L * delta > count ? delta - count : delta;
    }

    /// <summary>Row appearance for a row at signed distance <paramref name="distance"/> from the continuous position.</summary>
    public static RowPose Pose(double distance, double rowPitch)
    {
        if (!double.IsFinite(distance) || !double.IsFinite(rowPitch) || rowPitch <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(distance));
        }

        var a = Math.Abs(distance);
        var opacity = a <= 1 ? 1 - .5 * a : a <= 2 ? .5 - .34 * (a - 1) : .16 * (3 - a);
        return new RowPose(
            distance * rowPitch * (1 - Math.Min(a, 3) * .035),
            -a * 13,
            -distance * 12,
            Math.Max(.65, 1 - a * .044),
            Math.Clamp(opacity, 0, 1),
            a < 3);
    }

    /// <summary>Smoothstep used by the debug timeline to move rows between source and target positions.</summary>
    public static double SmoothStep(double t)
    {
        t = Math.Clamp(t, 0, 1);
        return t * t * (3 - 2 * t);
    }
}
