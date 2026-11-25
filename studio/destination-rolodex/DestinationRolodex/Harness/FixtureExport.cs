#if DEBUG
using System.Globalization;
using System.Text;
using DestinationRolodex.Models;
using DestinationRolodex.Rendering;
using DestinationRolodex.Services;
using SkiaSharp;

namespace DestinationRolodex.Harness;

/// <summary>
/// Stage 1 evidence: renders raw photo-layer frames through the real PhotoRenderer into an offscreen raster surface.
/// Output: fixed-progress captures (0/.25/.5/.75/1) for both modes and both directions, an endpoint-equality check
/// against the static frame, a frame sequence per mode/direction for playback recordings, camera-extreme coverage
/// checks, and a JSON report. CPU raster backend: same SkSL, same geometry as the on-screen GPU path.
/// </summary>
internal static class FixtureExport
{
    private const int SceneWidth = 480;
    private const int SceneHeight = 530;
    private static readonly float[] Stops = { 0f, .25f, .5f, .75f, 1f };

    public static async Task<string> RunAsync(string directory, IReadOnlyList<Destination> catalog, Func<Uri, CancellationToken, Task<Stream>> opener)
    {
        Directory.CreateDirectory(directory);
        var report = new StringBuilder();
        report.AppendLine("{");
        report.AppendLine($"  \"date\": \"{DateTime.UtcNow:O}\",");
        report.AppendLine($"  \"sceneSize\": [{SceneWidth}, {SceneHeight}],");
        report.AppendLine($"  \"layerSize\": [{SceneWidth * 1.26:0.##}, {SceneHeight * 1.26:0.##}],");
        report.AppendLine("  \"backend\": \"SkiaSharp CPU raster (SKSurface.Create), same SkSL as the SKCanvasElement path\",");

        using var renderer = new PhotoRenderer();
        report.AppendLine($"  \"runtimeEffect\": {(renderer.HasRuntimeEffect ? "\"compiled\"" : Json(renderer.CompileError ?? "unknown failure"))},");
        if (!renderer.HasRuntimeEffect)
        {
            report.AppendLine("  \"status\": \"blocked\"");
            report.AppendLine("}");
            await File.WriteAllTextAsync(Path.Combine(directory, "report.json"), report.ToString());
            return renderer.CompileError ?? "SkSL compile failed";
        }

        using var cache = new DestinationMediaCache(opener);
        var byId = catalog.ToDictionary(d => d.Id);
        var ph = await cache.GetAsync(byId["ph"]);
        var jp = await cache.GetAsync(byId["jp"]);
        var it = await cache.GetAsync(byId["it"]);
        report.AppendLine("  \"photos\": {");
        report.AppendLine($"    \"ph\": [{ph.Width}, {ph.Height}], \"jp\": [{jp.Width}, {jp.Height}], \"it\": [{it.Width}, {it.Height}]");
        report.AppendLine("  },");

        var info = new SKImageInfo(SceneWidth, SceneHeight, SKColorType.Rgba8888, SKAlphaType.Premul);
        using var surface = SKSurface.Create(info);

        var pairs = new (string Name, DecodedPhoto A, Destination DA, DecodedPhoto B, Destination DB, float Direction)[]
        {
            ("ph-jp", ph, byId["ph"], jp, byId["jp"], 1),
            ("jp-ph", jp, byId["jp"], ph, byId["ph"], -1),
            ("jp-it", jp, byId["jp"], it, byId["it"], 1),
        };

        var fixedDir = Path.Combine(directory, "fixed");
        Directory.CreateDirectory(fixedDir);
        var captures = new List<string>();
        foreach (var pair in pairs)
        {
            foreach (var mode in new[] { PhotoTransitionKind.MorphBlend, PhotoTransitionKind.LiquidWarp })
            {
                foreach (var p in Stops)
                {
                    var frame = new PhotoFrame(pair.A, pair.B, pair.DA.FocalPoint, pair.DB.FocalPoint, p, .6f, pair.Direction, mode, CameraPose.Identity, SKColors.Black) { Scrim = 0 };
                    var name = $"{ModeName(mode)}-{pair.Name}-{(int)(p * 100):000}.png";
                    Save(surface, renderer, frame, Path.Combine(fixedDir, name));
                    captures.Add(name);
                }
            }
        }

        report.AppendLine($"  \"fixedCaptures\": [{string.Join(", ", captures.Select(Json))}],");

        // A09: endpoints must equal the static frames with the same crop/focal/camera, byte for byte.
        var endpointChecks = new List<string>();
        foreach (var pair in pairs)
        {
            var staticA = Render(surface, renderer, PhotoFrame.Static(pair.A, pair.DA.FocalPoint, CameraPose.Identity, SKColors.Black) with { Scrim = 0 });
            var staticB = Render(surface, renderer, PhotoFrame.Static(pair.B, pair.DB.FocalPoint, CameraPose.Identity, SKColors.Black) with { Scrim = 0 });
            foreach (var mode in new[] { PhotoTransitionKind.MorphBlend, PhotoTransitionKind.LiquidWarp })
            {
                var at0 = Render(surface, renderer, new PhotoFrame(pair.A, pair.B, pair.DA.FocalPoint, pair.DB.FocalPoint, 0, .6f, pair.Direction, mode, CameraPose.Identity, SKColors.Black) { Scrim = 0 });
                var at1 = Render(surface, renderer, new PhotoFrame(pair.A, pair.B, pair.DA.FocalPoint, pair.DB.FocalPoint, 1, .6f, pair.Direction, mode, CameraPose.Identity, SKColors.Black) { Scrim = 0 });
                var mid = Render(surface, renderer, new PhotoFrame(pair.A, pair.B, pair.DA.FocalPoint, pair.DB.FocalPoint, .5f, .6f, pair.Direction, mode, CameraPose.Identity, SKColors.Black) { Scrim = 0 });
                var fade = Render(surface, renderer, new PhotoFrame(pair.A, pair.B, pair.DA.FocalPoint, pair.DB.FocalPoint, .5f, .6f, pair.Direction, mode, CameraPose.Identity, SKColors.Black) { Scrim = 0, DiagnosticCrossfade = true });
                endpointChecks.Add($"{{ \"pair\": \"{pair.Name}\", \"mode\": \"{ModeName(mode)}\", \"maxDiffAt0\": {MaxDiff(at0, staticA)}, \"maxDiffAt1\": {MaxDiff(at1, staticB)}, \"midpointVsCrossfadeMeanDiff\": {MeanDiff(mid, fade):0.##}, \"midpointVsSourceMeanDiff\": {MeanDiff(mid, staticA):0.##} }}");
            }
        }

        report.AppendLine($"  \"endpointChecks\": [\n    {string.Join(",\n    ", endpointChecks)}\n  ],");

        // A13: camera extremes must never expose the layer edge. Magenta backdrop makes any exposure countable.
        var exposure = new List<string>();
        var magenta = new SKColor(255, 0, 255);
        foreach (var pose in new[]
                 {
                     new CameraPose(37, 37, 2.8f, 3.8f, 1f),
                     new CameraPose(-37, -37, -2.8f, -3.8f, 1f),
                     new CameraPose(37, -37, -2.8f, 3.8f, 1.65f),
                     new CameraPose(-37, 37, 2.8f, -3.8f, 1.65f),
                     new CameraPose(12, 12, 1.25f, 1.6f, 1f),
                 })
        {
            var px = Render(surface, renderer, PhotoFrame.Static(it, byId["it"].FocalPoint, pose, magenta) with { Scrim = 0 });
            var count = CountColor(px, magenta);
            exposure.Add($"{{ \"pose\": \"t({pose.TranslateX},{pose.TranslateY}) rx{pose.RotateXDegrees} ry{pose.RotateYDegrees} z{pose.Zoom}\", \"exposedPixels\": {count} }}");
            Save(surface, renderer, PhotoFrame.Static(it, byId["it"].FocalPoint, pose, magenta) with { Scrim = 0 }, Path.Combine(fixedDir, $"camera-{pose.TranslateX}-{pose.TranslateY}-z{pose.Zoom.ToString(CultureInfo.InvariantCulture)}.png"));
        }

        report.AppendLine($"  \"cameraCoverage\": [\n    {string.Join(",\n    ", exposure)}\n  ],");

        // A28: frame sequences for playback recordings (1200 ms at 30 fps = 36 frames + endpoints).
        var sequenceDir = Path.Combine(directory, "frames");
        var sequences = new List<string>();
        foreach (var pair in pairs.Take(2))
        {
            foreach (var mode in new[] { PhotoTransitionKind.MorphBlend, PhotoTransitionKind.LiquidWarp })
            {
                var folder = Path.Combine(sequenceDir, $"{ModeName(mode)}-{pair.Name}");
                Directory.CreateDirectory(folder);
                const int frames = 37;
                for (var i = 0; i < frames; i++)
                {
                    var p = i / (float)(frames - 1);
                    var frame = new PhotoFrame(pair.A, pair.B, pair.DA.FocalPoint, pair.DB.FocalPoint, p, .6f, pair.Direction, mode, CameraPose.Identity, SKColors.Black) { Scrim = 0 };
                    Save(surface, renderer, frame, Path.Combine(folder, $"f{i:000}.png"));
                }

                sequences.Add($"frames/{ModeName(mode)}-{pair.Name} ({frames} frames)");
            }
        }

        report.AppendLine($"  \"sequences\": [{string.Join(", ", sequences.Select(Json))}],");
        var timings = renderer.RecentRenderMilliseconds.OrderBy(v => v).ToArray();
        report.AppendLine($"  \"cpuRasterRenderMs\": {{ \"count\": {timings.Length}, \"p50\": {Percentile(timings, .5):0.##}, \"p95\": {Percentile(timings, .95):0.##}, \"max\": {(timings.Length > 0 ? timings[^1] : 0):0.##} }},");
        report.AppendLine("  \"status\": \"ok\"");
        report.AppendLine("}");
        await File.WriteAllTextAsync(Path.Combine(directory, "report.json"), report.ToString());
        return "ok";
    }

    private static string ModeName(PhotoTransitionKind kind) => kind == PhotoTransitionKind.MorphBlend ? "morph" : "liquid";

    private static byte[] Render(SKSurface surface, PhotoRenderer renderer, PhotoFrame frame)
    {
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);
        canvas.ResetMatrix();
        renderer.Render(canvas, SceneWidth, SceneHeight, frame);
        canvas.Flush();
        using var image = surface.Snapshot();
        using var pixmap = image.PeekPixels();
        var bytes = new byte[pixmap.RowBytes * pixmap.Height];
        System.Runtime.InteropServices.Marshal.Copy(pixmap.GetPixels(), bytes, 0, bytes.Length);
        return bytes;
    }

    private static void Save(SKSurface surface, PhotoRenderer renderer, PhotoFrame frame, string path)
    {
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);
        canvas.ResetMatrix();
        renderer.Render(canvas, SceneWidth, SceneHeight, frame);
        canvas.Flush();
        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var file = File.Create(path);
        data.SaveTo(file);
    }

    private static int MaxDiff(byte[] a, byte[] b)
    {
        var max = 0;
        for (var i = 0; i < a.Length; i++)
        {
            max = Math.Max(max, Math.Abs(a[i] - b[i]));
        }

        return max;
    }

    private static double MeanDiff(byte[] a, byte[] b)
    {
        long sum = 0;
        for (var i = 0; i < a.Length; i++)
        {
            sum += Math.Abs(a[i] - b[i]);
        }

        return sum / (double)a.Length;
    }

    private static int CountColor(byte[] rgba, SKColor color)
    {
        var count = 0;
        for (var i = 0; i + 3 < rgba.Length; i += 4)
        {
            if (Math.Abs(rgba[i] - color.Red) < 8 && Math.Abs(rgba[i + 1] - color.Green) < 8 && Math.Abs(rgba[i + 2] - color.Blue) < 8)
            {
                count++;
            }
        }

        return count;
    }

    private static double Percentile(double[] sorted, double p) => sorted.Length == 0 ? 0 : sorted[Math.Min(sorted.Length - 1, (int)Math.Floor(p * (sorted.Length - 1)))];

    private static string Json(string s) => "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "") + "\"";
}
#endif
