using DestinationRolodex.Models;
using SkiaSharp;

namespace DestinationRolodex.Rendering;

/// <summary>
/// Location-aware orthographic globe (brief §5): an equirectangular land mask sampled through the inverse orthographic
/// projection into a small render buffer, oriented toward a unit vector on the sphere. No idle spin; the orientation is
/// supplied per frame from the same adjacent pair and fraction as the scene. Pure SkiaSharp, no XAML.
/// </summary>
public sealed class GlobeRenderer : IDisposable
{
    public const int BufferSize = 96;

    private readonly byte[]? _mask;
    private readonly int _maskW, _maskH;
    private readonly SKBitmap _buffer = new(BufferSize, BufferSize, SKColorType.Rgba8888, SKAlphaType.Premul);
    private readonly SKPaint _bufferPaint = new() { IsAntialias = true };
    private readonly SKPaint _markerPaint = new() { IsAntialias = true, Style = SKPaintStyle.Fill };
    private readonly SKPaint _markerRingPaint = new() { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 1.2f };
    private readonly SKPaint _rimPaint = new() { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 1f };
    private (double X, double Y, double Z)? _renderedOrientation;
    private bool _renderedPinned;
    private bool _disposed;

    /// <param name="packedLandMask">Equirectangular mask, 1 bit per pixel packed MSB-first per row (1 = land); null for a maskless globe.</param>
    public GlobeRenderer(byte[]? packedLandMask, int maskWidth = 512, int maskHeight = 256)
    {
        if (packedLandMask is not null && packedLandMask.Length >= maskWidth * maskHeight / 8)
        {
            _mask = packedLandMask;
            _maskW = maskWidth;
            _maskH = maskHeight;
        }
    }

    public SKColor Ocean { get; set; } = new(0x2B, 0x47, 0x4F);
    public SKColor Land { get; set; } = new(0xE9, 0xE2, 0xD3);
    public SKColor Rim { get; set; } = new(0xFF, 0xF8, 0xEB, 0x90);
    public SKColor Marker { get; set; } = new(0xFF, 0xF7, 0xE0);

    /// <summary>Unit vector for a geographic point (x toward lon 0, z toward the north pole).</summary>
    public static (double X, double Y, double Z) ToUnitVector(GeoPoint p)
    {
        var lat = p.Latitude * Math.PI / 180d;
        var lon = p.Longitude * Math.PI / 180d;
        return (Math.Cos(lat) * Math.Cos(lon), Math.Cos(lat) * Math.Sin(lon), Math.Sin(lat));
    }

    /// <summary>
    /// Shortest great-circle interpolation. Nearly identical inputs return the start; antipodal inputs travel through a
    /// deterministic intermediate (the pole-ward perpendicular) so the camera never stalls or flips.
    /// </summary>
    public static (double X, double Y, double Z) Slerp((double X, double Y, double Z) a, (double X, double Y, double Z) b, double t)
    {
        t = Math.Clamp(t, 0, 1);
        var dot = Math.Clamp(a.X * b.X + a.Y * b.Y + a.Z * b.Z, -1, 1);
        if (dot > .99999)
        {
            return a;
        }

        if (dot < -.99999)
        {
            // Antipodal: pick a perpendicular axis deterministically and rotate 180° through it.
            var axis = Math.Abs(a.Z) < .9 ? Cross(a, (0, 0, 1)) : Cross(a, (1, 0, 0));
            axis = Normalize(axis);
            var half = Normalize(Cross(axis, a));
            return t < .5 ? Slerp(a, half, t * 2) : Slerp(half, b, (t - .5) * 2);
        }

        var omega = Math.Acos(dot);
        var so = Math.Sin(omega);
        var wa = Math.Sin((1 - t) * omega) / so;
        var wb = Math.Sin(t * omega) / so;
        return Normalize((wa * a.X + wb * b.X, wa * a.Y + wb * b.Y, wa * a.Z + wb * b.Z));
    }

    private static (double X, double Y, double Z) Cross((double X, double Y, double Z) a, (double X, double Y, double Z) b) =>
        (a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);

    private static (double X, double Y, double Z) Normalize((double X, double Y, double Z) v)
    {
        var l = Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
        return l < 1e-12 ? (1, 0, 0) : (v.X / l, v.Y / l, v.Z / l);
    }

    /// <summary>Latitude/longitude in radians of a unit vector.</summary>
    public static (double Lat, double Lon) ToLatLon((double X, double Y, double Z) v) => (Math.Asin(Math.Clamp(v.Z, -1, 1)), Math.Atan2(v.Y, v.X));

    /// <summary>
    /// Draws the globe centered in <paramref name="bounds"/>. <paramref name="orientation"/> is the unit vector at the
    /// disc center; null draws a neutral, unpinned globe (a destination without coordinates never inherits a neighbor's pin).
    /// </summary>
    public void Render(SKCanvas canvas, SKRect bounds, (double X, double Y, double Z)? orientation)
    {
        if (_disposed)
        {
            return;
        }

        var pinned = orientation is not null;
        var o = orientation ?? (Math.Cos(20 * Math.PI / 180), 0, Math.Sin(20 * Math.PI / 180));
        if (_renderedOrientation is not { } r || Math.Abs(r.X - o.X) + Math.Abs(r.Y - o.Y) + Math.Abs(r.Z - o.Z) > 1e-4 || _renderedPinned != pinned)
        {
            RenderBuffer(o);
            _renderedOrientation = o;
            _renderedPinned = pinned;
        }

        var size = Math.Min(bounds.Width, bounds.Height);
        var dest = new SKRect(bounds.MidX - size / 2, bounds.MidY - size / 2, bounds.MidX + size / 2, bounds.MidY + size / 2);
        using var image = SKImage.FromBitmap(_buffer);
        canvas.DrawImage(image, dest, new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear), _bufferPaint);

        _rimPaint.Color = Rim;
        canvas.DrawCircle(dest.MidX, dest.MidY, size / 2 - .5f, _rimPaint);

        if (pinned)
        {
            _markerPaint.Color = Marker;
            _markerRingPaint.Color = Marker.WithAlpha(0x70);
            canvas.DrawCircle(dest.MidX, dest.MidY, Math.Max(1.4f, size * .07f), _markerPaint);
            canvas.DrawCircle(dest.MidX, dest.MidY, Math.Max(2.6f, size * .14f), _markerRingPaint);
        }
    }

    /// <summary>Inverse orthographic projection per buffer pixel; the center of the disc is the supplied unit vector.</summary>
    private void RenderBuffer((double X, double Y, double Z) center)
    {
        var (lat0, lon0) = ToLatLon(center);
        double sinLat0 = Math.Sin(lat0), cosLat0 = Math.Cos(lat0);
        var n = BufferSize;
        var half = n / 2d;
        var radius = half - 1;
        var ocean = Ocean;
        var land = Land;
        var pixels = _buffer.GetPixelSpan();
        var maskW = _maskW;
        var maskH = _maskH;

        for (var py = 0; py < n; py++)
        {
            for (var px = 0; px < n; px++)
            {
                var x = (px + .5 - half) / radius;
                var y = (half - (py + .5)) / radius;
                var rho2 = x * x + y * y;
                var i = (py * n + px) * 4;
                if (rho2 > 1)
                {
                    pixels[i] = pixels[i + 1] = pixels[i + 2] = pixels[i + 3] = 0;
                    continue;
                }

                var rho = Math.Sqrt(rho2);
                var c = Math.Asin(Math.Clamp(rho, 0, 1));
                double sinC = Math.Sin(c), cosC = Math.Cos(c);
                var lat = rho < 1e-9 ? lat0 : Math.Asin(cosC * sinLat0 + (y * sinC * cosLat0) / rho);
                var lon = rho < 1e-9 ? lon0 : lon0 + Math.Atan2(x * sinC, rho * cosLat0 * cosC - y * sinLat0 * sinC);

                var isLand = false;
                if (_mask is not null)
                {
                    var u = (lon / (2 * Math.PI) + .5) % 1;
                    if (u < 0) u += 1;
                    var v = .5 - lat / Math.PI;
                    var mx = Math.Clamp((int)(u * maskW), 0, maskW - 1);
                    var my = Math.Clamp((int)(v * maskH), 0, maskH - 1);
                    isLand = (_mask[my * (maskW / 8) + (mx >> 3)] & (0x80 >> (mx & 7))) != 0;
                }

                // Edge shading: darken toward the limb so the disc reads as a sphere.
                var shade = (float)(.72 + .28 * cosC);
                var col = isLand ? land : ocean;
                pixels[i] = (byte)(col.Red * shade);
                pixels[i + 1] = (byte)(col.Green * shade);
                pixels[i + 2] = (byte)(col.Blue * shade);
                pixels[i + 3] = 255;
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _buffer.Dispose();
        _bufferPaint.Dispose();
        _markerPaint.Dispose();
        _markerRingPaint.Dispose();
        _rimPaint.Dispose();
    }
}
