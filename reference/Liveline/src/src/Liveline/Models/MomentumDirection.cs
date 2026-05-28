namespace Liveline.Models;

/// <summary>
/// Controls the momentum indicator drawn on the live dot of a <see cref="LivelineChart"/>.
/// </summary>
public enum MomentumDirection
{
    /// <summary>No momentum indicator.</summary>
    Off,
    /// <summary>Auto-detect the direction from the change between the last two values.</summary>
    Auto,
    /// <summary>Force an upward indicator.</summary>
    Up,
    /// <summary>Force a downward indicator.</summary>
    Down,
    /// <summary>Force a flat (neutral) indicator.</summary>
    Flat
}

internal static class MomentumHelper
{
    /// <summary>
    /// Resolves a <see cref="MomentumDirection"/> mode into a concrete render direction.
    /// <see cref="MomentumDirection.Auto"/> derives the direction from the delta between the
    /// current and previous values; all other modes pass through unchanged.
    /// </summary>
    public static MomentumDirection Resolve(MomentumDirection mode, double currentValue, double previousValue)
    {
        if (mode != MomentumDirection.Auto)
            return mode;

        double delta = currentValue - previousValue;
        double threshold = Math.Max(Math.Abs(currentValue) * 0.001, 0.001);

        if (delta > threshold) return MomentumDirection.Up;
        if (delta < -threshold) return MomentumDirection.Down;
        return MomentumDirection.Flat;
    }
}
