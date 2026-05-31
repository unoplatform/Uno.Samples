using Liveline.Helpers;
using SkiaSharp;

namespace Liveline.Rendering;

internal static class MomentumRenderer
{
    private const float DotRadius = 5f;

    public static void Draw(
        SKCanvas canvas,
        float width, float height,
        double[] yValues,
        double minY, double maxY,
        ChartPalette palette,
        RenderContext rc)
    {
        if (yValues.Length < 2) return;

        var (_, right, top, bottom) = GridRenderer.GetChartArea(width, height);
        float chartHeight = bottom - top;
        double range = maxY - minY;
        if (range <= 0) range = 1;

        float dotX = right;
        float dotY = (float)(bottom - (yValues[^1] - minY) / range * chartHeight);
        dotY = Math.Clamp(dotY, top, bottom);

        rc.Fill.Style = SKPaintStyle.Fill;

        rc.Fill.Color = ColorHelper.WithAlpha(palette.LineColor, 50);
        canvas.DrawCircle(dotX, dotY, DotRadius + 4f, rc.Fill);

        rc.Fill.Color = palette.LineColor;
        canvas.DrawCircle(dotX, dotY, DotRadius, rc.Fill);

        rc.Fill.Color = new SKColor(255, 255, 255, 120);
        canvas.DrawCircle(dotX - 1.5f, dotY - 1.5f, 2f, rc.Fill);
    }
}
