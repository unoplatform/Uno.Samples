using Liveline.Helpers;
using SkiaSharp;

namespace Liveline.Rendering;

internal static class BadgeRenderer
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
        ChartPalette palette,
        RenderContext rc)
    {
        var (_, right, top, bottom) = GridRenderer.GetChartArea(width, height);
        float chartHeight = bottom - top;
        double range = maxY - minY;
        if (range <= 0) range = 1;

        float y = (float)(bottom - (badgeY - minY) / range * chartHeight);
        y = Math.Clamp(y, top + BadgeHeight / 2, bottom - BadgeHeight / 2);

        string text = FormatBadgeValue(currentValue);
        float textWidth = rc.BadgeFont.MeasureText(text);
        float badgeWidth = textWidth + BadgePaddingH * 2;

        float bx = right + 6f;
        float by = y - BadgeHeight / 2;
        bx = Math.Min(bx, width - badgeWidth - BadgeMarginRight);

        rc.Fill.Color = palette.BadgeBg;
        rc.Fill.Style = SKPaintStyle.Fill;
        var rect = new SKRoundRect(new SKRect(bx, by, bx + badgeWidth, by + BadgeHeight), BadgeRadius);
        canvas.DrawRoundRect(rect, rc.Fill);

        rc.Fill.Color = palette.BadgeText;
        canvas.DrawText(text, bx + badgeWidth / 2, y + 4.5f, SKTextAlign.Center, rc.BadgeFont, rc.Fill);
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
