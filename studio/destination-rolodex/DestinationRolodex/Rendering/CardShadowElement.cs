using SkiaSharp;
using Uno.WinUI.Graphics2DSK;
using Windows.Foundation;

namespace DestinationRolodex.Rendering;

// xaml-lint: allow builtin - ShadowContainer cannot retint per frame and paints under translucent surfaces; this is a bare blurred rect.
/// <summary>
/// Destination-tinted ambient shadow under the postcard, drawn as a blurred rounded rect in a canvas that extends past the
/// card by <see cref="Bleed"/>. The tint follows the material mix per frame; the blur mask is cached. Opacity is baked into
/// the paint because SKCanvasElement ignores XAML Opacity.
/// </summary>
public sealed class CardShadowElement : SKCanvasElement
{
    public const float Bleed = 72f;

    private readonly SKPaint _paint = new() { IsAntialias = true, Style = SKPaintStyle.Fill, MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 24f) };
    private readonly SKPaint _contactPaint = new() { IsAntialias = true, Style = SKPaintStyle.Fill, MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 5f) };
    private volatile ShadowFrame _frame = new(new SKColor(0x18, 0x19, 0x17), 30, 1);
    private bool _disposed;

    public sealed record ShadowFrame(SKColor Tint, float CornerRadius, float Opacity);

    public CardShadowElement()
    {
        if (!IsSupportedOnCurrentPlatform())
        {
            throw new PlatformNotSupportedException("SKCanvasElement requires Skia rendering on the current platform.");
        }
    }

    public void Present(ShadowFrame frame)
    {
        if (_disposed)
        {
            return;
        }

        if (_frame.Tint == frame.Tint && Math.Abs(_frame.Opacity - frame.Opacity) < .01f && Math.Abs(_frame.CornerRadius - frame.CornerRadius) < .5f)
        {
            return;
        }

        _frame = frame;
        Invalidate();
    }

    protected override void RenderOverride(SKCanvas canvas, Size area)
    {
        if (_disposed)
        {
            return;
        }

        var f = _frame;
        var card = new SKRect(Bleed, Bleed, (float)area.Width - Bleed, (float)area.Height - Bleed);
        var o = Math.Clamp(f.Opacity, 0, 1);
        _paint.Color = f.Tint.WithAlpha((byte)(0x38 * o));
        canvas.DrawRoundRect(new SKRoundRect(SKRect.Create(card.Left + 4, card.Top + 26, card.Width - 8, card.Height - 20), f.CornerRadius), _paint);
        _contactPaint.Color = f.Tint.WithAlpha((byte)(0x2A * o));
        canvas.DrawRoundRect(new SKRoundRect(SKRect.Create(card.Left + 1, card.Top + 3, card.Width - 2, card.Height - 1), f.CornerRadius), _contactPaint);
    }

    public void Release()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _paint.Dispose();
        _contactPaint.Dispose();
    }
}
