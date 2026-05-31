using Liveline.Animation;
using Liveline.Helpers;
using Liveline.Models;
using Liveline.Rendering;
using SkiaSharp;
using Uno.WinUI.Graphics2DSK;
using Windows.Foundation;

namespace Liveline;

/// <summary>
/// The SkiaSharp drawing surface for <see cref="LivelineChart"/>. Receives state via
/// <see cref="UpdateState"/>, interpolates each frame through <see cref="TickAnimation"/>,
/// and renders grid, line, fill, badge, and momentum indicator.
/// </summary>
public class LivelineChartCanvas : SKCanvasElement
{
    private readonly LerpEngine _lerp = new();
    private RenderContext _rc = new();
    private ChartPalette _palette = ColorHelper.DerivePalette("#4CAF50", false);
    private DateTimeOffset[] _times = [];
    private double _currentValue;
    private double _previousValue;
    private bool _showGrid = true;
    private bool _showBadge = true;
    private bool _showFill = true;
    private MomentumDirection _momentumDirection = MomentumDirection.Off;
    private double _lerpSpeed = 0.04;
    private bool _isPaused;
    private bool _isLoading;
    private bool _hasData;
    private double _breathPhase;
    private const int BreathingPointCount = 60;

    /// <summary>Recreates native render resources after they were released on unload.</summary>
    public void EnsureResources()
    {
        if (_rc.IsDisposed)
            _rc = new RenderContext();
    }

    /// <summary>Disposes native Skia paints/fonts when the control leaves the visual tree.</summary>
    public void ReleaseResources() => _rc.Dispose();

    public void UpdateState(
        IList<LivelinePoint>? data,
        double value,
        LivelineTheme? theme,
        bool showGrid, bool showBadge, bool showFill,
        MomentumDirection momentum,
        double lerpSpeed,
        bool isPaused, bool isLoading)
    {
        _showGrid = showGrid;
        _showBadge = showBadge;
        _showFill = showFill;
        _lerpSpeed = lerpSpeed;
        _isPaused = isPaused;
        _isLoading = isLoading;

        var themeColor = theme?.Color ?? "#4CAF50";
        var isDark = theme?.IsDark ?? false;
        _palette = ColorHelper.DerivePalette(themeColor, isDark);

        if (data != null && data.Count > 0)
        {
            _previousValue = _currentValue;
            _currentValue = value;

            int count = data.Count;
            var yValues = new double[count];
            if (_times.Length != count)
                _times = new DateTimeOffset[count];

            double minY = double.MaxValue, maxY = double.MinValue;

            for (int i = 0; i < count; i++)
            {
                var v = data[i].Value;
                yValues[i] = v;
                _times[i] = data[i].Time;
                if (v < minY) minY = v;
                if (v > maxY) maxY = v;
            }

            double padding = (maxY - minY) * 0.05;
            if (padding < 0.01) padding = 1;
            minY -= padding;
            maxY += padding;

            _lerp.SetTargets(yValues, minY, maxY, value);
            _hasData = true;
        }

        _momentumDirection = MomentumHelper.Resolve(momentum, _currentValue, _previousValue);

        if (!_hasData && _isLoading)
            _lerp.SeedFlatLine(BreathingPointCount, 0.0);

        Invalidate();
    }

    public bool TickAnimation()
    {
        if (_isPaused) return false;

        if (!_hasData && _isLoading)
        {
            _breathPhase += 0.03;
            return true;
        }

        if (!_hasData) return false;

        return _lerp.Tick(_lerpSpeed);
    }

    protected override void RenderOverride(SKCanvas canvas, Size area)
    {
        if (_rc.IsDisposed) return;

        float w = (float)area.Width;
        float h = (float)area.Height;

        canvas.Clear(_palette.Background);

        if (!_hasData && _isLoading)
        {
            DrawBreathingLine(canvas, w, h);
            return;
        }

        if (_lerp.CurrentY.Length < 2)
        {
            DrawLoadingOrEmpty(canvas, w, h);
            return;
        }

        double minY = _lerp.CurrentMinY;
        double maxY = _lerp.CurrentMaxY;

        if (_showGrid)
            GridRenderer.Draw(canvas, w, h, minY, maxY, _times, _palette, _rc);

        double liveDotY = _lerp.CurrentY[^1];
        GridRenderer.DrawTrackingLine(canvas, w, h, liveDotY, minY, maxY, _palette, _rc);

        LineRenderer.Draw(canvas, w, h, _lerp.CurrentY, minY, maxY, _showFill, _palette, _rc);

        if (_momentumDirection != MomentumDirection.Off)
            MomentumRenderer.Draw(canvas, w, h, _lerp.CurrentY, minY, maxY, _palette, _rc);

        if (_showBadge)
            BadgeRenderer.Draw(canvas, w, h, _currentValue, _lerp.CurrentBadgeY, minY, maxY, _palette, _rc);

        if (_isLoading)
            DrawLoadingOrEmpty(canvas, w, h);
    }

    private void DrawBreathingLine(SKCanvas canvas, float w, float h)
    {
        var (left, right, top, bottom) = GridRenderer.GetChartArea(w, h);
        float chartHeight = bottom - top;
        float chartWidth = right - left;
        float centerY = top + chartHeight / 2f;

        float breathAmplitude = (float)(Math.Sin(_breathPhase) * 0.5 + 0.5);
        float waveHeight = 4f + breathAmplitude * 12f;
        byte alpha = (byte)(80 + breathAmplitude * 120);

        using var path = new SKPath();
        path.MoveTo(left, centerY);

        for (int i = 1; i < BreathingPointCount; i++)
        {
            float t = (float)i / (BreathingPointCount - 1);
            float x = left + t * chartWidth;
            float y = centerY + (float)Math.Sin(_breathPhase * 2.0 + t * Math.PI * 2.0) * waveHeight;

            float prevT = (float)(i - 1) / (BreathingPointCount - 1);
            float prevX = left + prevT * chartWidth;
            float prevY = centerY + (float)Math.Sin(_breathPhase * 2.0 + prevT * Math.PI * 2.0) * waveHeight;
            float cpOffset = (x - prevX) * 0.3f;

            path.CubicTo(prevX + cpOffset, prevY, x - cpOffset, y, x, y);
        }

        using var fillPath = new SKPath(path);
        fillPath.LineTo(right, bottom);
        fillPath.LineTo(left, bottom);
        fillPath.Close();

        using var gradient = SKShader.CreateLinearGradient(
            new SKPoint(0, centerY - waveHeight),
            new SKPoint(0, bottom),
            [ColorHelper.WithAlpha(_palette.LineColor, (byte)(alpha / 3)), ColorHelper.WithAlpha(_palette.LineColor, 0)],
            null,
            SKShaderTileMode.Clamp);

        _rc.Fill.Style = SKPaintStyle.Fill;
        _rc.Fill.Color = SKColors.White;
        _rc.Fill.Shader = gradient;
        canvas.DrawPath(fillPath, _rc.Fill);
        _rc.Fill.Shader = null;

        _rc.Stroke.Color = ColorHelper.WithAlpha(_palette.LineColor, alpha);
        canvas.DrawPath(path, _rc.Stroke);

        float dotX = left + chartWidth / 2f;
        float dotY = centerY + (float)Math.Sin(_breathPhase * 2.0 + Math.PI) * waveHeight;

        _rc.Fill.Color = ColorHelper.WithAlpha(_palette.LineColor, (byte)(30 + breathAmplitude * 50));
        canvas.DrawCircle(dotX, dotY, 8f, _rc.Fill);

        _rc.Fill.Color = ColorHelper.WithAlpha(_palette.LineColor, alpha);
        canvas.DrawCircle(dotX, dotY, 4f, _rc.Fill);
    }

    private void DrawLoadingOrEmpty(SKCanvas canvas, float w, float h)
    {
        _rc.Fill.Style = SKPaintStyle.Fill;
        _rc.Fill.Color = _palette.TextDim;

        string msg = _isLoading ? "Loading..." : "No data";
        canvas.DrawText(msg, w / 2, h / 2, SKTextAlign.Center, _rc.MessageFont, _rc.Fill);
    }
}
