using Liveline.Helpers;
using SkiaSharp;

namespace Liveline.Rendering;

internal static class LineRenderer
{
    public static void Draw(
        SKCanvas canvas,
        float width, float height,
        double[] yValues,
        double minY, double maxY,
        bool showFill,
        ChartPalette palette,
        RenderContext rc)
    {
        if (yValues.Length < 2) return;

        var (left, right, top, bottom) = GridRenderer.GetChartArea(width, height);
        float chartHeight = bottom - top;
        float chartWidth = right - left;
        double range = maxY - minY;
        if (range <= 0) range = 1;

        using var path = new SKPath();
        float firstX = left;
        float firstY = (float)(bottom - (yValues[0] - minY) / range * chartHeight);
        path.MoveTo(firstX, firstY);

        float invRange = (float)(1.0 / range);
        float invLast = 1f / (yValues.Length - 1);

        for (int i = 1; i < yValues.Length; i++)
        {
            float x = left + i * invLast * chartWidth;
            float y = bottom - ((float)yValues[i] - (float)minY) * invRange * chartHeight;

            float prevX = left + (i - 1) * invLast * chartWidth;
            float prevY = bottom - ((float)yValues[i - 1] - (float)minY) * invRange * chartHeight;
            float cpOffset = (x - prevX) * 0.3f;

            path.CubicTo(prevX + cpOffset, prevY, x - cpOffset, y, x, y);
        }

        if (showFill)
        {
            using var fillPath = new SKPath(path);
            fillPath.LineTo(right, bottom);
            fillPath.LineTo(firstX, bottom);
            fillPath.Close();

            using var gradient = SKShader.CreateLinearGradient(
                new SKPoint(0, top),
                new SKPoint(0, bottom),
                [palette.FillTop, palette.FillBottom],
                null,
                SKShaderTileMode.Clamp);

            rc.Fill.Color = SKColors.White;
            rc.Fill.Style = SKPaintStyle.Fill;
            rc.Fill.Shader = gradient;
            canvas.DrawPath(fillPath, rc.Fill);
            rc.Fill.Shader = null;
        }

        rc.Stroke.Color = palette.LineColor;
        canvas.DrawPath(path, rc.Stroke);
    }
}
