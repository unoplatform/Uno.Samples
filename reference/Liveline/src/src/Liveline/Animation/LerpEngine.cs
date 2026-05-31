namespace Liveline.Animation;

public class LerpEngine
{
    private double[] _currentY = [];
    private double[] _targetY = [];
    private double _currentMinY;
    private double _currentMaxY;
    private double _targetMinY;
    private double _targetMaxY;
    private double _currentBadgeY;
    private double _targetBadgeY;
    private bool _initialized;

    public double[] CurrentY => _currentY;
    public double CurrentMinY => _currentMinY;
    public double CurrentMaxY => _currentMaxY;
    public double CurrentBadgeY => _currentBadgeY;
    public bool HasTargets => _targetY.Length > 0;

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
        _initialized = true;
    }

    public void SetTargets(double[] yValues, double minY, double maxY, double badgeY)
    {
        if (_targetY.Length != yValues.Length)
        {
            var newCurrentY = new double[yValues.Length];
            var newTargetY = new double[yValues.Length];

            for (int i = 0; i < yValues.Length; i++)
            {
                newCurrentY[i] = i < _currentY.Length ? _currentY[i] : yValues[i];
                newTargetY[i] = yValues[i];
            }

            _currentY = newCurrentY;
            _targetY = newTargetY;

            if (!_initialized)
            {
                _currentMinY = minY;
                _currentMaxY = maxY;
                _currentBadgeY = badgeY;
                _initialized = true;
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

        double badgeDiff = _targetBadgeY - _currentBadgeY;
        if (Math.Abs(badgeDiff) > epsilon)
        {
            _currentBadgeY += badgeDiff * speed;
            changed = true;
        }
        else if (_currentBadgeY != _targetBadgeY)
        {
            _currentBadgeY = _targetBadgeY;
            changed = true;
        }

        return changed;
    }
}
