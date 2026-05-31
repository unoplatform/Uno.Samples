using Liveline.Helpers;
using SkiaSharp;

namespace Liveline.Rendering;

/// <summary>
/// Draws an interactive crosshair, snapping to the nearest data point
/// with a tooltip pill showing the value and a date label above.
/// </summary>
public static class CrosshairRenderer
{
    // Cache typefaces to avoid repeated lookups
    private static SKTypeface? _serifTypeface;
    private static SKTypeface? _sansTypeface;

    private static SKTypeface SerifTypeface => _serifTypeface ??= LoadSerifTypeface();
    private static SKTypeface SansTypeface => _sansTypeface ??= SKTypeface.FromFamilyName("Segoe UI", SKFontStyle.Normal);

    private static SKTypeface LoadSerifTypeface()
    {
        // Load Instrument Serif from the app's font assets
        var basePath = AppContext.BaseDirectory;
        var fontPath = Path.Combine(basePath, "Assets", "Fonts", "Instrument_Serif", "InstrumentSerif-Regular.ttf");
        if (File.Exists(fontPath))
            return SKTypeface.FromFile(fontPath);

        // Fallback: try system serif fonts
        return SKTypeface.FromFamilyName("Georgia") ?? SKTypeface.Default;
    }

    public static void Draw(
        SKCanvas canvas,
        float width, float height,
        double[] yValues,
        DateTimeOffset[] times,
        double minY, double maxY,
        float crosshairX,
        ChartPalette palette)
    {
        if (yValues.Length < 2 || float.IsNaN(crosshairX)) return;

        var (left, right, top, bottom) = GridRenderer.GetChartArea(width, height);
        float chartWidth = right - left;
        float chartHeight = bottom - top;
        double range = maxY - minY;
        if (range <= 0) range = 1;

        // Clamp crosshair to chart area
        crosshairX = Math.Clamp(crosshairX, left, right);

        // Find nearest data point
        float closestDist = float.MaxValue;
        int closestIdx = 0;
        for (int i = 0; i < yValues.Length; i++)
        {
            float px = left + (float)i / (yValues.Length - 1) * chartWidth;
            float dist = Math.Abs(px - crosshairX);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestIdx = i;
            }
        }

        float pointX = left + (float)closestIdx / (yValues.Length - 1) * chartWidth;
        float pointY = (float)(bottom - (yValues[closestIdx] - minY) / range * chartHeight);

        bool isSnapped = closestDist < 18;
        float drawX = isSnapped ? pointX : crosshairX;

        // Vertical crosshair line (dashed)
        using var linePaint = new SKPaint
        {
            Color = new SKColor(0, 0, 0, 35),
            StrokeWidth = 1f,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            PathEffect = SKPathEffect.CreateDash([4f, 4f], 0)
        };
        canvas.DrawLine(drawX, top, drawX, bottom, linePaint);

        // Glow ring around nearest point
        using var glowPaint = new SKPaint
        {
            Color = ColorHelper.WithAlpha(palette.LineColor, 20),
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawCircle(pointX, pointY, 10f, glowPaint);

        // Data point dot (larger when snapped)
        using var dotPaint = new SKPaint
        {
            Color = palette.LineColor,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawCircle(pointX, pointY, isSnapped ? 4f : 1.5f, dotPaint);

        // ── Tooltip pill (date + value stacked, white bg) ──
        bool hasTime = times.Length > closestIdx;
        var value = yValues[closestIdx];
        string valueText = value >= 10000 ? $"${value:N0}" : $"${value:N2}";
        string? dateText = hasTime ? times[closestIdx].ToString("MMM d") : null;

        using var valueFont = new SKFont(SerifTypeface, 13f);
        using var dateFont = new SKFont(SansTypeface, 8f);
        float valueWidth = valueFont.MeasureText(valueText);
        float dateWidth = dateText != null ? dateFont.MeasureText(dateText) : 0;
        float contentWidth = Math.Max(valueWidth, dateWidth);

        float pillPadH = 14f;
        float pillPadV = 10f;
        float lineSpacing = dateText != null ? 19f : 0f;
        float pillWidth = contentWidth + pillPadH * 2;
        float pillHeight = (dateText != null ? 36f : 22f) + pillPadV;
        float pillX = pointX - pillWidth / 2;
        float pillY = pointY - pillHeight - 10;

        // Flip below point if too high
        if (pillY < top) pillY = pointY + 14;

        // Clamp within chart bounds
        pillX = Math.Clamp(pillX, left, right - pillWidth);

        // White background with subtle border
        using var pillBg = new SKPaint
        {
            Color = new SKColor(255, 255, 255, 240),
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawRoundRect(pillX, pillY, pillWidth, pillHeight, 6, 6, pillBg);

        using var pillBorder = new SKPaint
        {
            Color = new SKColor(0, 0, 0, 20),
            StrokeWidth = 1f,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };
        canvas.DrawRoundRect(pillX, pillY, pillWidth, pillHeight, 6, 6, pillBorder);

        // Date (top line, muted)
        float textY = pillY + pillPadV;
        if (dateText != null)
        {
            textY += 9f;
            using var datePaint = new SKPaint
            {
                Color = new SKColor(0, 0, 0, 120),
                IsAntialias = true
            };
            canvas.DrawText(dateText, pillX + pillPadH, textY, SKTextAlign.Left, dateFont, datePaint);
            textY += lineSpacing;
        }
        else
        {
            textY += 13f;
        }

        // Value (bottom line, serif, heavier weight via fake bold)
        using var valuePaint = new SKPaint
        {
            Color = new SKColor(26, 26, 46),
            IsAntialias = true
        };
        valueFont.Embolden = true;
        canvas.DrawText(valueText, pillX + pillPadH, textY, SKTextAlign.Left, valueFont, valuePaint);
    }
}
