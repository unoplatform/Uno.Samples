using SkiaSharp;
using SkiaSharp.Views.Windows;

namespace Meridian.Controls;

public sealed class VolumeChartControl : SKXamlCanvas
{
    // ── Theme colors ──────────────────────────────────────────────────
    private static readonly SKColor ColorTextPrimary = new(0x1A, 0x1A, 0x2E);
    private static readonly SKColor ColorTextSubtle = new(0xC4, 0xC0, 0xB8);
    private static readonly SKColor ColorBorder = new(0xE8, 0xE4, 0xDE);
    private static readonly SKColor ColorAccent = new(0xC9, 0xA9, 0x6E);

    // ── Dependency properties ─────────────────────────────────────────
    public static readonly DependencyProperty VolumeDataProperty =
        DependencyProperty.Register(nameof(VolumeData), typeof(IList<VolumeBar>),
            typeof(VolumeChartControl), new PropertyMetadata(null, OnDataChanged));

    public IList<VolumeBar>? VolumeData
    {
        get => (IList<VolumeBar>?)GetValue(VolumeDataProperty);
        set => SetValue(VolumeDataProperty, value);
    }

    // ── Hover state ───────────────────────────────────────────────────
    private int _hoveredBar = -1;
    private float _barGap;
    private float _paddingLeft;

    // ── Cached paints ─────────────────────────────────────────────────
    private readonly SKPaint _gridPaint;
    private readonly SKPaint _zonePaint;
    private readonly SKPaint _barPaintHigh;
    private readonly SKPaint _barPaintNormal;
    private readonly SKPaint _envelopePaint;
    private readonly SKPaint _labelPaint;
    private readonly SKPaint _crosshairPaint;
    private readonly SKPaint _dotPaint;
    private readonly SKPaint _tipBgPaint;
    private readonly SKPaint _tipTextPaint;

    // ── Cached fonts ──────────────────────────────────────────────────
    private readonly SKFont _labelFont;
    private readonly SKFont _tipFont;

    private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((VolumeChartControl)d).Invalidate();

    public VolumeChartControl()
    {
        // Grid lines
        _gridPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            Color = ColorBorder,
            StrokeWidth = 1,
            PathEffect = SKPathEffect.CreateDash(new[] { 4f, 4f }, 0),
        };

        // Market hours zone
        _zonePaint = new SKPaint
        {
            IsAntialias = true,
            Color = ColorAccent.WithAlpha(12),
        };

        // Bar paints
        _barPaintHigh = new SKPaint
        {
            IsAntialias = true,
            Color = ColorAccent.WithAlpha(180),
        };

        _barPaintNormal = new SKPaint
        {
            IsAntialias = true,
            Color = ColorTextSubtle.WithAlpha(180),
        };

        // Envelope line
        _envelopePaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            Color = ColorAccent.WithAlpha(120),
            StrokeWidth = 1.5f,
        };

        // X-axis labels
        _labelPaint = new SKPaint
        {
            IsAntialias = true,
            Color = ColorTextSubtle,
        };

        // Crosshair
        _crosshairPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            Color = ColorAccent.WithAlpha(150),
            StrokeWidth = 1,
            PathEffect = SKPathEffect.CreateDash(new[] { 3f, 3f }, 0),
        };

        // Dot on envelope
        _dotPaint = new SKPaint
        {
            IsAntialias = true,
            Color = ColorAccent,
        };

        // Tooltip
        _tipBgPaint = new SKPaint
        {
            IsAntialias = true,
            Color = ColorTextPrimary,
        };

        _tipTextPaint = new SKPaint
        {
            IsAntialias = true,
            Color = SKColors.White,
        };

        _labelFont = new SKFont(SKTypeface.FromFamilyName("Outfit"), 9);
        _tipFont = new SKFont(SKTypeface.FromFamilyName("IBM Plex Mono"), 10);

        PaintSurface += OnPaintSurface;
        PointerMoved += OnPointerMoved;
        PointerExited += (_, _) => { _hoveredBar = -1; Invalidate(); };
        Height = 140;
    }

    private void OnPointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var data = VolumeData;
        if (data is null || data.Count == 0 || _barGap == 0) return;

        var pos = e.GetCurrentPoint(this).Position;
        var actualWidth = ActualWidth > 0 ? ActualWidth : 1;
        var idx = (int)((pos.X - _paddingLeft / actualWidth * actualWidth) / (actualWidth / data.Count));
        idx = Math.Clamp(idx, 0, data.Count - 1);
        if (idx != _hoveredBar)
        {
            _hoveredBar = idx;
            Invalidate();
        }
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        var data = VolumeData;
        if (data is null || data.Count == 0) return;

        var w = e.Info.Width;
        var h = e.Info.Height;
        if (w <= 0 || h <= 0) return;

        var padding = new SKRect(30, 10, 10, 25);
        var chartW = w - padding.Left - padding.Right;
        var chartH = h - padding.Top - padding.Bottom;
        if (chartW <= 0 || chartH <= 0) return;

        var maxVol = data.Max(d => d.Volume);
        if (maxVol == 0) maxVol = 1;

        var barWidth = chartW / data.Count * 0.6f;
        var barGap = chartW / data.Count;

        // Grid lines at 25%, 50%, 75%
        for (int pct = 25; pct <= 75; pct += 25)
        {
            var y = padding.Top + chartH * (1 - pct / 100f);
            canvas.DrawLine(padding.Left, y, w - padding.Right, y, _gridPaint);
        }

        // Market hours zone (9-16 = indices 9-16)
        var marketStart = padding.Left + 9 * barGap;
        var marketEnd = padding.Left + 16 * barGap + barWidth;
        canvas.DrawRect(marketStart, padding.Top, marketEnd - marketStart, chartH, _zonePaint);

        // Bars
        var envelopePoints = new List<SKPoint>(data.Count);

        for (int i = 0; i < data.Count; i++)
        {
            var vol = data[i].Volume;
            var barH = (float)vol / maxVol * chartH;
            var x = padding.Left + i * barGap;
            var y = padding.Top + chartH - barH;

            // Color: gold for high volume (>55M), subtle otherwise
            var isHigh = vol > 55;
            var barPaint = isHigh ? _barPaintHigh : _barPaintNormal;

            var barRect = new SKRoundRect(
                new SKRect(x, y, x + barWidth, padding.Top + chartH),
                barWidth / 2, barWidth / 2);
            canvas.DrawRoundRect(barRect, barPaint);

            // Envelope point at bar top
            envelopePoints.Add(new SKPoint(x + barWidth / 2, y));
        }

        // Envelope line (smooth)
        if (envelopePoints.Count >= 2)
        {
            var envelopePath = new SKPath();
            envelopePath.MoveTo(envelopePoints[0]);
            for (int i = 1; i < envelopePoints.Count; i++)
            {
                var prev = envelopePoints[i - 1];
                var curr = envelopePoints[i];
                var cpx = (prev.X + curr.X) / 2;
                envelopePath.CubicTo(cpx, prev.Y, cpx, curr.Y, curr.X, curr.Y);
            }

            canvas.DrawPath(envelopePath, _envelopePaint);
            envelopePath.Dispose();
        }

        // X-axis labels (every 6 hours)
        for (int i = 0; i < data.Count; i += 6)
        {
            var x = padding.Left + i * barGap + barWidth / 2;
            canvas.DrawText($"{i}:00", x, h - 5, SKTextAlign.Center, _labelFont, _labelPaint);
        }

        _barGap = barGap;
        _paddingLeft = padding.Left;

        // Hover tooltip
        if (_hoveredBar >= 0 && _hoveredBar < data.Count)
        {
            var hx = padding.Left + _hoveredBar * barGap + barWidth / 2;

            // Crosshair vertical line
            canvas.DrawLine(hx, padding.Top, hx, padding.Top + chartH, _crosshairPaint);

            // Dot on envelope
            if (_hoveredBar < envelopePoints.Count)
            {
                canvas.DrawCircle(envelopePoints[_hoveredBar], 4, _dotPaint);
            }

            // Tooltip box
            var vol = data[_hoveredBar].Volume;
            var label = $"{_hoveredBar}:00 · {vol}M";

            var tipW = 90f;
            var tipH = 22f;
            var tipX = Math.Clamp(hx - tipW / 2, 0, w - tipW);
            // Position tooltip inside chart area (not above, which gets clipped)
            var barTopY = _hoveredBar < envelopePoints.Count ? envelopePoints[_hoveredBar].Y : padding.Top + 20;
            var tipY = Math.Max(padding.Top + 4, barTopY - tipH - 8);

            canvas.DrawRoundRect(tipX, tipY, tipW, tipH, 6, 6, _tipBgPaint);
            canvas.DrawText(label, tipX + tipW / 2, tipY + 15, SKTextAlign.Center, _tipFont, _tipTextPaint);
        }
    }
}
