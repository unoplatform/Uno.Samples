using SkiaSharp;
using Uno.WinUI.Graphics2DSK;
using Windows.Foundation;

namespace DestinationRolodex.Rendering;

/// <summary>Small SKCanvasElement hosting the globe. The card sets the orientation per frame; nothing animates on its own.</summary>
public sealed class GlobeElement : SKCanvasElement
{
    private readonly GlobeRenderer _renderer;
    private volatile GlobeFrame _frame = new(null, SKColors.Transparent);
    private bool _disposed;

    public sealed record GlobeFrame((double X, double Y, double Z)? Orientation, SKColor Accent);

    public GlobeElement(GlobeRenderer renderer)
    {
        if (!IsSupportedOnCurrentPlatform())
        {
            throw new PlatformNotSupportedException("SKCanvasElement requires Skia rendering on the current platform.");
        }

        _renderer = renderer;
    }

    public void Present(GlobeFrame frame)
    {
        if (_disposed)
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
        _renderer.Marker = f.Accent;
        _renderer.Render(canvas, new SKRect(0, 0, (float)area.Width, (float)area.Height), f.Orientation);
    }

    public void Release()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _renderer.Dispose();
    }
}
