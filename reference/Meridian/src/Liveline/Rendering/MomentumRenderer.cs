using Liveline.Helpers;
using Liveline.Models;
using SkiaSharp;

namespace Liveline.Rendering;

/// <summary>
/// Draws a live dot at the last data point with a directional arrow.
/// Green for up, red for down, grey for flat.
/// </summary>
public static class MomentumRenderer
{
    private const float DotRadius = 5f;

    public static void Draw(
        SKCanvas canvas,
        float width, float height,
        double[] yValues,
        double minY, double maxY,
        MomentumDirection direction,
        ChartPalette palette)
    {
        if (yValues.Length < 2) return;

        var (left, right, top, bottom) = GridRenderer.GetChartArea(width, height);
        float chartHeight = bottom - top;
        double range = maxY - minY;
        if (range <= 0) range = 1;

        // Live dot at last data point
        float dotX = right;
        float dotY = (float)(bottom - (yValues[^1] - minY) / range * chartHeight);
        dotY = Math.Clamp(dotY, top, bottom);

        // Outer glow ring
        using var glowPaint = new SKPaint
        {
            Color = ColorHelper.WithAlpha(palette.LineColor, 50),
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawCircle(dotX, dotY, DotRadius + 4f, glowPaint);

        // Solid dot
        using var dotPaint = new SKPaint
        {
            Color = palette.LineColor,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawCircle(dotX, dotY, DotRadius, dotPaint);

        // White center highlight
        using var highlightPaint = new SKPaint
        {
            Color = new SKColor(255, 255, 255, 120),
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawCircle(dotX - 1.5f, dotY - 1.5f, 2f, highlightPaint);

    }
}
