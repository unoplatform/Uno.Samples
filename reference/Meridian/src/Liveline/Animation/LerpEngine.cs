namespace Liveline.Animation;

/// <summary>
/// Interpolation state machine that smoothly lerps animated values toward targets.
/// Y-axis range snaps outward instantly to prevent line clipping.
/// </summary>
public class LerpEngine
{
    private double[] _currentY = Array.Empty<double>();
    private double[] _targetY = Array.Empty<double>();
    private double _currentMinY;
    private double _currentMaxY;
    private double _targetMinY;
    private double _targetMaxY;
    private double _currentBadgeY;
    private double _targetBadgeY;

    public double[] CurrentY => _currentY;
    public double CurrentMinY => _currentMinY;
    public double CurrentMaxY => _currentMaxY;
    public double CurrentBadgeY => _currentBadgeY;
    public bool HasTargets => _targetY.Length > 0;

    /// <summary>
    /// Seeds a flat line at the given value so the breathing animation has geometry to draw.
    /// When real data arrives via SetTargets, the lerp will morph from this flat line.
    /// </summary>
    public void SeedFlatLine(int pointCount, double centerValue)
    {
        if (_currentY.Length == pointCount) return;

        _currentY = new double[pointCount];
        _targetY = new double[pointCount];
        Array.Fill(_currentY, centerValue);
        Array.Fill(_targetY, centerValue);

        _currentMinY = centerValue - 1;
        _currentMaxY = centerValue + 1;
        _targetMinY = _currentMinY;
        _targetMaxY = _currentMaxY;
        _currentBadgeY = centerValue;
        _targetBadgeY = centerValue;
    }

    public void SetTargets(double[] yValues, double minY, double maxY, double badgeY)
    {
        if (_targetY.Length != yValues.Length)
        {
            var newCurrentY = new double[yValues.Length];
            var newTargetY = new double[yValues.Length];

            for (int i = 0; i < yValues.Length; i++)
            {
                if (i < _currentY.Length)
                    newCurrentY[i] = _currentY[i];
                else
                    newCurrentY[i] = yValues[i];

                newTargetY[i] = yValues[i];
            }

            _currentY = newCurrentY;
            _targetY = newTargetY;

            if (_currentMinY == 0 && _currentMaxY == 0)
            {
                _currentMinY = minY;
                _currentMaxY = maxY;
                _currentBadgeY = badgeY;
            }
        }
        else
        {
            Array.Copy(yValues, _targetY, yValues.Length);
        }

        _targetMinY = minY;
        _targetMaxY = maxY;
        _targetBadgeY = badgeY;
    }

    /// <summary>
    /// Advances one frame of interpolation. Returns true if any value changed.
    /// </summary>
    public bool Tick(double speed)
    {
        if (_currentY.Length == 0) return false;

        bool changed = false;
        const double epsilon = 0.01;

        for (int i = 0; i < _currentY.Length; i++)
        {
            double diff = _targetY[i] - _currentY[i];
            if (Math.Abs(diff) > epsilon)
            {
                _currentY[i] += diff * speed;
                changed = true;
            }
            else if (_currentY[i] != _targetY[i])
            {
                _currentY[i] = _targetY[i];
                changed = true;
            }
        }

        // Y-axis range: snap outward instantly, lerp inward
        if (_targetMinY < _currentMinY)
        {
            _currentMinY = _targetMinY;
            changed = true;
        }
        else if (_targetMinY > _currentMinY)
        {
            double diff = _targetMinY - _currentMinY;
            if (Math.Abs(diff) > epsilon)
            {
                _currentMinY += diff * speed;
                changed = true;
            }
            else
                _currentMinY = _targetMinY;
        }

        if (_targetMaxY > _currentMaxY)
        {
            _currentMaxY = _targetMaxY;
            changed = true;
        }
        else if (_targetMaxY < _currentMaxY)
        {
            double diff = _targetMaxY - _currentMaxY;
            if (Math.Abs(diff) > epsilon)
            {
                _currentMaxY += diff * speed;
                changed = true;
            }
            else
                _currentMaxY = _targetMaxY;
        }

        {
            double diff = _targetBadgeY - _currentBadgeY;
            if (Math.Abs(diff) > epsilon)
            {
                _currentBadgeY += diff * speed;
                changed = true;
            }
            else if (_currentBadgeY != _targetBadgeY)
            {
                _currentBadgeY = _targetBadgeY;
                changed = true;
            }
        }

        return changed;
    }
}
