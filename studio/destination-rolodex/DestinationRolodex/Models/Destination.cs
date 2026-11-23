namespace DestinationRolodex.Models;

/// <summary>The two approved photo transitions. Nothing else is advertised (decision D02).</summary>
public enum PhotoTransitionKind
{
    MorphBlend = 0,
    LiquidWarp = 1,
}

public enum CardViewMode
{
    Browse,
    Look,
}

public enum DestinationMediaStatus
{
    Empty,
    Loading,
    Ready,
    Failed,
}

/// <summary>How the photo layer is currently being produced. Fallback paths are explicit, never relabelled as an effect.</summary>
public enum PhotoRendererStatus
{
    /// <summary>Runtime SkSL effect compiled and rendering on a supported Skia surface.</summary>
    Shader,
    /// <summary>Skia surface available, runtime effect unavailable: plain cover image with a short crossfade.</summary>
    PlainImage,
    /// <summary>No Skia surface on this target: XAML image fallback.</summary>
    Unavailable,
}

/// <summary>Normalized, top-left-oriented focal point (CSS object-position), not a pixel position.</summary>
public readonly record struct FocalPoint
{
    public double X { get; }
    public double Y { get; }

    public FocalPoint(double x, double y)
    {
        if (!double.IsFinite(x) || !double.IsFinite(y) || x < 0 || x > 1 || y < 0 || y > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(x), "Focal coordinates must be finite values in [0, 1].");
        }

        X = x;
        Y = y;
    }

    public static FocalPoint Center => new(.5, .5);

    /// <summary>Parses a CSS object-position such as "50% 52%".</summary>
    public static FocalPoint Parse(string? position)
    {
        if (string.IsNullOrWhiteSpace(position))
        {
            return Center;
        }

        var parts = position.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        double Read(int i) => parts.Length > i && double.TryParse(parts[i].TrimEnd('%'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var v)
            ? Math.Clamp(v / 100d, 0, 1)
            : .5;
        var x = Read(0);
        var y = parts.Length > 1 ? Read(1) : x;
        return new FocalPoint(x, y);
    }
}

/// <summary>Featured-place coordinates in degrees (the place in the photograph, not a country centroid).</summary>
public readonly record struct GeoPoint(double Latitude, double Longitude)
{
    public bool IsValid => double.IsFinite(Latitude) && double.IsFinite(Longitude) && Math.Abs(Latitude) <= 90 && Math.Abs(Longitude) <= 180;
}

/// <summary>One browsable destination. Immutable; the card never mutates it.</summary>
public sealed record Destination(
    string Id,
    string Country,
    string Location,
    string Region,
    string Description,
    Uri ImageUri,
    FocalPoint FocalPoint,
    string ImageDescription,
    string PhotoCredit,
    Uri? PhotoSource,
    string FallbackColor,
    GeoPoint? Geo = null);

public readonly record struct TransitionSettings
{
    public const double MinDurationMs = 350;
    public const double MaxDurationMs = 3000;

    public PhotoTransitionKind Kind { get; }
    public TimeSpan Duration { get; }
    public double Strength { get; }

    public TransitionSettings(PhotoTransitionKind kind, TimeSpan duration, double strength)
    {
        if (kind is not PhotoTransitionKind.MorphBlend and not PhotoTransitionKind.LiquidWarp)
        {
            throw new ArgumentOutOfRangeException(nameof(kind));
        }

        if (!double.IsFinite(strength))
        {
            throw new ArgumentOutOfRangeException(nameof(strength));
        }

        Kind = kind;
        Duration = TimeSpan.FromMilliseconds(Math.Clamp(duration.TotalMilliseconds, MinDurationMs, MaxDurationMs));
        Strength = Math.Clamp(strength, 0, 1);
    }

    public static TransitionSettings Default => new(PhotoTransitionKind.MorphBlend, TimeSpan.FromMilliseconds(1200), .60);
}
