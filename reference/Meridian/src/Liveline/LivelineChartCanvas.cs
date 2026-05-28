using Liveline.Animation;
using Liveline.Helpers;
using Liveline.Models;
using Liveline.Rendering;
using SkiaSharp;
using Uno.WinUI.Graphics2DSK;
using Windows.Foundation;

namespace Liveline;

/// <summary>
/// SKCanvasElement subclass that performs all Skia drawing for the chart.
/// State is captured into plain fields (safe for the render thread)
/// and updated from the UI thread via UpdateState().
/// </summary>
public class LivelineChartCanvas : SKCanvasElement
{
    private readonly LerpEngine _lerp = new();
    private ChartPalette _palette = ColorHelper.DerivePalette("#4CAF50", false);
    private DateTimeOffset[] _times = Array.Empty<DateTimeOffset>();
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
    private float _fillOpacity = 1.0f;
    private float _crosshairX = float.NaN;
    private float _baselineY = float.NaN;

    /// <summary>
    /// Multiplier for fill gradient top alpha (1.0 = default, higher = denser).
    /// </summary>
    public float FillOpacity
    {
        get => _fillOpacity;
        set { _fillOpacity = value; Invalidate(); }
    }

    /// <summary>
    /// X position for the crosshair in logical pixels. NaN = hidden.
    /// </summary>
    public float CrosshairX
    {
        get => _crosshairX;
        set { _crosshairX = value; Invalidate(); }
    }

    /// <summary>
    /// Y value for the average cost baseline. NaN = hidden.
    /// </summary>
    public float BaselineY
    {
        get => _baselineY;
        set { _baselineY = value; Invalidate(); }
    }

    public void UpdateState(
        IList<LivelinePoint>? data,
        double value,
        LivelineTheme? theme,
        bool showGrid, bool showBadge, bool showFill,
        object? momentum,
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

            var yValues = new double[data.Count];
            var times = new DateTimeOffset[data.Count];
            double minY = double.MaxValue, maxY = double.MinValue;

            for (int i = 0; i < data.Count; i++)
            {
                yValues[i] = data[i].Value;
                times[i] = data[i].Time;
                if (data[i].Value < minY) minY = data[i].Value;
                if (data[i].Value > maxY) maxY = data[i].Value;
            }

            double padding = (maxY - minY) * 0.05;
            if (padding < 0.01) padding = 1;
            minY -= padding;
            maxY += padding;

            _times = times;
            _lerp.SetTargets(yValues, minY, maxY, value);
            _hasData = true;
        }

        _momentumDirection = MomentumHelper.Resolve(momentum, _currentValue, _previousValue);

        // Seed a flat line for the breathing animation when no real data yet
        if (!_hasData && _isLoading)
        {
            _lerp.SeedFlatLine(BreathingPointCount, 0.0);
        }

        Invalidate();
    }

    /// <summary>
    /// Called every frame from CompositionTarget.Rendering.
    /// Always ticks the lerp and returns true if a redraw is needed.
    /// </summary>
    public bool TickAnimation()
    {
        if (_isPaused) return false;

        // Breathing animation while loading (before data arrives)
        if (!_hasData && _isLoading)
        {
            _breathPhase += 0.03;
            return true; // always redraw during breathing
        }

        if (!_hasData) return false;

        // Always tick - lerp converges smoothly over many frames
        return _lerp.Tick(_lerpSpeed);
    }

    protected override void RenderOverride(SKCanvas canvas, Size area)
    {
        float w = (float)area.Width;
        float h = (float)area.Height;

        canvas.Clear(_palette.Background);

        // Breathing line animation while waiting for data
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

        // 1. Grid labels
        if (_showGrid)
            GridRenderer.Draw(canvas, w, h, minY, maxY, _times, _palette);

        // 2. Horizontal tracking line at the live dot Y
        double liveDotY = _lerp.CurrentY[^1];
        GridRenderer.DrawTrackingLine(canvas, w, h, liveDotY, minY, maxY, _palette);

        // 2b. Average cost baseline
        if (!float.IsNaN(_baselineY))
            BaselineRenderer.Draw(canvas, w, h, _baselineY, minY, maxY, _palette);

        // 3. Line + fill (apply Weight Whisper fill opacity modifier)
        var renderPalette = _fillOpacity == 1.0f ? _palette : _palette with
        {
            FillTop = ColorHelper.WithAlpha(_palette.FillTop,
                (byte)Math.Clamp(_palette.FillTop.Alpha * _fillOpacity, 0, 255))
        };
        LineRenderer.Draw(canvas, w, h, _lerp.CurrentY, minY, maxY, _showFill, renderPalette);

        // 4. Live dot + momentum arrow
        MomentumRenderer.Draw(canvas, w, h, _lerp.CurrentY, minY, maxY, _momentumDirection, _palette);

        // 5. Badge (drawn last so it overlays everything)
        if (_showBadge)
            BadgeRenderer.Draw(canvas, w, h, _currentValue, _lerp.CurrentBadgeY, minY, maxY, _palette);

        // 6. Crosshair + tooltip (drawn on top of everything)
        if (!float.IsNaN(_crosshairX))
            CrosshairRenderer.Draw(canvas, w, h, _lerp.CurrentY, _times, minY, maxY, _crosshairX, _palette);

        if (_isLoading)
            DrawLoadingOrEmpty(canvas, w, h);
    }

    private void DrawBreathingLine(SKCanvas canvas, float w, float h)
    {
        var (left, right, top, bottom) = GridRenderer.GetChartArea(w, h);
        float chartHeight = bottom - top;
        float chartWidth = right - left;
        float centerY = top + chartHeight / 2f;

        // Breathing: a gentle sine wave that pulses in amplitude and opacity
        float breathAmplitude = (float)(Math.Sin(_breathPhase) * 0.5 + 0.5); // 0..1
        float waveHeight = 4f + breathAmplitude * 12f; // 4..16px
        byte alpha = (byte)(80 + breathAmplitude * 120); // 80..200

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

        // Gradient fill under breathing line
        using var fillPath = new SKPath(path);
        fillPath.LineTo(right, bottom);
        fillPath.LineTo(left, bottom);
        fillPath.Close();

        using var fillPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Shader = SKShader.CreateLinearGradient(
                new SKPoint(0, centerY - waveHeight),
                new SKPoint(0, bottom),
                new[] { ColorHelper.WithAlpha(_palette.LineColor, (byte)(alpha / 3)), ColorHelper.WithAlpha(_palette.LineColor, (byte)0) },
                null,
                SKShaderTileMode.Clamp)
        };
        canvas.DrawPath(fillPath, fillPaint);

        // Line stroke
        using var linePaint = new SKPaint
        {
            Color = ColorHelper.WithAlpha(_palette.LineColor, alpha),
            StrokeWidth = 2.5f,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };
        canvas.DrawPath(path, linePaint);

        // Pulsing dot at center
        float dotX = left + chartWidth / 2f;
        float dotY = centerY + (float)Math.Sin(_breathPhase * 2.0 + Math.PI) * waveHeight;
        float dotAlpha = breathAmplitude;

        using var dotGlow = new SKPaint
        {
            Color = ColorHelper.WithAlpha(_palette.LineColor, (byte)(30 + dotAlpha * 50)),
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawCircle(dotX, dotY, 8f, dotGlow);

        using var dotPaint = new SKPaint
        {
            Color = ColorHelper.WithAlpha(_palette.LineColor, alpha),
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawCircle(dotX, dotY, 4f, dotPaint);
    }

    private void DrawLoadingOrEmpty(SKCanvas canvas, float w, float h)
    {
        using var font = new SKFont(SKTypeface.FromFamilyName("Segoe UI", SKFontStyle.Normal), 14f);
        using var textPaint = new SKPaint
        {
            Color = _palette.TextDim,
            IsAntialias = true
        };

        string msg = _isLoading ? "Loading..." : "No data";
        canvas.DrawText(msg, w / 2, h / 2, SKTextAlign.Center, font, textPaint);
    }
}
