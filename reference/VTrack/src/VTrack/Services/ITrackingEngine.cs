using SkiaSharp;
using VTrack.DataContracts;

namespace VTrack.Services;

/// <summary>Result of opening a video source.</summary>
public sealed record OpenResult(bool Success, string? Error, double DurationSeconds, double Fps, int TotalFrames)
{
    public static OpenResult Fail(string error) => new(false, error, 0, 0, 0);
}

/// <summary>
/// One decoded frame plus the overlay/tracking output computed for it.
/// Produced on the UI thread by <see cref="ITrackingEngine.Advance"/>.
/// </summary>
public sealed record TrackingFrame(
    SKBitmap? Frame,
    IReadOnlyList<BoundingBox> Boxes,
    IReadOnlyList<TrackedSubject> OverlaySubjects,
    double PositionSeconds,
    IReadOnlyList<TrackedSubject>? EventLogUpdate,
    string? Status);

/// <summary>
/// Owns the video decode + object-detection pipeline. The Windows-desktop build uses the
/// OpenCV/YOLO <c>TrackingEngine</c>; other platforms get <see cref="NullTrackingEngine"/>.
/// All members are called from the UI thread except internal inference, which the engine
/// marshals back itself.
/// </summary>
public interface ITrackingEngine : IDisposable
{
    bool IsOpen { get; }
    double Fps { get; }
    double DurationSeconds { get; }
    int TotalFrames { get; }

    OpenResult Open(string? path);
    void Play();
    void Pause();
    void Seek(double seconds);

    /// <summary>Loads the model (if needed) and resolves the query into a class filter.</summary>
    Task StartTrackingAsync(string query, IProgress<string>? status, CancellationToken ct);
    void StopTracking();

    /// <summary>Advance one frame. Returns null when no video is open.</summary>
    TrackingFrame? Advance();
}

/// <summary>No-op engine for platforms without the OpenCV backend (wasm/android/ios).</summary>
public sealed class NullTrackingEngine : ITrackingEngine
{
    private const string Unsupported = "Video detection is only available on the Windows desktop build.";

    public bool IsOpen => false;
    public double Fps => 30;
    public double DurationSeconds => 0;
    public int TotalFrames => 0;

    public OpenResult Open(string? path) => OpenResult.Fail(Unsupported);
    public void Play() { }
    public void Pause() { }
    public void Seek(double seconds) { }

    public Task StartTrackingAsync(string query, IProgress<string>? status, CancellationToken ct)
    {
        status?.Report(Unsupported);
        return Task.CompletedTask;
    }

    public void StopTracking() { }
    public TrackingFrame? Advance() => null;
    public void Dispose() { }
}
