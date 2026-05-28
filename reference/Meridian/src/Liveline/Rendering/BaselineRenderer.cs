using Liveline.Helpers;
using SkiaSharp;

namespace Liveline.Rendering;

/// <summary>
/// Draws a dashed horizontal reference line at the user's average cost basis.
/// </summary>
public static class BaselineRenderer
{
    public static void Draw(SKCanvas canvas, float width, float height,
        float baselineValue, double minY, double maxY, ChartPalette palette)
    {
        var (left, right, top, bottom) = GridRenderer.GetChartArea(width, height);
        float chartHeight = bottom - top;
        double range = maxY - minY;
        if (range <= 0) return;

        float y = (float)(bottom - (baselineValue - minY) / range * chartHeight);
        y = Math.Clamp(y, top, bottom);

        // Dashed line - use the line color at 30% opacity
        using var linePaint = new SKPaint
        {
            Color = ColorHelper.WithAlpha(palette.LineColor, 80),
            StrokeWidth = 1f,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            PathEffect = SKPathEffect.CreateDash(new[] { 6f, 4f }, 0)
        };
        canvas.DrawLine(left, y, right, y, linePaint);

        // Label "Avg $XXX.XX" positioned just above the line at the right edge
        string label = $"Avg ${baselineValue:N2}";
        using var textFont = new SKFont(SKTypeface.FromFamilyName("Segoe UI", SKFontStyle.Normal), 9f);
        using var textPaint = new SKPaint
        {
            Color = ColorHelper.WithAlpha(palette.LineColor, 140),
            IsAntialias = true
        };

        float textWidth = textFont.MeasureText(label);
        canvas.DrawText(label, right - textWidth, y - 6f, SKTextAlign.Left, textFont, textPaint);
    }
}
