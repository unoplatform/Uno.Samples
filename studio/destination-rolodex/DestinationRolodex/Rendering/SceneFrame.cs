using DestinationRolodex.Models;
using SkiaSharp;

namespace DestinationRolodex.Rendering;

/// <summary>
/// One frame sample derived from the continuous picker position (brief §3). Everything that moves with browsing reads
/// this record: row placement, photo uniforms (adjacent ordered pair + fraction), globe orientation, photo-in-type,
/// caption treatment, material colors and the arrival camera.
/// </summary>
public readonly record struct SceneFrame(double Q, double P, int FromIndex, int ToIndex, int Selected, double Travel, int Count)
{
    public static SceneFrame Sample(double q, int count)
    {
        if (count <= 0)
        {
            return new SceneFrame(q, 0, -1, -1, -1, 0, 0);
        }

        var low = (long)Math.Floor(q);
        var p = q - low;
        return new SceneFrame(
            q,
            p,
            CircularMath.PositiveModulo(low, count),
            CircularMath.PositiveModulo(low + 1, count),
            CircularMath.PositiveModulo((long)Math.Floor(q + .5), count),
            Math.Sin(Math.PI * p),
            count);
    }

    public bool IsAtRest => P <= 1e-6 || P >= 1 - 1e-6;

    /// <summary>Photo-in-type overlay opacity for a row at signed distance <paramref name="rowDistance"/> (zero at arrival).</summary>
    public double PhotoInTypeOpacity(double rowDistance) => Travel * .60 * Math.Max(0, 1 - Math.Abs(rowDistance) * .72);

    /// <summary>Caption opacity in flight: at most a 17 % reduction.</summary>
    public double CaptionOpacity => 1 - .17 * Travel;

    /// <summary>Caption downward movement in flight: at most 2 units.</summary>
    public double CaptionOffsetY => 2 * Travel;

    /// <summary>Arrival camera: up to 1.2 % scale and 2 units vertical travel, both zero at rest.</summary>
    public double ArrivalScale => 1 + .012 * Travel;
    public double ArrivalOffsetY => 2 * Travel;
}

/// <summary>Authored color interpolation (brief §4): pigment lerp by p, then fixed neutral/warm mixes. No photo sampling.</summary>
public readonly record struct MaterialColors(SKColor Pigment, SKColor Ink, SKColor Accent, SKColor Shadow)
{
    private static readonly SKColor Neutral = new(24, 25, 23);
    private static readonly SKColor Warm = new(255, 247, 224);

    public static MaterialColors From(SKColor from, SKColor to, double p)
    {
        var pigment = Lerp(from, to, Math.Clamp(p, 0, 1));
        var ink = Mix(Neutral, pigment, .15);
        var accent = Mix(Warm, pigment, .20);
        var shadow = Mix(Neutral, pigment, .35);
        return new MaterialColors(pigment, ink, accent, shadow);
    }

    public static SKColor Lerp(SKColor a, SKColor b, double t) => new(
        (byte)Math.Round(a.Red + (b.Red - a.Red) * t),
        (byte)Math.Round(a.Green + (b.Green - a.Green) * t),
        (byte)Math.Round(a.Blue + (b.Blue - a.Blue) * t),
        255);

    /// <summary>base × (1 − amount) + pigment × amount.</summary>
    public static SKColor Mix(SKColor baseColor, SKColor pigment, double amount) => Lerp(baseColor, pigment, amount);
}
