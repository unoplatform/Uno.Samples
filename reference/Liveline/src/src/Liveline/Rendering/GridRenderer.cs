using Liveline.Helpers;
using SkiaSharp;

namespace Liveline.Rendering;

internal static class GridRenderer
{
    private const float LeftPadding = 16f;
    private const float RightPadding = 80f;
    private const float TopPadding = 16f;
    private const float BottomPadding = 32f;
    private const int TargetGridLines = 5;
    private static readonly float[] DashIntervals = [4f, 4f];

    public static (float Left, float Right, float Top, float Bottom) GetChartArea(float width, float height)
        => (LeftPadding, width - RightPadding, TopPadding, height - BottomPadding);

    public static void Draw(
        SKCanvas canvas,
        float width, float height,
        double minY, double maxY,
        DateTimeOffset[] times,
        ChartPalette palette,
        RenderContext rc)
    {
        var (left, right, top, bottom) = GetChartArea(width, height);
        float chartHeight = bottom - top;
        float chartWidth = right - left;

        if (chartHeight <= 0 || chartWidth <= 0) return;

        rc.Fill.Color = palette.TextDim;
        rc.Fill.Style = SKPaintStyle.Fill;

        double range = maxY - minY;
        if (range <= 0) range = 1;

        double step = NiceStep(range, TargetGridLines);
        double firstLine = Math.Ceiling(minY / step) * step;

        for (double val = firstLine; val <= maxY; val += step)
        {
            float y = (float)(bottom - (val - minY) / range * chartHeight);
            if (y < top || y > bottom) continue;

            canvas.DrawText(FormatValue(val), right + 6f, y + 4f, SKTextAlign.Left, rc.LabelFont, rc.Fill);
        }

        if (times.Length >= 2)
        {
            int labelCount = Math.Max(2, (int)(chartWidth / 80));
            int stepIdx = Math.Max(1, (times.Length - 1) / (labelCount - 1));

            for (int i = 0; i < times.Length; i += stepIdx)
            {
                float x = left + (float)i / (times.Length - 1) * chartWidth;
                string timeLabel = times[i].ToString("mm:ss");
                float tw = rc.LabelFont.MeasureText(timeLabel);

                float drawX = Math.Clamp(x - tw / 2f, left, right - tw);
                canvas.DrawText(timeLabel, drawX, bottom + 16f, SKTextAlign.Left, rc.LabelFont, rc.Fill);
            }
        }
    }

    public static void DrawTrackingLine(
        SKCanvas canvas,
        float width, float height,
        double dotValue, double minY, double maxY,
        ChartPalette palette,
        RenderContext rc)
    {
        var (left, right, top, bottom) = GetChartArea(width, height);
        float chartHeight = bottom - top;
        double range = maxY - minY;
        if (range <= 0) range = 1;

        float y = (float)(bottom - (dotValue - minY) / range * chartHeight);
        y = Math.Clamp(y, top, bottom);

        rc.ThinStroke.Color = ColorHelper.WithAlpha(palette.LineColor, 90);
        using var dash = SKPathEffect.CreateDash(DashIntervals, 0);
        rc.ThinStroke.PathEffect = dash;

        canvas.DrawLine(left, y, right, y, rc.ThinStroke);

        rc.ThinStroke.PathEffect = null;
    }

    private static double NiceStep(double range, int targetLines)
    {
        double rough = range / targetLines;
        double mag = Math.Pow(10, Math.Floor(Math.Log10(rough)));
        double normalized = rough / mag;

        double nice;
        if (normalized <= 1.5) nice = 1;
        else if (normalized <= 3.5) nice = 2;
        else if (normalized <= 7.5) nice = 5;
        else nice = 10;

        return nice * mag;
    }

    private static string FormatValue(double val)
    {
        if (Math.Abs(val) >= 1000)
            return val.ToString("N0");
        if (Math.Abs(val) >= 1)
            return val.ToString("F1");
        return val.ToString("F2");
    }
}
