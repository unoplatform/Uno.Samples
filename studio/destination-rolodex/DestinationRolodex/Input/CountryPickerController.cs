using DestinationRolodex.Models;

namespace DestinationRolodex.Input;

/// <summary>
/// Continuous unbounded picker position, damped spring, drag/wheel/keyboard stepping and selection detection.
/// Pure C#, ported from the approved prototype's picker logic. Time comes in as seconds from the caller's clock.
/// Owns no XAML and no photos. Selection authority: whoever subscribes to <see cref="SelectionChanged"/>.
/// </summary>
public sealed class CountryPickerController
{
    // Living Postcard reference spring: 115 × (target − q) − 21 × velocity, integrated in substeps of 1/120 s.
    private const double SpringStiffness = 115;
    private const double SpringDamping = 21;
    private const double SubStep = 1d / 120;
    private const double MaxCatchUpSeconds = .5;
    private const double WheelSnapDelaySeconds = .110;
    private const double TypeAheadTimeoutSeconds = .650;

    private sealed class DragState
    {
        public double StartX, StartY, StartPosition, LastY, LastTime, Speed;
        public bool Moved;
        public long? Row;
    }

    private DragState? _drag;
    private double? _wheelSnapDue;
    private int _index = -1;
    private bool _userChange;
    private double? _previousPosition;
    private string _typeAhead = string.Empty;
    private double _typeTime = double.NegativeInfinity;

    public int Count { get; private set; }
    public double Position { get; private set; }
    public double Target { get; private set; }
    public double Velocity { get; private set; }
    public double RowPitch { get; set; } = 76;
    public bool ReducedMotion { get; set; }

    /// <summary>Debug replay/scrub drives the position directly; the spring must not fight it.</summary>
    public bool IsFrozen { get; set; }

    public int LastDirection { get; private set; } = 1;
    public bool IsDragging => _drag is not null;
    public int SelectedIndex => _index;
    public bool IsSettled => !IsDragging && Math.Abs(Target - Position) <= .0008 && Math.Abs(Velocity) <= .003;

    /// <summary>Raised whenever the continuous position changes (rows need re-posing).</summary>
    public event Action? PositionChanged;

    /// <summary>Raised when the rounded position selects a different destination. Bool: user-initiated.</summary>
    public event Action<int, bool>? SelectionChanged;

    public void Reset(int count, int startIndex)
    {
        Count = Math.Max(0, count);
        _drag = null;
        _wheelSnapDue = null;
        Velocity = 0;
        Position = Target = Count == 0 ? 0 : Math.Clamp(startIndex, 0, Count - 1);
        _index = -1;
        _previousPosition = null;
        _userChange = false;
        PositionChanged?.Invoke();
        CheckSelection();
    }

    public void Step(int step, bool user)
    {
        if (Count == 0)
        {
            return;
        }

        LastDirection = Math.Sign(step) != 0 ? Math.Sign(step) : LastDirection;
        Target = CircularMath.RoundLikeJavaScript(Target) + step;
        _userChange = user;
        _wheelSnapDue = null;
        Normalize();
        if (ReducedMotion)
        {
            Position = Target;
            Velocity = 0;
            PositionChanged?.Invoke();
            CheckSelection();
        }
    }

    public void GoTo(int index)
    {
        if (Count == 0)
        {
            return;
        }

        if (index < 0 || index >= Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        var current = CircularMath.PositiveModulo(CircularMath.RoundLikeJavaScript(Target), Count);
        Step(CircularMath.ShortestDelta(current, index, Count), true);
    }

    /// <summary>CSS-scale wheel delta (100 per notch, positive = scroll down). Snaps 110 ms after the last event.</summary>
    public void Wheel(double cssDeltaY, double nowSeconds)
    {
        if (Count == 0)
        {
            return;
        }

        Target += Math.Clamp(cssDeltaY, -180, 180) / 115d;
        _userChange = true;
        _wheelSnapDue = nowSeconds + WheelSnapDelaySeconds;
        if (ReducedMotion)
        {
            Position = CircularMath.RoundLikeJavaScript(Target);
            Velocity = 0;
            PositionChanged?.Invoke();
            CheckSelection();
        }
    }

    public void DragStart(double x, double y, double nowSeconds, long? row)
    {
        if (Count == 0 || _drag is not null)
        {
            return;
        }

        _wheelSnapDue = null;
        _drag = new DragState { StartX = x, StartY = y, StartPosition = Position, LastY = y, LastTime = nowSeconds, Row = row };
        Velocity = 0;
        Target = Position;
    }

    public void DragMove(double x, double y, double nowSeconds)
    {
        if (_drag is null)
        {
            return;
        }

        var d = _drag;
        var dx = x - d.StartX;
        var dy = y - d.StartY;
        if (Math.Sqrt(dx * dx + dy * dy) > 5)
        {
            d.Moved = true;
        }

        var dt = Math.Max(nowSeconds - d.LastTime, .008);
        d.Speed = d.Speed * .55 + (-(y - d.LastY) / RowPitch / dt) * .45;
        Position = d.StartPosition - dy / RowPitch;
        Target = Position;
        _userChange = true;
        d.LastY = y;
        d.LastTime = nowSeconds;
        PositionChanged?.Invoke();
        CheckSelection();
    }

    /// <summary>Ends a drag. A short, unmoved press on a row selects that row; otherwise release momentum is bounded.</summary>
    public void DragEnd(double nowSeconds, bool cancelled)
    {
        if (_drag is null)
        {
            return;
        }

        var d = _drag;
        _drag = null;
        var speed = !cancelled && nowSeconds - d.LastTime < .095 ? Math.Clamp(d.Speed, -10, 10) : 0;
        Target = !cancelled && !d.Moved && d.Row is { } row
            ? row
            : CircularMath.RoundLikeJavaScript(Position + Math.Clamp(speed * .15, -1.6, 1.6));
        Velocity = cancelled ? 0 : speed * .3;
        if (ReducedMotion)
        {
            Position = Target;
            Velocity = 0;
            PositionChanged?.Invoke();
            CheckSelection();
        }

        Normalize();
    }

    /// <summary>Type-ahead: returns the matched index or -1. Letters typed within 650 ms accumulate.</summary>
    public int TypeAhead(char letter, IReadOnlyList<string> countries, double nowSeconds)
    {
        var l = char.ToLowerInvariant(letter).ToString();
        _typeAhead = nowSeconds - _typeTime < TypeAheadTimeoutSeconds ? _typeAhead + l : l;
        _typeTime = nowSeconds;
        for (var i = 0; i < countries.Count; i++)
        {
            if (countries[i].StartsWith(_typeAhead, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>Debug replay/scrub: place rows deterministically between two indices (smoothstep of progress).</summary>
    public void SyncPreview(int fromIndex, int toIndex, double progress)
    {
        if (Count == 0)
        {
            return;
        }

        var delta = CircularMath.ShortestDelta(fromIndex, toIndex, Count);
        Position = Target = fromIndex + delta * CircularMath.SmoothStep(progress);
        Velocity = 0;
        _userChange = true;
        PositionChanged?.Invoke();
        CheckSelection();
    }

    /// <summary>Diagnostics: places the continuous position directly (no spring), raising position and selection updates.</summary>
    public void SetPositionForDiagnostics(double q)
    {
        Position = Target = q;
        Velocity = 0;
        _userChange = false;
        PositionChanged?.Invoke();
        CheckSelection();
    }

    /// <summary>Leaves preview: rounds to the nearest row and settles there.</summary>
    public void SnapToNearest()
    {
        Position = Target = CircularMath.RoundLikeJavaScript(Position);
        Velocity = 0;
        PositionChanged?.Invoke();
        CheckSelection();
    }

    /// <summary>Advances the spring. Returns true while rows are still moving.</summary>
    public bool Tick(double dt, double nowSeconds)
    {
        if (Count == 0)
        {
            return false;
        }

        if (_wheelSnapDue is { } due && nowSeconds >= due)
        {
            _wheelSnapDue = null;
            Target = CircularMath.RoundLikeJavaScript(Target);
            Normalize();
        }

        if (IsFrozen || _drag is not null)
        {
            return false;
        }

        var moving = false;
        if (Math.Abs(Target - Position) > .0008 || Math.Abs(Velocity) > .003)
        {
            if (ReducedMotion)
            {
                Position = CircularMath.RoundLikeJavaScript(Target);
                Velocity = 0;
            }
            else
            {
                var remaining = Math.Min(dt, MaxCatchUpSeconds);
                while (remaining > 0)
                {
                    var h = Math.Min(SubStep, remaining);
                    var acceleration = (Target - Position) * SpringStiffness - Velocity * SpringDamping;
                    Velocity += acceleration * h;
                    Position += Velocity * h;
                    remaining -= h;
                }
            }

            moving = true;
            PositionChanged?.Invoke();
            CheckSelection();
        }
        else if (Position != Target && !ReducedMotion)
        {
            Position = Target;
            Velocity = 0;
            PositionChanged?.Invoke();
            CheckSelection();
        }

        return moving || _wheelSnapDue is not null;
    }

    private void Normalize()
    {
        var n = Count;
        if (n == 0)
        {
            return;
        }

        if (Math.Abs(Target) > n * 1000d)
        {
            var shift = Math.Truncate(Target / n) * n;
            Target -= shift;
            Position -= shift;
            if (_drag is not null)
            {
                _drag.StartPosition -= shift;
            }
        }
    }

    private void CheckSelection()
    {
        if (Count == 0)
        {
            return;
        }

        var motion = Math.Sign(Position - (_previousPosition ?? Position));
        if (motion != 0)
        {
            LastDirection = motion;
        }

        _previousPosition = Position;
        var index = CircularMath.SelectedIndex(Position, Count);
        if (index != _index)
        {
            _index = index;
            SelectionChanged?.Invoke(index, _userChange);
        }
    }
}
