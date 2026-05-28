using System.IO;
using OpenCvSharp;
using SkiaSharp;
using VTrack.DataContracts;

namespace VTrack.Services;

/// <summary>
/// Windows-desktop video decode + YOLO detection pipeline. Decoding and overlay
/// composition run on the UI thread (via <see cref="Advance"/>); inference runs on a
/// background task and its results are consumed on the next <see cref="Advance"/>.
/// </summary>
public sealed class TrackingEngine : ITrackingEngine
{
    private static readonly string[] DetectionColors =
    {
        "#FF5722", "#4CAF50", "#2196F3", "#9C27B0", "#FFC107",
        "#00BCD4", "#E91E63", "#8BC34A", "#3F51B5", "#FF9800"
    };

    // Casual English → COCO class. Anything outside this set falls through to a substring search.
    private static readonly Dictionary<string, string> SynonymToClass = new(StringComparer.OrdinalIgnoreCase)
    {
        ["person"] = "person", ["people"] = "person", ["human"] = "person",
        ["man"] = "person", ["men"] = "person", ["woman"] = "person", ["women"] = "person",
        ["kid"] = "person", ["kids"] = "person", ["child"] = "person", ["children"] = "person",
        ["boy"] = "person", ["girl"] = "person", ["adult"] = "person",

        ["ball"] = "sports ball", ["balls"] = "sports ball",
        ["dog"] = "dog", ["dogs"] = "dog", ["puppy"] = "dog",
        ["cat"] = "cat", ["cats"] = "cat", ["kitten"] = "cat",
        ["car"] = "car", ["cars"] = "car", ["vehicle"] = "car", ["auto"] = "car",
        ["bike"] = "bicycle", ["bicycle"] = "bicycle",
        ["motorbike"] = "motorcycle", ["motorcycle"] = "motorcycle",
        ["truck"] = "truck", ["bus"] = "bus", ["train"] = "train",
        ["horse"] = "horse", ["cow"] = "cow", ["sheep"] = "sheep", ["bird"] = "bird",
        ["phone"] = "cell phone", ["cellphone"] = "cell phone", ["mobile"] = "cell phone",
        ["computer"] = "laptop", ["laptop"] = "laptop",
        ["tv"] = "tv", ["television"] = "tv",
        ["chair"] = "chair", ["seat"] = "chair",
        ["bottle"] = "bottle", ["drink"] = "bottle",
        ["cup"] = "cup", ["glass"] = "cup", ["mug"] = "cup",
        ["backpack"] = "backpack", ["bag"] = "backpack",
        ["umbrella"] = "umbrella",
    };

    private static readonly char[] QuerySplitters = { ' ', ',', ';', '/', '+', '&' };

    private const double EventDedupeSeconds = 0.5;
    private const int MaxEventLogSize = 500;

    private VideoCapture? _capture;
    private Mat? _bgraMat;
    private Mat? _frame;        // reused per tick to avoid GC pressure
    private Mat? _detectFrame;  // reused for handoff to the inference worker
    private SKBitmap? _frameBitmap;
    private double _fps = 30;
    private int _totalFrames;
    private bool _isPlaying;

    private YoloDetector? _yolo;
    private bool _detectionEnabled;

    // Inference handoff between the UI thread and the background worker.
    private readonly object _gate = new();
    private bool _inferenceInFlight;
    private IReadOnlyList<Detection>? _pendingDetections;
    private double _pendingInferenceMs;

    private IReadOnlyList<Detection> _latestDetections = Array.Empty<Detection>();
    private readonly Dictionary<string, string> _classColors = new();
    private int _nextColorIdx;
    private HashSet<string> _allowedClasses = new(StringComparer.OrdinalIgnoreCase);
    private bool _matchAll;
    private int _framesSinceLastPerfStatus;

    private readonly List<TrackedSubject> _eventLog = new();
    private readonly Dictionary<string, double> _lastEventTimeByClass = new(StringComparer.OrdinalIgnoreCase);
    private int _eventCounter;

    public bool IsOpen => _capture != null;
    public double Fps => _fps;
    public double DurationSeconds => _totalFrames / _fps;
    public int TotalFrames => _totalFrames;

    public OpenResult Open(string? path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            return OpenResult.Fail($"File not found: {path}");
        }

        try
        {
            _capture = new VideoCapture(path, VideoCaptureAPIs.FFMPEG);
            if (!_capture.IsOpened())
            {
                _capture.Dispose();
                _capture = null;
                return OpenResult.Fail($"Could not open: {path}");
            }

            _fps = _capture.Fps > 0 ? _capture.Fps : 30;
            _totalFrames = _capture.FrameCount;
            _bgraMat = new Mat();
            _frame = new Mat();
            _isPlaying = true;

            return new OpenResult(true, null, DurationSeconds, _fps, _totalFrames);
        }
        catch (Exception ex)
        {
            return OpenResult.Fail($"Open error: {ex.Message}");
        }
    }

    public void Play() => _isPlaying = true;
    public void Pause() => _isPlaying = false;

    public void Seek(double seconds)
    {
        if (_capture == null || _totalFrames <= 0) return;
        _capture.PosFrames = Math.Clamp((int)(seconds * _fps), 0, _totalFrames - 1);
    }

    public async Task StartTrackingAsync(string query, IProgress<string>? status, CancellationToken ct)
    {
        if (_capture == null) return;

        // Reset tracking state for the new query.
        _classColors.Clear();
        _nextColorIdx = 0;
        _latestDetections = Array.Empty<Detection>();
        _eventLog.Clear();
        _lastEventTimeByClass.Clear();
        _eventCounter = 0;
        lock (_gate) _pendingDetections = null;

        _yolo ??= new YoloDetector();
        if (!_yolo.IsReady)
        {
            await _yolo.LoadAsync(status, ct);
        }

        ResolveAllowedClasses(query);

        if (_allowedClasses.Count == 0 && !_matchAll)
        {
            status?.Report($"'{query}' didn't match any class. Try: person, ball, car, dog, bicycle, ...");
            _detectionEnabled = false;
            return;
        }

        var allowedSummary = _matchAll ? "all classes" : string.Join(", ", _allowedClasses);
        status?.Report($"Tracking {allowedSummary}");
        _detectionEnabled = true;
    }

    public void StopTracking() => _detectionEnabled = false;

    public TrackingFrame? Advance()
    {
        if (_capture == null || _bgraMat == null || _frame == null) return null;

        // When paused, hold on the current frame without advancing or re-decoding.
        if (!_isPlaying && _frameBitmap != null)
        {
            return new TrackingFrame(_frameBitmap, Array.Empty<BoundingBox>(), Array.Empty<TrackedSubject>(),
                _capture.PosFrames / _fps, null, null);
        }

        if (!_capture.Read(_frame) || _frame.Empty())
        {
            _capture.PosFrames = 0;
            if (!_capture.Read(_frame) || _frame.Empty())
            {
                _isPlaying = false;
                return null;
            }
        }

        // BGR → BGRA for display.
        Cv2.CvtColor(_frame, _bgraMat, ColorConversionCodes.BGR2BGRA);

        var w = _bgraMat.Width;
        var h = _bgraMat.Height;
        if (_frameBitmap == null || _frameBitmap.Width != w || _frameBitmap.Height != h)
        {
            _frameBitmap?.Dispose();
            _frameBitmap = new SKBitmap(new SKImageInfo(w, h, SKColorType.Bgra8888, SKAlphaType.Opaque));
        }

        var byteCount = (long)_frameBitmap.RowBytes * h;
        unsafe
        {
            Buffer.MemoryCopy(
                _bgraMat.Data.ToPointer(),
                _frameBitmap.GetPixels().ToPointer(),
                byteCount,
                byteCount);
        }

        var currentFrameIdx = Math.Max(0, _capture.PosFrames - 1);
        string? status = null;
        IReadOnlyList<TrackedSubject>? eventLogUpdate = null;

        if (_detectionEnabled && _yolo is { IsReady: true })
        {
            // Consume the most recent completed inference, if any.
            IReadOnlyList<Detection>? completed = null;
            double completedMs = 0;
            lock (_gate)
            {
                if (_pendingDetections != null)
                {
                    completed = _pendingDetections;
                    completedMs = _pendingInferenceMs;
                    _pendingDetections = null;
                }
            }

            if (completed != null)
            {
                status = ConsumeDetections(completed, completedMs, currentFrameIdx, out eventLogUpdate);
            }

            // Kick off the next inference on a copy of the current frame.
            StartInferenceIfIdle();
        }

        var boxes = BuildOverlayBoxes(currentFrameIdx, w, h);
        var overlaySubjects = BuildOverlaySubjects();

        return new TrackingFrame(_frameBitmap, boxes, overlaySubjects, _capture.PosFrames / _fps, eventLogUpdate, status);
    }

    private void StartInferenceIfIdle()
    {
        if (_frame == null || _yolo is not { IsReady: true }) return;

        bool start;
        lock (_gate)
        {
            start = !_inferenceInFlight;
            if (start) _inferenceInFlight = true;
        }
        if (!start) return;

        _detectFrame ??= new Mat();
        _frame.CopyTo(_detectFrame);
        var captured = _detectFrame; // worker reads; UI thread won't touch until the worker finishes

        _ = Task.Run(() =>
        {
            IReadOnlyList<Detection> results;
            double ms;
            try
            {
                results = _yolo!.Detect(captured);
                ms = _yolo.LastInferenceMs;
            }
            catch
            {
                results = Array.Empty<Detection>();
                ms = 0;
            }
            lock (_gate)
            {
                _pendingDetections = results;
                _pendingInferenceMs = ms;
                _inferenceInFlight = false;
            }
        });
    }

    /// <summary>Filters a completed inference, updates the event log + color map, returns an optional status line.</summary>
    private string? ConsumeDetections(IReadOnlyList<Detection> detections, double inferenceMs, long frameIdx,
        out IReadOnlyList<TrackedSubject>? eventLogUpdate)
    {
        var filtered = _matchAll || _allowedClasses.Count == 0
            ? detections
            : detections.Where(d => _allowedClasses.Contains(d.Label)).ToList();
        _latestDetections = filtered;

        // Stable color per class for the overlay.
        foreach (var d in filtered)
        {
            if (_classColors.ContainsKey(d.Label)) continue;
            _classColors[d.Label] = DetectionColors[_nextColorIdx % DetectionColors.Length];
            _nextColorIdx++;
        }

        eventLogUpdate = AppendEventsToLog(filtered, frameIdx);

        // Surface inference time periodically so the user can see actual perf.
        _framesSinceLastPerfStatus++;
        if (_framesSinceLastPerfStatus >= 30 && _yolo != null)
        {
            _framesSinceLastPerfStatus = 0;
            var fps = inferenceMs > 0 ? 1000.0 / inferenceMs : 0;
            return $"{_yolo.BackendName} · {inferenceMs:F0} ms/frame · {fps:F1} fps";
        }
        return null;
    }

    private IReadOnlyList<TrackedSubject>? AppendEventsToLog(IReadOnlyList<Detection> filtered, long frameIdx)
    {
        if (_capture == null) return null;

        var nowSec = _capture.PosFrames / _fps;
        var added = false;

        foreach (var d in filtered)
        {
            // Dedupe: same class within EventDedupeSeconds counts as one continuous sighting.
            if (_lastEventTimeByClass.TryGetValue(d.Label, out var lastSec) &&
                nowSec - lastSec < EventDedupeSeconds)
            {
                _lastEventTimeByClass[d.Label] = nowSec;
                continue;
            }

            _lastEventTimeByClass[d.Label] = nowSec;
            if (!_classColors.TryGetValue(d.Label, out var color))
            {
                color = DetectionColors[_nextColorIdx % DetectionColors.Length];
                _nextColorIdx++;
                _classColors[d.Label] = color;
            }

            _eventCounter++;
            var timestamp = TimeSpan.FromSeconds(nowSec).ToString(@"m\:ss");
            _eventLog.Add(new TrackedSubject(
                Id: $"event-{_eventCounter}",
                Label: $"{d.Label} — {timestamp} ({d.Confidence:P0})",
                Color: color,
                Confidence: d.Confidence,
                FirstFrame: (int)frameIdx,
                LastFrame: (int)frameIdx));
            added = true;
        }

        if (!added) return null;

        // Cap the log to keep memory + UI bounded; drop oldest entries first.
        if (_eventLog.Count > MaxEventLogSize)
        {
            _eventLog.RemoveRange(0, _eventLog.Count - MaxEventLogSize);
        }

        // Show newest first.
        return _eventLog.AsEnumerable().Reverse().ToList();
    }

    private List<BoundingBox> BuildOverlayBoxes(long frameIdx, int w, int h)
    {
        if (!_detectionEnabled || _latestDetections.Count == 0)
        {
            return new List<BoundingBox>(0);
        }

        var boxes = new List<BoundingBox>(_latestDetections.Count);
        foreach (var d in _latestDetections)
        {
            boxes.Add(new BoundingBox(
                SubjectId: d.Label,
                Frame: (int)frameIdx,
                X: (double)d.Box.X / w,
                Y: (double)d.Box.Y / h,
                Width: (double)d.Box.Width / w,
                Height: (double)d.Box.Height / h,
                Confidence: d.Confidence));
        }
        return boxes;
    }

    /// <summary>
    /// One overlay subject per detected class, carrying its color. The canvas matches
    /// <see cref="BoundingBox.SubjectId"/> (the class label) to <see cref="TrackedSubject.Id"/>.
    /// </summary>
    private List<TrackedSubject> BuildOverlaySubjects()
    {
        if (!_detectionEnabled || _latestDetections.Count == 0)
        {
            return new List<TrackedSubject>(0);
        }

        var subjects = new List<TrackedSubject>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var d in _latestDetections)
        {
            if (!seen.Add(d.Label)) continue;
            var color = _classColors.TryGetValue(d.Label, out var c) ? c : DetectionColors[0];
            subjects.Add(new TrackedSubject(d.Label, d.Label, color, d.Confidence, 0, 0));
        }
        return subjects;
    }

    private void ResolveAllowedClasses(string query)
    {
        _allowedClasses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        _matchAll = false;

        if (string.IsNullOrWhiteSpace(query) || _yolo == null) return;

        var lower = query.ToLowerInvariant().Trim();
        if (lower is "all" or "everything" or "any" or "*")
        {
            _matchAll = true;
            return;
        }

        var words = lower.Split(QuerySplitters, StringSplitOptions.RemoveEmptyEntries);
        var cocoClasses = _yolo.ClassNames;

        foreach (var word in words)
        {
            if (SynonymToClass.TryGetValue(word, out var mapped))
            {
                _allowedClasses.Add(mapped);
                continue;
            }
            // Substring match against COCO class names ("ball" → "sports ball").
            foreach (var c in cocoClasses)
            {
                if (c.Contains(word, StringComparison.OrdinalIgnoreCase))
                {
                    _allowedClasses.Add(c);
                }
            }
        }
    }

    public void Dispose()
    {
        _capture?.Dispose();
        _capture = null;
        _bgraMat?.Dispose();
        _bgraMat = null;
        _frame?.Dispose();
        _frame = null;
        _detectFrame?.Dispose();
        _detectFrame = null;
        _frameBitmap?.Dispose();
        _frameBitmap = null;
        _yolo?.Dispose();
        _yolo = null;
        _detectionEnabled = false;
        _latestDetections = Array.Empty<Detection>();
        _classColors.Clear();
        _nextColorIdx = 0;
    }
}
