using DestinationRolodex.Models;
using SkiaSharp;

namespace DestinationRolodex.Rendering;

/// <summary>Flat-image camera pose: translate in logical px, tilt in degrees, zoom factor. Perspective is fixed at 1000 (prototype).</summary>
public readonly record struct CameraPose(float TranslateX, float TranslateY, float RotateXDegrees, float RotateYDegrees, float Zoom)
{
    public static CameraPose Identity => new(0, 0, 0, 0, 1);
}

/// <summary>
/// A decoded photograph owned by the media cache. The image and its child shader live until the cache retires
/// and collects the entry; frames referencing it keep rendering safely because collection is deferred past
/// the frames that may still read it (see DestinationMediaCache.Collect).
/// </summary>
public sealed class DecodedPhoto : IDisposable
{
    private SKShader? _shader;
    private bool _disposed;

    public DecodedPhoto(string id, SKImage image)
    {
        Id = id;
        Image = image;
        Width = image.Width;
        Height = image.Height;
        Aspect = (float)image.Width / image.Height;
        Bytes = (long)image.Width * image.Height * 4;
    }

    public string Id { get; }
    public SKImage Image { get; }
    public int Width { get; }
    public int Height { get; }
    public float Aspect { get; }
    public long Bytes { get; }
    public bool IsDisposed => _disposed;

    /// <summary>Clamp-tiled, linearly filtered, identity local matrix: the shader samples image pixels directly.</summary>
    public SKShader Shader => _shader ??= Image.ToShader(SKShaderTileMode.Clamp, SKShaderTileMode.Clamp, new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.None));

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _shader?.Dispose();
        Image.Dispose();
    }
}

/// <summary>
/// One country row as the renderer draws it: solid ink base plus, in flight, its own destination photo through the
/// same glyphs (brief §4 photo-in-type). All geometry is in scene coordinates; the row is centered horizontally.
/// </summary>
public sealed record RowGlyphs(
    string Text,
    float FontSize,
    float TrackingEm,
    float CenterY,
    float ScaleX,
    float ScaleY,
    float Opacity,
    float BlurSigma,
    DecodedPhoto? Photo,
    FocalPoint Focal,
    float PhotoAlpha);

/// <summary>
/// Immutable inputs for one rendered frame. Built on the UI thread, read on the render thread.
/// Progress 0 is exactly <see cref="From"/>; progress 1 is exactly <see cref="To"/>. A static frame sets both to the same photo.
/// </summary>
public sealed record PhotoFrame(
    DecodedPhoto? From,
    DecodedPhoto? To,
    FocalPoint FocalA,
    FocalPoint FocalB,
    float Progress,
    float Strength,
    float Direction,
    PhotoTransitionKind Mode,
    CameraPose Camera,
    SKColor Backdrop)
{
    /// <summary>Shader mode override for the lab's diagnostic crossfade (mode 2); never advertised as an effect.</summary>
    public bool DiagnosticCrossfade { get; init; }

    /// <summary>Country rows to draw above the scrim; empty in Look around or for an empty catalog.</summary>
    public IReadOnlyList<RowGlyphs> Rows { get; init; } = Array.Empty<RowGlyphs>();
    public float RowsOpacity { get; init; } = 1;
    public float RowsOffsetY { get; init; }
    public SKColor RowInk { get; init; } = new(0xFF, 0xFD, 0xF6);

    /// <summary>Scrim strength behind the caption (0 hides it, 1 is the authored strength).</summary>
    public float Scrim { get; init; } = 1;

    /// <summary>Faint warm highlight at the pointer (fine pointers only); null when absent.</summary>
    public SKPoint? PointerLight { get; init; }
    public float PointerLightOpacity { get; init; }

    public static PhotoFrame Empty(SKColor backdrop) => new(null, null, FocalPoint.Center, FocalPoint.Center, 0, .6f, 1, PhotoTransitionKind.MorphBlend, CameraPose.Identity, backdrop);

    public static PhotoFrame Static(DecodedPhoto photo, FocalPoint focal, CameraPose camera, SKColor backdrop) =>
        new(photo, photo, focal, focal, 0, .6f, 1, PhotoTransitionKind.MorphBlend, camera, backdrop);

    public bool HasPhoto => From is { IsDisposed: false } || To is { IsDisposed: false };
    public bool IsTransition => From is not null && To is not null && !ReferenceEquals(From, To) && Progress > 0 && Progress < 1;
}
