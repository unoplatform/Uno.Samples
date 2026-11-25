using SkiaSharp;
using Uno.WinUI.Graphics2DSK;
using Windows.Foundation;

namespace DestinationRolodex.Rendering;

/// <summary>
/// The photo layer of one card: an SKCanvasElement that presents whatever <see cref="PhotoFrame"/> it was last given.
/// Construct it only after <see cref="SKCanvasElement.IsSupportedOnCurrentPlatform"/> returned true; the card does that
/// check and shows the XAML fallback otherwise. Nothing here decodes, compiles or touches XAML state.
/// </summary>
public sealed class PhotoTransitionSurface : SKCanvasElement
{
    private readonly PhotoRenderer _renderer;
    private volatile PhotoFrame _frame;
    private bool _disposed;

    public PhotoTransitionSurface(bool allowRuntimeEffect = true)
    {
        if (!IsSupportedOnCurrentPlatform())
        {
            throw new PlatformNotSupportedException("SKCanvasElement requires Skia rendering on the current platform.");
        }

        _renderer = new PhotoRenderer(allowRuntimeEffect);
        _frame = PhotoFrame.Empty(SKColors.Black);
    }

    public PhotoRenderer Renderer => _renderer;

    public PhotoFrame CurrentFrame => _frame;

    /// <summary>Swaps the frame inputs (UI thread) and requests one redraw. Frames are immutable, so the render thread never sees a half-written state.</summary>
    public void Present(PhotoFrame frame)
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

        _renderer.Render(canvas, (float)area.Width, (float)area.Height, _frame);
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
