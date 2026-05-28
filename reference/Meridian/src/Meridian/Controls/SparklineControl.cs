using SkiaSharp;
using SkiaSharp.Views.Windows;

namespace Meridian.Controls;

public sealed class SparklineControl : SKXamlCanvas
{
    // ── Theme colors ──────────────────────────────────────────────────
    private static readonly SKColor ColorGain = new(0x2D, 0x6A, 0x4F);
    private static readonly SKColor ColorLoss = new(0xB5, 0x34, 0x2B);

    // ── Dependency properties ─────────────────────────────────────────
    public static readonly DependencyProperty PointsProperty =
        DependencyProperty.Register(nameof(Points), typeof(IList<double>),
            typeof(SparklineControl), new PropertyMetadata(null, OnDataChanged));

    public static readonly DependencyProperty IsPositiveProperty =
        DependencyProperty.Register(nameof(IsPositive), typeof(bool),
            typeof(SparklineControl), new PropertyMetadata(true, OnDataChanged));

    public IList<double>? Points
    {
        get => (IList<double>?)GetValue(PointsProperty);
        set => SetValue(PointsProperty, value);
    }

    public bool IsPositive
    {
        get => (bool)GetValue(IsPositiveProperty);
        set => SetValue(IsPositiveProperty, value);
    }

    // ── Cached paints ─────────────────────────────────────────────────
    private readonly SKPaint _strokePaint;
    private readonly SKPaint _fillPaint;
    private bool _lastIsPositive = true;

    private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((SparklineControl)d).Invalidate();

    public SparklineControl()
    {
        _strokePaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            Color = ColorGain,
            StrokeWidth = 1.5f,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
        };

        _fillPaint = new SKPaint
        {
            IsAntialias = true,
        };

        PaintSurface += OnPaintSurface;
        Width = 72;
        Height = 30;
    }

    private void RebuildPaintsForPolarity(bool isPositive, float height)
    {
        var color = isPositive ? ColorGain : ColorLoss;

        _strokePaint.Color = color;

        _fillPaint.Shader?.Dispose();
        _fillPaint.Shader = SKShader.CreateLinearGradient(
            new SKPoint(0, 0), new SKPoint(0, height),
            new[] { color.WithAlpha(50), color.WithAlpha(0) },
            SKShaderTileMode.Clamp);

        _lastIsPositive = isPositive;
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        var points = Points;
        if (points is null || points.Count < 2) return;

        var w = e.Info.Width;
        var h = e.Info.Height;
        if (w <= 0 || h <= 0) return;

        var padding = 2f;
        var isPositive = IsPositive;

        // Rebuild paints only when polarity changes (gradient needs height too)
        if (isPositive != _lastIsPositive)
        {
            RebuildPaintsForPolarity(isPositive, h);
        }
        else
        {
            // Always refresh gradient when height changes (cheap check)
            RebuildPaintsForPolarity(isPositive, h);
        }

        var min = points.Min();
        var max = points.Max();
        var range = max - min;
        if (range == 0) range = 1;

        var path = new SKPath();
        for (int i = 0; i < points.Count; i++)
        {
            var x = padding + (float)i / (points.Count - 1) * (w - padding * 2);
            var y = padding + (float)(h - padding * 2 - (points[i] - min) / range * (h - padding * 2));
            if (i == 0) path.MoveTo(x, y);
            else path.LineTo(x, y);
        }

        // Area fill
        var areaPath = new SKPath(path);
        areaPath.LineTo(w - padding, h);
        areaPath.LineTo(padding, h);
        areaPath.Close();

        canvas.DrawPath(areaPath, _fillPaint);
        canvas.DrawPath(path, _strokePaint);

        areaPath.Dispose();
        path.Dispose();
    }
}
