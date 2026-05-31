using Liveline.Helpers;
using SkiaSharp;

namespace Liveline.Rendering;

/// <summary>
/// Draws the current value badge in the right margin of the chart.
/// </summary>
public static class BadgeRenderer
{
    private const float BadgeHeight = 28f;
    private const float BadgePaddingH = 8f;
    private const float BadgeRadius = 6f;
    private const float BadgeMarginRight = 4f;

    public static void Draw(
        SKCanvas canvas,
        float width, float height,
        double currentValue,
        double badgeY,
        double minY, double maxY,
        ChartPalette palette)
    {
        var (left, right, top, bottom) = GridRenderer.GetChartArea(width, height);
        float chartHeight = bottom - top;
        double range = maxY - minY;
        if (range <= 0) range = 1;

        float y = (float)(bottom - (badgeY - minY) / range * chartHeight);
        y = Math.Clamp(y, top + BadgeHeight / 2, bottom - BadgeHeight / 2);

        string text = FormatBadgeValue(currentValue);

        using var textFont = new SKFont(SKTypeface.FromFamilyName("Segoe UI", SKFontStyle.Bold), 12f);

        using var textPaint = new SKPaint
        {
            Color = palette.BadgeText,
            IsAntialias = true
        };

        float textWidth = textFont.MeasureText(text);
        float badgeWidth = textWidth + BadgePaddingH * 2;

        // Position badge fully in the right margin, left-aligned to chart edge
        float bx = right + 6f;
        float by = y - BadgeHeight / 2;

        // Clamp so badge doesn't overflow the canvas
        float maxBx = width - badgeWidth - BadgeMarginRight;
        bx = Math.Min(bx, maxBx);

        using var bgPaint = new SKPaint
        {
            Color = palette.BadgeBg,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        var rect = new SKRoundRect(new SKRect(bx, by, bx + badgeWidth, by + BadgeHeight), BadgeRadius);
        canvas.DrawRoundRect(rect, bgPaint);

        canvas.DrawText(text, bx + badgeWidth / 2, y + 4.5f, SKTextAlign.Center, textFont, textPaint);
    }

    private static string FormatBadgeValue(double val)
    {
        if (Math.Abs(val) >= 1000)
            return val.ToString("N1");
        if (Math.Abs(val) >= 1)
            return val.ToString("F2");
        return val.ToString("F3");
    }
}
