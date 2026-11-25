using DestinationRolodex.Rendering;

namespace DestinationRolodex.Input;

/// <summary>
/// Constrained flat-image camera: damped pan, zoom and tilt over the overscanned photo layer. It does not model
/// hidden geometry. Browse amplitude ±12 px / ±1.25° / ±1.6°; Look around ±37 px / ±2.8° / ±3.8°; zoom 1–1.65.
/// </summary>
public sealed class CameraController
{
    private double _panX, _panY, _zoom = 1, _phase;

    public double TargetX { get; private set; }
    public double TargetY { get; private set; }
    public double TargetZoom { get; private set; } = 1;
    public double Zoom => _zoom;
    public bool IsLooking { get; set; }
    public bool ReducedMotion { get; set; }
    /// <summary>Ambient drift while the tour plays (±3 px, slow sine); off when reduced or looking.</summary>
    public bool IsDrifting { get; set; }
    public CameraPose Pose { get; private set; } = CameraPose.Identity;

    /// <summary>Normalized pointer position over the scene, -1..1 on both axes (browse parallax).</summary>
    public void SetPointer(double nx, double ny)
    {
        TargetX = Math.Clamp(nx, -1, 1);
        TargetY = Math.Clamp(ny, -1, 1);
    }

    public void ClearPointer()
    {
        TargetX = 0;
        TargetY = 0;
    }

    /// <summary>Look-around drag: 145 px of pointer travel equals the full ±1 range.</summary>
    public void PanBy(double startTargetX, double startTargetY, double dx, double dy)
    {
        TargetX = Math.Clamp(startTargetX + dx / 145d, -1, 1);
        TargetY = Math.Clamp(startTargetY + dy / 145d, -1, 1);
    }

    public void Nudge(double dx, double dy)
    {
        TargetX = Math.Clamp(TargetX + dx, -1, 1);
        TargetY = Math.Clamp(TargetY + dy, -1, 1);
    }

    public void SetZoom(double zoom) => TargetZoom = Math.Clamp(zoom, 1, 1.65);

    public void Reset()
    {
        TargetX = TargetY = 0;
        TargetZoom = 1;
        if (ReducedMotion)
        {
            _panX = _panY = 0;
            _zoom = 1;
        }
    }

    /// <summary>Advances damping. Returns true while the camera is still settling.</summary>
    public bool Tick(double dt)
    {
        var damp = 1 - Math.Exp(-dt * 6);
        _panX += (TargetX - _panX) * damp;
        _panY += (TargetY - _panY) * damp;
        _zoom += (TargetZoom - _zoom) * damp;
        var moving = Math.Abs(_panX - TargetX) + Math.Abs(_panY - TargetY) + Math.Abs(_zoom - TargetZoom) > .0008;

        _phase += dt;
        var drifting = IsDrifting && !ReducedMotion && !IsLooking;
        var driftX = drifting ? Math.Sin(_phase * .16) * 3 : 0;
        var driftY = drifting ? Math.Cos(_phase * .13) * 3 : 0;
        var amplitude = IsLooking ? 37 : 12;
        var x = (ReducedMotion && !IsLooking ? 0 : _panX * amplitude) + driftX;
        var y = (ReducedMotion && !IsLooking ? 0 : _panY * amplitude) + driftY;
        var rx = ReducedMotion ? 0 : -_panY * (IsLooking ? 2.8 : 1.25);
        var ry = ReducedMotion ? 0 : _panX * (IsLooking ? 3.8 : 1.6);
        Pose = new CameraPose((float)x, (float)y, (float)rx, (float)ry, (float)_zoom);
        return moving || drifting;
    }
}
