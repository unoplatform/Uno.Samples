using SkiaSharp;
using SkiaSharp.Views.Windows;

namespace Meridian.Controls;

public sealed class SectorRingControl : SKXamlCanvas
{
    // ── Theme colors ──────────────────────────────────────────────────
    private static readonly SKColor ColorTextPrimary = new(0x1A, 0x1A, 0x2E);
    private static readonly SKColor ColorTextMuted = new(0x8A, 0x8A, 0x8A);
    private static readonly SKColor ColorHighlight = new(0x2D, 0x6A, 0x4F, 15);

    // ── Dependency properties ─────────────────────────────────────────
    public static readonly DependencyProperty SectorsProperty =
        DependencyProperty.Register(nameof(Sectors), typeof(IList<Sector>),
            typeof(SectorRingControl), new PropertyMetadata(null, OnDataChanged));

    public IList<Sector>? Sectors
    {
        get => (IList<Sector>?)GetValue(SectorsProperty);
        set => SetValue(SectorsProperty, value);
    }

    // ── Hover state ───────────────────────────────────────────────────
    private int _hoveredIndex = -1;
    private readonly List<ArcHitZone> _arcZones = new();
    private readonly List<float> _legendYPositions = new();
    private float _legendX;
    private float _swatchAngle; // smooth-animated rotation for hovered swatch

    // ── Cached paints ─────────────────────────────────────────────────
    private readonly SKPaint _arcPaint;
    private readonly SKPaint _centerTextPaint;
    private readonly SKPaint _centerSubPaint;
    private readonly SKPaint _swatchPaint;
    private readonly SKPaint _legendNamePaint;
    private readonly SKPaint _legendPctPaint;
    private readonly SKPaint _highlightPaint;

    // ── Cached fonts ──────────────────────────────────────────────────
    private readonly SKFont _centerFontDefault;
    private readonly SKFont _centerFontHoveredSerif;
    private readonly SKFont _subFontDefault;
    private readonly SKFont _subFontHovered;
    private readonly SKFont _legendNameFontNormal;
    private readonly SKFont _legendNameFontBold;
    private readonly SKFont _legendPctFont;

    // ── Serif typeface (for hovered center data point) ───────────────
    private static SKTypeface? _serifTypeface;
    private static SKTypeface SerifTypeface => _serifTypeface ??= LoadSerifTypeface();

    private static SKTypeface LoadSerifTypeface()
    {
        var basePath = AppContext.BaseDirectory;
        var fontPath = Path.Combine(basePath, "Assets", "Fonts", "Instrument_Serif", "InstrumentSerif-Regular.ttf");
        if (File.Exists(fontPath)) return SKTypeface.FromFile(fontPath);
        return SKTypeface.FromFamilyName("Georgia") ?? SKTypeface.Default;
    }

    private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((SectorRingControl)d).Invalidate();

    public SectorRingControl()
    {
        // Arc paint (color set per segment in paint loop)
        _arcPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeCap = SKStrokeCap.Round,
        };

        // Center text paints
        _centerTextPaint = new SKPaint { IsAntialias = true, Color = ColorTextPrimary };
        _centerSubPaint = new SKPaint { IsAntialias = true, Color = ColorTextMuted };

        // Legend paints (color/alpha set per row)
        _swatchPaint = new SKPaint { IsAntialias = true };
        _legendNamePaint = new SKPaint { IsAntialias = true, Color = ColorTextPrimary };
        _legendPctPaint = new SKPaint { IsAntialias = true, Color = ColorTextMuted };
        _highlightPaint = new SKPaint { IsAntialias = true, Color = ColorHighlight, Style = SKPaintStyle.Fill };

        // Fonts
        var outfitSemiBold = SKTypeface.FromFamilyName("Outfit", SKFontStyleWeight.SemiBold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
        var outfitMedium = SKTypeface.FromFamilyName("Outfit", SKFontStyleWeight.Medium, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
        var outfitNormal = SKTypeface.FromFamilyName("Outfit", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
        var plexMono = SKTypeface.FromFamilyName("IBM Plex Mono");

        _centerFontDefault = new SKFont(outfitSemiBold, 16);
        _centerFontHoveredSerif = new SKFont(SerifTypeface, 16);
        _subFontDefault = new SKFont(outfitNormal, 9);
        _subFontHovered = new SKFont(outfitNormal, 10);
        _legendNameFontNormal = new SKFont(outfitMedium, 11);   // smaller + more weight
        _legendNameFontBold = new SKFont(outfitSemiBold, 12);
        _legendPctFont = new SKFont(plexMono, 11);              // match smaller size

        PaintSurface += OnPaintSurface;
        PointerMoved += OnPointerMoved;
        PointerExited += (_, _) => { _hoveredIndex = -1; Invalidate(); };
        Height = 200;
    }

    private void OnPointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var pos = e.GetCurrentPoint(this).Position;

        var newHovered = -1;
        for (int i = 0; i < _arcZones.Count; i++)
        {
            var z = _arcZones[i];
            var dx = pos.X - z.CenterX;
            var dy = pos.Y - z.CenterY;
            var dist = Math.Sqrt(dx * dx + dy * dy);
            if (dist < z.OuterRadius && dist > z.InnerRadius)
            {
                var angle = Math.Atan2(dy, dx) * 180 / Math.PI;
                if (angle < -90) angle += 360;
                if (angle >= z.StartAngle && angle < z.StartAngle + z.SweepAngle)
                {
                    newHovered = i;
                    break;
                }
            }
        }

        // Also check legend rows
        if (newHovered < 0)
        {
            for (int i = 0; i < _legendYPositions.Count; i++)
            {
                var ly = _legendYPositions[i];
                if (pos.Y >= ly - 15 && pos.Y <= ly + 15 && pos.X > _legendX)
                {
                    newHovered = i;
                    break;
                }
            }
        }

        if (newHovered != _hoveredIndex)
        {
            _hoveredIndex = newHovered;
            _swatchAngle = 0; // reset animation for new hovered item
            Invalidate();
        }
    }

    private record struct ArcHitZone(float CenterX, float CenterY, float InnerRadius, float OuterRadius, float StartAngle, float SweepAngle);

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        var sectors = Sectors;
        if (sectors is null || sectors.Count == 0) return;

        var w = e.Info.Width;
        var h = e.Info.Height;
        if (w <= 0 || h <= 0) return;

        var actualWidth = ActualWidth;
        if (actualWidth <= 0) actualWidth = 1;
        var scale = (float)(w / actualWidth);

        // Donut dimensions
        var ringSize = Math.Min(w * 0.42f, h * 0.8f);
        var centerX = ringSize / 2 + 20;
        var centerY = h / 2f;
        var outerRadius = ringSize / 2f;
        var strokeWidth = 22f;

        _arcZones.Clear();
        _legendYPositions.Clear();

        // Draw arcs
        float startAngle = -90f;
        for (int i = 0; i < sectors.Count; i++)
        {
            var sector = sectors[i];
            var sweepAngle = (float)(sector.Pct / 100.0 * 360.0);
            var color = SKColor.Parse(sector.ColorHex);
            var isHovered = i == _hoveredIndex;

            var expand = isHovered ? 4f : 0f;
            var sw = isHovered ? strokeWidth + 6 : strokeWidth;
            var alpha = (_hoveredIndex >= 0 && !isHovered) ? (byte)90 : (byte)255;

            var rect = new SKRect(
                centerX - outerRadius - expand + sw / 2,
                centerY - outerRadius - expand + sw / 2,
                centerX + outerRadius + expand - sw / 2,
                centerY + outerRadius + expand - sw / 2);

            _arcPaint.Color = color.WithAlpha(alpha);
            _arcPaint.StrokeWidth = sw;

            canvas.DrawArc(rect, startAngle, sweepAngle - 2, false, _arcPaint);

            // Store hit zone (in device-independent coords)
            _arcZones.Add(new ArcHitZone(
                centerX / scale, centerY / scale,
                (outerRadius - strokeWidth) / scale, (outerRadius + strokeWidth / 2) / scale,
                startAngle, sweepAngle));

            startAngle += sweepAngle;
        }

        // Center text — hovered: data point (serif) above category name
        var hasHover = _hoveredIndex >= 0 && _hoveredIndex < sectors.Count;
        if (hasHover)
        {
            var hoveredSector = sectors[_hoveredIndex];
            // Data point above (Instrument Serif)
            canvas.DrawText($"{hoveredSector.Pct:F1}%", centerX, centerY - 4,
                SKTextAlign.Center, _centerFontHoveredSerif, _centerTextPaint);
            // Category name below
            canvas.DrawText(hoveredSector.Name, centerX, centerY + 12,
                SKTextAlign.Center, _subFontHovered, _centerSubPaint);
        }
        else
        {
            canvas.DrawText($"{sectors.Count}", centerX, centerY - 4,
                SKTextAlign.Center, _centerFontDefault, _centerTextPaint);
            canvas.DrawText("SECTORS", centerX, centerY + 12,
                SKTextAlign.Center, _subFontDefault, _centerSubPaint);
        }

        // Legend (right side) — more spacing between rows
        var legendSpacing = 30f;
        var legendX = centerX + outerRadius + 36;
        _legendX = legendX / scale;
        var legendY = centerY - (sectors.Count * legendSpacing / 2f);

        // Animate swatch rotation toward target
        float swatchTarget = _hoveredIndex >= 0 ? 45f : 0f;
        _swatchAngle += (swatchTarget - _swatchAngle) * 0.18f;
        bool stillAnimating = Math.Abs(_swatchAngle - swatchTarget) > 0.5f;

        for (int i = 0; i < sectors.Count; i++)
        {
            var sector = sectors[i];
            var color = SKColor.Parse(sector.ColorHex);
            var isHovered = i == _hoveredIndex;
            var alpha = (_hoveredIndex >= 0 && !isHovered) ? (byte)90 : (byte)255;

            // Subtle lift on hovered row
            var rowY = isHovered ? legendY - 1.5f : legendY;

            // Highlight container behind hovered row (centered on text)
            if (isHovered)
            {
                canvas.DrawRoundRect(legendX - 10, rowY - 12, w - legendX + 4, 24, 6, 6, _highlightPaint);
            }

            // Color swatch (smooth rotation on hover — diamond effect)
            float swatchCx = legendX + 4;
            float swatchCy = rowY - 2;
            _swatchPaint.Color = color.WithAlpha(alpha);

            float angle = isHovered ? _swatchAngle : 0;
            if (angle > 0.5f)
            {
                canvas.Save();
                canvas.RotateDegrees(angle, swatchCx, swatchCy);
                canvas.DrawRoundRect(legendX, rowY - 6, 8, 8, 2, 2, _swatchPaint);
                canvas.Restore();
            }
            else
            {
                canvas.DrawRoundRect(legendX, rowY - 6, 8, 8, 2, 2, _swatchPaint);
            }

            var nameFont = isHovered ? _legendNameFontBold : _legendNameFontNormal;
            _legendNamePaint.Color = ColorTextPrimary.WithAlpha(alpha);
            canvas.DrawText(sector.Name, legendX + 16, rowY + 3, SKTextAlign.Left, nameFont, _legendNamePaint);

            _legendPctPaint.Color = ColorTextMuted.WithAlpha(alpha);
            canvas.DrawText($"{sector.Pct:F1}%", w - 10, rowY + 3, SKTextAlign.Right, _legendPctFont, _legendPctPaint);

            _legendYPositions.Add(legendY / scale);
            legendY += legendSpacing;
        }

        // Keep animating until swatch rotation converges
        if (stillAnimating)
            DispatcherQueue?.TryEnqueue(() => Invalidate());
    }
}
