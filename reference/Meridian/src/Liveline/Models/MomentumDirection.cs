namespace Liveline.Models;

/// <summary>
/// Resolved momentum direction used internally by the renderer.
/// </summary>
public enum MomentumDirection
{
    Off,
    Up,
    Down,
    Flat
}

/// <summary>
/// Helpers to parse the Momentum property value.
/// Pass true for auto-detect, false for off, or "up"/"down"/"flat" to force.
/// </summary>
public static class MomentumHelper
{
    /// <summary>
    /// Resolves the Momentum property value into a concrete direction.
    /// </summary>
    public static MomentumDirection Resolve(object? value, double currentValue, double previousValue)
    {
        if (value is false)
            return MomentumDirection.Off;

        if (value is string s)
        {
            return s.ToLowerInvariant() switch
            {
                "up" => MomentumDirection.Up,
                "down" => MomentumDirection.Down,
                "flat" => MomentumDirection.Flat,
                _ => MomentumDirection.Off
            };
        }

        // true or any other truthy value = auto-detect
        if (value is true or not null)
        {
            double delta = currentValue - previousValue;
            double threshold = Math.Max(Math.Abs(currentValue) * 0.001, 0.001);

            if (delta > threshold) return MomentumDirection.Up;
            if (delta < -threshold) return MomentumDirection.Down;
            return MomentumDirection.Flat;
        }

        return MomentumDirection.Off;
    }
}
