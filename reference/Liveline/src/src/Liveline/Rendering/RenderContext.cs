using SkiaSharp;

namespace Liveline.Rendering;

internal sealed class RenderContext : IDisposable
{
    public readonly SKPaint Fill = new() { IsAntialias = true, Style = SKPaintStyle.Fill };
    public readonly SKPaint Stroke = new()
    {
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2.5f,
        StrokeCap = SKStrokeCap.Round,
        StrokeJoin = SKStrokeJoin.Round
    };
    public readonly SKPaint ThinStroke = new()
    {
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f
    };
    public readonly SKFont LabelFont = new(SKTypeface.FromFamilyName("Segoe UI", SKFontStyle.Normal), 11f);
    public readonly SKFont BadgeFont = new(SKTypeface.FromFamilyName("Segoe UI", SKFontStyle.Bold), 12f);
    public readonly SKFont MessageFont = new(SKTypeface.FromFamilyName("Segoe UI", SKFontStyle.Normal), 14f);

    public bool IsDisposed { get; private set; }

    public void Dispose()
    {
        if (IsDisposed) return;
        IsDisposed = true;
        Fill.Dispose();
        Stroke.Dispose();
        ThinStroke.Dispose();
        LabelFont.Dispose();
        BadgeFont.Dispose();
        MessageFont.Dispose();
    }
}
