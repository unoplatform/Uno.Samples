namespace DestinationRolodex.Input;

/// <summary>Cubic Bézier easing (CSS cubic-bezier) evaluated in C#, for the card's small frame-loop tweens.</summary>
public readonly struct CubicBezier
{
    private readonly double _x1, _y1, _x2, _y2;

    public CubicBezier(double x1, double y1, double x2, double y2)
    {
        _x1 = x1; _y1 = y1; _x2 = x2; _y2 = y2;
    }

    /// <summary>The prototype's house curve: cubic-bezier(.2, .7, .2, 1).</summary>
    public static CubicBezier Settle => new(.2, .7, .2, 1);
    public static CubicBezier Linear => new(0, 0, 1, 1);

    public double Evaluate(double t)
    {
        if (t <= 0) return 0;
        if (t >= 1) return 1;
        // Solve x(u) = t for u by bisection, then return y(u).
        double lo = 0, hi = 1, u = t;
        for (var i = 0; i < 24; i++)
        {
            u = (lo + hi) / 2;
            var x = Bezier(_x1, _x2, u);
            if (x < t) lo = u; else hi = u;
        }

        return Bezier(_y1, _y2, u);
    }

    private static double Bezier(double p1, double p2, double u)
    {
        var v = 1 - u;
        return 3 * v * v * u * p1 + 3 * v * u * u * p2 + u * u * u;
    }
}

/// <summary>A value moving from its current value to a target over a duration. Stepped from the frame loop; snaps when reduced.</summary>
public sealed class Tween
{
    private double _from, _to, _elapsed, _duration;
    private CubicBezier _curve = CubicBezier.Settle;

    public Tween(double initial)
    {
        _from = _to = initial;
        _duration = 0;
        _elapsed = 0;
    }

    public double Value { get; private set; }
    public bool IsRunning => _elapsed < _duration;

    public void Snap(double value)
    {
        _from = _to = Value = value;
        _duration = 0;
        _elapsed = 0;
    }

    public void Go(double target, double durationSeconds, CubicBezier? curve = null)
    {
        if (durationSeconds <= 0)
        {
            Snap(target);
            return;
        }

        _from = Value;
        _to = target;
        _duration = durationSeconds;
        _elapsed = 0;
        _curve = curve ?? CubicBezier.Settle;
    }

    /// <summary>Restarts from an explicit start value (the detail copy entrance starts at .22 every time).</summary>
    public void Restart(double from, double target, double durationSeconds, CubicBezier? curve = null)
    {
        Value = from;
        Go(target, durationSeconds, curve);
    }

    public bool Tick(double dt)
    {
        if (!IsRunning)
        {
            Value = _to;
            return false;
        }

        _elapsed += dt;
        var t = Math.Clamp(_elapsed / _duration, 0, 1);
        Value = _from + (_to - _from) * _curve.Evaluate(t);
        return IsRunning;
    }
}
