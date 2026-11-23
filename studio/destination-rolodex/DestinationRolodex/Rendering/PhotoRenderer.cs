using System.Diagnostics;
using DestinationRolodex.Models;
using SkiaSharp;

namespace DestinationRolodex.Rendering;

/// <summary>
/// Draws the Living Postcard scene for one card: the overscanned photo layer (runtime SkSL effect or plain fallback)
/// under the camera, the top wash and caption scrim, the pointer highlight, and the country rows (solid ink plus the
/// photo-in-type pass). Pure SkiaSharp: the same code renders on the SKCanvasElement window canvas and offscreen.
///
/// Ownership: the renderer owns its compiled effect, builder, paints, typeface and text-blob cache. It never owns
/// photos (the cache does). Per frame it only sets uniforms and builds short-lived shaders; nothing is compiled,
/// decoded or shaped twice for the same text.
/// </summary>
public sealed class PhotoRenderer : IDisposable
{
    public const float OverscanPerSide = .13f;
    public const float Perspective = 1000f;
    public const int MaxTextureSide = 1600;
    private const int TextBlobCacheLimit = 48;

    private readonly SKRuntimeEffect? _effect;
    private readonly SKRuntimeShaderBuilder? _builder;
    private readonly SKPaint _photoPaint = new() { IsAntialias = false };
    private readonly SKPaint _backdropPaint = new() { Style = SKPaintStyle.Fill };
    private readonly SKPaint _imagePaint = new() { IsAntialias = false };
    private readonly SKPaint _gradientPaint = new() { Style = SKPaintStyle.Fill };
    private readonly SKPaint _textPaint = new() { IsAntialias = true, Style = SKPaintStyle.Fill };
    private readonly SKPaint _textFillPaint = new() { IsAntialias = true, Style = SKPaintStyle.Fill };
    private readonly SKSamplingOptions _sampling = new(SKFilterMode.Linear, SKMipmapMode.None);
    private readonly Dictionary<(string Text, float Size, float Tracking), (SKTextBlob Blob, float Width, float CapHeight)> _blobs = new();
    private readonly Dictionary<int, SKMaskFilter> _blurs = new();
    private readonly double[] _renderMs = new double[240];
    private SKTypeface? _typeface;
    private int _renderCount;
    private bool _disposed;

    public PhotoRenderer(bool allowRuntimeEffect = true)
    {
        if (allowRuntimeEffect)
        {
            _effect = SKRuntimeEffect.CreateShader(TransitionShaderSource.Sksl, out var errors);
            CompileError = _effect is null ? (string.IsNullOrWhiteSpace(errors) ? "SKRuntimeEffect.CreateShader returned null." : errors) : null;
            if (_effect is not null)
            {
                _builder = new SKRuntimeShaderBuilder(_effect);
            }
        }
        else
        {
            CompileError = "Runtime effect disabled by host.";
        }
    }

    /// <summary>True when the SkSL effect compiled and the morph/liquid paths are live.</summary>
    public bool HasRuntimeEffect => _builder is not null;

    public string? CompileError { get; }

    /// <summary>Frames rendered so far; the media cache uses it as a barrier before disposing retired photos.</summary>
    public long FramesRendered => Interlocked.Read(ref _framesRendered);
    private long _framesRendered;

    /// <summary>Country-row typeface (Inter SemiBold). Rows are skipped until it is set.</summary>
    public void SetTypeface(SKTypeface typeface)
    {
        _typeface = typeface;
        ClearTextCache();
    }

    public bool HasTypeface => _typeface is not null;

    /// <summary>Last render durations in milliseconds (ring buffer), for the performance evidence record.</summary>
    public IReadOnlyList<double> RecentRenderMilliseconds
    {
        get
        {
            var n = Math.Min(_renderCount, _renderMs.Length);
            var copy = new double[n];
            Array.Copy(_renderMs, copy, n);
            return copy;
        }
    }

    /// <summary>Overscanned layer rectangle for a scene of the given size (13% per side, 1.26x).</summary>
    public static SKRect LayerRect(float sceneWidth, float sceneHeight) =>
        new(-OverscanPerSide * sceneWidth, -OverscanPerSide * sceneHeight, (1 + OverscanPerSide) * sceneWidth, (1 + OverscanPerSide) * sceneHeight);

    /// <summary>
    /// CSS: perspective(1000px) translate3d(x,y,0) rotateX(rx) rotateY(ry) scale(z) with transform-origin at the scene center,
    /// projected to the z=0 plane as a 3x3 projective matrix. Same mapping for static frames and shader frames.
    /// </summary>
    public static SKMatrix CameraMatrix(CameraPose c, float centerX, float centerY)
    {
        var rx = c.RotateXDegrees * Math.PI / 180d;
        var ry = c.RotateYDegrees * Math.PI / 180d;
        double s = c.Zoom;
        double cry = Math.Cos(ry), sry = Math.Sin(ry), crx = Math.Cos(rx), srx = Math.Sin(rx);
        double d = Perspective;

        double a00 = s * cry, a01 = 0, a02 = c.TranslateX;
        double a10 = s * sry * srx, a11 = s * crx, a12 = c.TranslateY;
        double a20 = s * sry * crx / d, a21 = -s * srx / d, a22 = 1;

        var projected = new SKMatrix(
            (float)(a00 + centerX * a20), (float)(a01 + centerX * a21), (float)(a02 + centerX * a22),
            (float)(a10 + centerY * a20), (float)(a11 + centerY * a21), (float)(a12 + centerY * a22),
            (float)a20, (float)a21, (float)a22);

        return projected.PreConcat(SKMatrix.CreateTranslation(-centerX, -centerY));
    }

    /// <summary>Cover crop in normalized top-left image space: (offsetX, offsetY, scaleX, scaleY). Mirrors crop() in the shader.</summary>
    public static (float OffsetX, float OffsetY, float ScaleX, float ScaleY) CoverCrop(float imageAspect, float layerAspect, FocalPoint focal)
    {
        float sx, sy;
        if (imageAspect > layerAspect)
        {
            sx = layerAspect / imageAspect;
            sy = 1;
        }
        else
        {
            sx = 1;
            sy = imageAspect / layerAspect;
        }

        return ((1 - sx) * (float)focal.X, (1 - sy) * (float)focal.Y, sx, sy);
    }

    /// <summary>Matrix mapping scene coordinates to image pixels for a photo cover-cropped onto the overscanned layer (identity camera).</summary>
    public static SKMatrix SceneToImage(DecodedPhoto photo, FocalPoint focal, float sceneWidth, float sceneHeight)
    {
        var layer = LayerRect(sceneWidth, sceneHeight);
        var (ox, oy, sx, sy) = CoverCrop(photo.Aspect, layer.Width / layer.Height, focal);
        var m = SKMatrix.CreateTranslation(ox * photo.Width, oy * photo.Height);
        m = m.PreConcat(SKMatrix.CreateScale(sx * photo.Width / layer.Width, sy * photo.Height / layer.Height));
        return m.PreConcat(SKMatrix.CreateTranslation(-layer.Left, -layer.Top));
    }

    public void Render(SKCanvas canvas, float sceneWidth, float sceneHeight, PhotoFrame frame)
    {
        if (_disposed || sceneWidth <= 0 || sceneHeight <= 0)
        {
            return;
        }

        var sw = Stopwatch.StartNew();
        canvas.Save();
        canvas.ClipRect(new SKRect(0, 0, sceneWidth, sceneHeight));

        _backdropPaint.Color = frame.Backdrop;
        canvas.DrawRect(0, 0, sceneWidth, sceneHeight, _backdropPaint);

        DrawPhoto(canvas, sceneWidth, sceneHeight, frame);
        DrawAtmosphere(canvas, sceneWidth, sceneHeight, frame);
        DrawRows(canvas, sceneWidth, sceneHeight, frame);

        canvas.Restore();
        RecordTiming(sw);
    }

    private void DrawPhoto(SKCanvas canvas, float sceneWidth, float sceneHeight, PhotoFrame frame)
    {
        var from = frame.From is { IsDisposed: false } ? frame.From : null;
        var to = frame.To is { IsDisposed: false } ? frame.To : null;
        if (from is null && to is null)
        {
            return;
        }

        var a = from ?? to!;
        var b = to ?? from!;
        var progress = from is null ? 1f : to is null ? 0f : Math.Clamp(frame.Progress, 0f, 1f);

        canvas.Save();
        var layer = LayerRect(sceneWidth, sceneHeight);
        var camera = CameraMatrix(frame.Camera, sceneWidth / 2f, sceneHeight / 2f);
        canvas.Concat(in camera);
        canvas.Translate(layer.Left, layer.Top);

        if (_builder is not null)
        {
            DrawWithEffect(canvas, layer.Width, layer.Height, a, b, frame, progress);
        }
        else
        {
            DrawPlain(canvas, layer.Width, layer.Height, a, b, frame, progress);
        }

        canvas.Restore();
    }

    private void DrawWithEffect(SKCanvas canvas, float w, float h, DecodedPhoto a, DecodedPhoto b, PhotoFrame frame, float progress)
    {
        var builder = _builder!;
        builder.Children["uFrom"] = a.Shader;
        builder.Children["uTo"] = b.Shader;
        builder.Uniforms["uProgress"] = progress;
        builder.Uniforms["uStrength"] = Math.Clamp(frame.Strength, 0f, 1f);
        builder.Uniforms["uDirection"] = frame.Direction < 0 ? -1f : 1f;
        builder.Uniforms["uAspect"] = w / h;
        builder.Uniforms["uAspectA"] = a.Aspect;
        builder.Uniforms["uAspectB"] = b.Aspect;
        builder.Uniforms["uFocalA"] = new[] { (float)frame.FocalA.X, (float)frame.FocalA.Y };
        builder.Uniforms["uFocalB"] = new[] { (float)frame.FocalB.X, (float)frame.FocalB.Y };
        builder.Uniforms["uMode"] = frame.DiagnosticCrossfade ? 2 : (int)frame.Mode;
        builder.Uniforms["uResolution"] = new[] { w, h };
        builder.Uniforms["uImageSizeA"] = new[] { (float)a.Width, (float)a.Height };
        builder.Uniforms["uImageSizeB"] = new[] { (float)b.Width, (float)b.Height };

        using var shader = builder.Build();
        _photoPaint.Shader = shader;
        canvas.DrawRect(0, 0, w, h, _photoPaint);
        _photoPaint.Shader = null;
    }

    /// <summary>Capability path when the runtime effect is unavailable: cover-cropped images with a plain crossfade. Never labelled as an effect.</summary>
    private void DrawPlain(SKCanvas canvas, float w, float h, DecodedPhoto a, DecodedPhoto b, PhotoFrame frame, float progress)
    {
        var dest = new SKRect(0, 0, w, h);
        var layerAspect = w / h;

        if (progress < 1)
        {
            _imagePaint.Color = SKColors.White;
            canvas.DrawImage(a.Image, SourceRect(a, layerAspect, frame.FocalA), dest, _sampling, _imagePaint);
        }

        if (progress > 0)
        {
            var eased = progress * progress * (3 - 2 * progress);
            _imagePaint.Color = SKColors.White.WithAlpha((byte)Math.Round(255 * eased));
            canvas.DrawImage(b.Image, SourceRect(b, layerAspect, frame.FocalB), dest, _sampling, _imagePaint);
        }
    }

    private static SKRect SourceRect(DecodedPhoto photo, float layerAspect, FocalPoint focal)
    {
        var (ox, oy, sx, sy) = CoverCrop(photo.Aspect, layerAspect, focal);
        return new SKRect(ox * photo.Width, oy * photo.Height, (ox + sx) * photo.Width, (oy + sy) * photo.Height);
    }

    /// <summary>Top wash (natural colors kept), feathered caption scrim, pointer highlight.</summary>
    private void DrawAtmosphere(SKCanvas canvas, float w, float h, PhotoFrame frame)
    {
        if (frame.Scrim <= 0 && frame.PointerLightOpacity <= 0.005f)
        {
            return; // raw photo frames (fixture export, parity comparison)
        }

        if (frame.Scrim > 0)
        using (var top = SKShader.CreateLinearGradient(new SKPoint(0, 0), new SKPoint(0, h * .22f),
                   new[] { new SKColor(0x05, 0x1D, 0x24, 0x34), new SKColor(0x02, 0x1E, 0x24, 0x00) }, null, SKShaderTileMode.Clamp))
        {
            _gradientPaint.Shader = top;
            canvas.DrawRect(0, 0, w, h * .22f, _gradientPaint);
        }

        if (frame.Scrim > 0)
        {
            var strength = Math.Clamp(frame.Scrim, 0, 1);
            using var scrim = SKShader.CreateLinearGradient(new SKPoint(0, h * .40f), new SKPoint(0, h),
                new[]
                {
                    new SKColor(0x08, 0x0C, 0x0C, 0x00),
                    new SKColor(0x08, 0x0C, 0x0C, (byte)(0x3A * strength)),
                    new SKColor(0x07, 0x0A, 0x0A, (byte)(0x9E * strength)),
                    new SKColor(0x06, 0x08, 0x08, (byte)(0xC4 * strength)),
                },
                new[] { 0f, .38f, .78f, 1f }, SKShaderTileMode.Clamp);
            _gradientPaint.Shader = scrim;
            canvas.DrawRect(0, h * .40f, w, h * .60f, _gradientPaint);
        }

        if (frame.PointerLight is { } light && frame.PointerLightOpacity > 0.005f)
        {
            var alpha = (byte)(0x22 * Math.Clamp(frame.PointerLightOpacity, 0, 1));
            using var glow = SKShader.CreateRadialGradient(light, Math.Max(w, h) * .42f,
                new[] { new SKColor(0xFF, 0xF7, 0xE0, alpha), new SKColor(0xFF, 0xF7, 0xE0, 0x00) }, null, SKShaderTileMode.Clamp);
            _gradientPaint.Shader = glow;
            canvas.DrawRect(0, 0, w, h, _gradientPaint);
        }

        _gradientPaint.Shader = null;
    }

    /// <summary>Country rows: solid ink at the row's opacity, then the row's own photo through the same glyphs at the photo-in-type alpha.</summary>
    private void DrawRows(SKCanvas canvas, float w, float h, PhotoFrame frame)
    {
        if (_typeface is null || frame.Rows.Count == 0 || frame.RowsOpacity <= 0.001f)
        {
            return;
        }

        var centerX = w / 2f;
        foreach (var row in frame.Rows)
        {
            if (row.Opacity <= 0.002f || string.IsNullOrEmpty(row.Text))
            {
                continue;
            }

            var (blob, width, capHeight) = GetBlob(row.Text, row.FontSize, row.TrackingEm);
            canvas.Save();
            canvas.Translate(centerX, row.CenterY + frame.RowsOffsetY);
            canvas.Scale(row.ScaleX, row.ScaleY);
            var x = -width / 2f;
            var baseline = capHeight / 2f;

            var alpha = (byte)Math.Round(255 * Math.Clamp(row.Opacity * frame.RowsOpacity, 0, 1));
            _textPaint.Color = frame.RowInk.WithAlpha(alpha);
            _textPaint.MaskFilter = row.BlurSigma > 0.05f ? GetBlur(row.BlurSigma) : null;
            canvas.DrawText(blob, x, baseline, _textPaint);

            if (row.Photo is { IsDisposed: false } photo && row.PhotoAlpha > 0.004f)
            {
                // Map the photo across the scene (identity camera) so the fill reads as one continuous photograph behind the letters.
                var sceneToImage = SceneToImage(photo, row.Focal, w, h);
                var rowLocalToScene = SKMatrix.CreateTranslation(centerX, row.CenterY + frame.RowsOffsetY).PreConcat(SKMatrix.CreateScale(row.ScaleX, row.ScaleY));
                var local = sceneToImage.PreConcat(rowLocalToScene);
                using var fill = photo.Shader.WithLocalMatrix(local.Invert());
                _textFillPaint.Shader = fill;
                _textFillPaint.Color = SKColors.White.WithAlpha((byte)Math.Round(255 * Math.Clamp(row.PhotoAlpha * frame.RowsOpacity, 0, 1)));
                _textFillPaint.MaskFilter = _textPaint.MaskFilter;
                canvas.DrawText(blob, x, baseline, _textFillPaint);
                _textFillPaint.Shader = null;
            }

            canvas.Restore();
        }

        _textPaint.MaskFilter = null;
        _textFillPaint.MaskFilter = null;
    }

    private (SKTextBlob Blob, float Width, float CapHeight) GetBlob(string text, float size, float trackingEm)
    {
        var key = (text, MathF.Round(size, 2), MathF.Round(trackingEm, 4));
        if (_blobs.TryGetValue(key, out var cached))
        {
            return cached;
        }

        if (_blobs.Count >= TextBlobCacheLimit)
        {
            ClearTextCache();
        }

        using var font = new SKFont(_typeface!, size) { Subpixel = true, Edging = SKFontEdging.SubpixelAntialias, Hinting = SKFontHinting.Slight };
        var glyphs = font.GetGlyphs(text);
        var advances = font.GetGlyphWidths(glyphs);
        var tracking = trackingEm * size;
        var positions = new SKPoint[glyphs.Length];
        var x = 0f;
        for (var i = 0; i < glyphs.Length; i++)
        {
            positions[i] = new SKPoint(x, 0);
            x += advances[i] + (i < glyphs.Length - 1 ? tracking : 0);
        }

        using var builder = new SKTextBlobBuilder();
        var run = builder.AllocatePositionedRun(font, glyphs.Length);
        run.SetGlyphs(glyphs);
        run.SetPositions(positions);
        var blob = builder.Build() ?? throw new InvalidOperationException("Text blob could not be built.");
        var metrics = font.Metrics;
        var capHeight = metrics.CapHeight > 0 ? metrics.CapHeight : -metrics.Ascent * .72f;
        var entry = (blob, x, capHeight);
        _blobs[key] = entry;
        return entry;
    }

    private SKMaskFilter GetBlur(float sigma)
    {
        var k = (int)Math.Round(sigma * 4);
        if (!_blurs.TryGetValue(k, out var f))
        {
            f = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, k / 4f);
            _blurs[k] = f;
        }

        return f;
    }

    private void ClearTextCache()
    {
        foreach (var e in _blobs.Values)
        {
            e.Blob.Dispose();
        }

        _blobs.Clear();
    }

    private void RecordTiming(Stopwatch sw)
    {
        sw.Stop();
        _renderMs[_renderCount % _renderMs.Length] = sw.Elapsed.TotalMilliseconds;
        _renderCount++;
        Interlocked.Increment(ref _framesRendered);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        ClearTextCache();
        foreach (var b in _blurs.Values)
        {
            b.Dispose();
        }

        _blurs.Clear();
        _photoPaint.Dispose();
        _backdropPaint.Dispose();
        _imagePaint.Dispose();
        _gradientPaint.Dispose();
        _textPaint.Dispose();
        _textFillPaint.Dispose();
        _typeface?.Dispose();
        // The builder disposes its effect; it was created here, so this is the one place that may dispose it.
        _builder?.Dispose();
    }
}
