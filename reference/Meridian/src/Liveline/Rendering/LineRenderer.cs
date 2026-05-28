using Liveline.Helpers;
using SkiaSharp;

namespace Liveline.Rendering;

/// <summary>
/// Draws the main line path with optional gradient fill below it.
/// </summary>
public static class LineRenderer
{
    public static void Draw(
        SKCanvas canvas,
        float width, float height,
        double[] yValues,
        double minY, double maxY,
        bool showFill,
        ChartPalette palette)
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

        for (int i = 1; i < yValues.Length; i++)
        {
            float x = left + (float)i / (yValues.Length - 1) * chartWidth;
            float y = (float)(bottom - (yValues[i] - minY) / range * chartHeight);

            float prevX = left + (float)(i - 1) / (yValues.Length - 1) * chartWidth;
            float prevY = (float)(bottom - (yValues[i - 1] - minY) / range * chartHeight);
            float cpOffset = (x - prevX) * 0.3f;

            path.CubicTo(prevX + cpOffset, prevY, x - cpOffset, y, x, y);
        }

        // Gradient fill
        if (showFill)
        {
            using var fillPath = new SKPath(path);
            float lastX = right;
            fillPath.LineTo(lastX, bottom);
            fillPath.LineTo(firstX, bottom);
            fillPath.Close();

            using var fillPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, top),
                    new SKPoint(0, bottom),
                    new[] { palette.FillTop, palette.FillBottom },
                    null,
                    SKShaderTileMode.Clamp)
            };

            canvas.DrawPath(fillPath, fillPaint);
        }

        // Line stroke
        using var linePaint = new SKPaint
        {
            Color = palette.LineColor,
            StrokeWidth = 2.5f,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };

        canvas.DrawPath(path, linePaint);
    }
}
