#if DEBUG
using System.Globalization;

namespace DestinationRolodex.Harness;

/// <summary>
/// Command-line hooks for deterministic verification runs (DEBUG only). The App MCP cannot set environment
/// variables, so every switch is an argument: --export-fixtures=DIR, --capture=FILE, --viewport=WxH, --mode=liquid,
/// --seek=0.5, --look, --reduced, --no-shader, --media-fail=jp, --media-delay=1500, --select=jp, --autoplay,
/// --second, --settle=1500, --perf=FILE, --steps=1000, --spike.
/// </summary>
internal sealed class DebugHarness
{
    public string? ExportFixtures { get; private set; }
    public string? CapturePath { get; private set; }
    public string? PerfPath { get; private set; }
    public int SettleMs { get; private set; } = 1500;
    public (int Width, int Height)? Viewport { get; private set; }
    public string? Mode { get; private set; }
    public double? Seek { get; private set; }
    public bool Look { get; private set; }
    public bool ReducedMotion { get; private set; }
    public bool NoShader { get; private set; }
    public string? MediaFail { get; private set; }
    public int MediaDelayMs { get; private set; }
    public string? Select { get; private set; }
    public bool AutoPlay { get; private set; }
    public bool SecondCard { get; private set; }
    public int Steps { get; private set; }
    public bool Spike { get; private set; }
    public bool Replay { get; private set; }
    public bool Empty { get; private set; }
    public bool Single { get; private set; }
    /// <summary>Fractional picker position for position-driven captures (LP03), e.g. 0.5 = halfway from index 0 to 1.</summary>
    public double? Position { get; private set; }
    public bool Crossfade { get; private set; }

    public bool IsActive => ExportFixtures is not null || CapturePath is not null || PerfPath is not null || Steps > 0;

    public static DebugHarness Parse(IEnumerable<string> args)
    {
        var h = new DebugHarness();
        foreach (var raw in args)
        {
            if (!raw.StartsWith("--", StringComparison.Ordinal))
            {
                continue;
            }

            var eq = raw.IndexOf('=');
            var key = (eq < 0 ? raw[2..] : raw[2..eq]).ToLowerInvariant();
            var value = eq < 0 ? string.Empty : raw[(eq + 1)..];
            switch (key)
            {
                case "export-fixtures": h.ExportFixtures = value; break;
                case "capture": h.CapturePath = value; break;
                case "perf": h.PerfPath = value; break;
                case "settle": h.SettleMs = int.Parse(value, CultureInfo.InvariantCulture); break;
                case "viewport":
                    var parts = value.Split('x');
                    h.Viewport = (int.Parse(parts[0], CultureInfo.InvariantCulture), int.Parse(parts[1], CultureInfo.InvariantCulture));
                    break;
                case "mode": h.Mode = value; break;
                case "seek": h.Seek = double.Parse(value, CultureInfo.InvariantCulture); break;
                case "look": h.Look = true; break;
                case "reduced": h.ReducedMotion = true; break;
                case "no-shader": h.NoShader = true; break;
                case "media-fail": h.MediaFail = value; break;
                case "media-delay": h.MediaDelayMs = int.Parse(value, CultureInfo.InvariantCulture); break;
                case "select": h.Select = value; break;
                case "autoplay": h.AutoPlay = true; break;
                case "second": h.SecondCard = true; break;
                case "steps": h.Steps = int.Parse(value, CultureInfo.InvariantCulture); break;
                case "spike": h.Spike = true; break;
                case "replay": h.Replay = true; break;
                case "empty": h.Empty = true; break;
                case "single": h.Single = true; break;
                case "position": h.Position = double.Parse(value, CultureInfo.InvariantCulture); break;
                case "crossfade": h.Crossfade = true; break;
            }
        }

        return h;
    }

    /// <summary>Desktop-only direct file opener for harness runs; the app itself uses ms-appx through StorageFile.</summary>
    public static Task<Stream> OpenLocalAsset(Uri uri, CancellationToken ct)
    {
        var relative = uri.AbsolutePath.TrimStart('/');
        var path = Path.Combine(AppContext.BaseDirectory, relative.Replace('/', Path.DirectorySeparatorChar));
        return Task.FromResult<Stream>(File.OpenRead(path));
    }
}
#endif
