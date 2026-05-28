using VTrack.Services;

namespace VTrack.Presentation;

public partial record VideoAnalysisModel
{
    // Segoe Fluent / MDL2 glyphs.
    private const string PlayGlyph = "";
    private const string PauseGlyph = "";
    private const string VolumeGlyph = "";
    private const string MuteGlyph = "";

    private readonly INavigator _navigator;
    private readonly ILogger<VideoAnalysisModel> _logger;
    private readonly ITrackingEngine _engine;
    private readonly VideoFile _videoFile;

    public VideoAnalysisModel(
        INavigator navigator,
        ILogger<VideoAnalysisModel> logger,
        ITrackingEngine engine,
        VideoFile videoFile)
    {
        _navigator = navigator;
        _logger = logger;
        _engine = engine;
        _videoFile = videoFile;
    }

    public string VideoName => _videoFile.Name;
    public string VideoUrl => _videoFile.VideoUrl;

    // Playback state
    public IState<bool> IsPlaying => State<bool>.Value(this, () => false);
    public IState<double> CurrentPosition => State<double>.Value(this, () => 0);
    public IState<double> VideoDuration => State<double>.Value(this, () => 100);
    public IState<bool> IsMuted => State<bool>.Value(this, () => false);

    // Tracking state
    public IState<string> QueryText => State<string>.Value(this, () => string.Empty);
    public IState<bool> IsProcessing => State<bool>.Value(this, () => false);
    public IState<double> ProcessingProgress => State<double>.Value(this, () => 0);
    public IListState<TrackedSubject> TrackedSubjects => ListState<TrackedSubject>.Empty(this);

    public IState<bool> HasSubjects => State<bool>.Value(this, () => false);
    public IState<bool> ShowEmptyState => State<bool>.Value(this, () => true);
    public IFeed<bool> CanStartTracking => QueryText.Select(text => !string.IsNullOrWhiteSpace(text));
    public IState<string> PlayPauseIcon => State<string>.Value(this, () => PauseGlyph);
    public IState<string> MuteIcon => State<string>.Value(this, () => VolumeGlyph);
    public IState<int> CurrentFrame => State<int>.Value(this, () => 0);
    public IState<int> TotalFrames => State<int>.Value(this, () => 0);
    public IState<string> CurrentTimeText => State<string>.Value(this, () => "00:00");
    public IState<string> DurationText => State<string>.Value(this, () => "00:00");
    public IState<string> MediaError => State<string>.Value(this, () => string.Empty);
    public IState<string> FrameCounterText => State<string>.Value(this, () => "F 000 / 000");

    /// <summary>Opens the video. Called by the page on data-context change; the page uses the
    /// returned fps to drive its render timer.</summary>
    public OpenResult StartSession()
    {
        var result = _engine.Open(_videoFile.VideoUrl);
        if (result.Success)
        {
            _ = VideoDuration.SetAsync(result.DurationSeconds, CancellationToken.None);
            _ = TotalFrames.SetAsync((int)(result.DurationSeconds * result.Fps), CancellationToken.None);
            _ = DurationText.SetAsync(TimeSpan.FromSeconds(result.DurationSeconds).ToString(@"mm\:ss"), CancellationToken.None);
            _ = IsPlaying.SetAsync(true, CancellationToken.None);
            _ = PlayPauseIcon.SetAsync(PauseGlyph, CancellationToken.None);
        }
        else if (result.Error is { } error)
        {
            _ = MediaError.SetAsync(error, CancellationToken.None);
        }
        return result;
    }

    /// <summary>Advances one video frame and reflects it into bindable state.
    /// Returns the frame for the page to render. Called from the page's render timer.</summary>
    public TrackingFrame? RenderTick()
    {
        var frame = _engine.Advance();
        if (frame is null) return null;

        // Per-frame UI updates run at video fps; fire-and-forget is deliberate here — awaiting
        // each state write per frame would serialize the render loop. One-shot updates
        // (StartTracking, session open) are awaited normally.
        _ = CurrentPosition.SetAsync(frame.PositionSeconds, CancellationToken.None);
        var frameIdx = (int)(frame.PositionSeconds * _engine.Fps);
        _ = CurrentFrame.SetAsync(frameIdx, CancellationToken.None);
        _ = CurrentTimeText.SetAsync(TimeSpan.FromSeconds(frame.PositionSeconds).ToString(@"mm\:ss"), CancellationToken.None);
        _ = FrameCounterText.SetAsync($"F {frameIdx:000} / {_engine.TotalFrames:000}", CancellationToken.None);

        if (frame.EventLogUpdate is { } events)
        {
            _ = TrackedSubjects.UpdateAsync(_ => events.ToImmutableList(), CancellationToken.None);
            _ = HasSubjects.SetAsync(events.Count > 0, CancellationToken.None);
        }

        if (frame.Status is { } status)
        {
            _ = MediaError.SetAsync(status, CancellationToken.None);
        }

        return frame;
    }

    public void Seek(double seconds) => _engine.Seek(seconds);

    public void EndSession() => _engine.Dispose();

    public async ValueTask GoBack(CancellationToken ct)
    {
        await _navigator.GoBack(this);
    }

    public async ValueTask PlayPause(CancellationToken ct)
    {
        var isPlaying = await IsPlaying.Value(ct);
        var newIsPlaying = !isPlaying;
        await IsPlaying.SetAsync(newIsPlaying, ct);
        await PlayPauseIcon.SetAsync(newIsPlaying ? PauseGlyph : PlayGlyph, ct);

        if (newIsPlaying) _engine.Play();
        else _engine.Pause();
    }

    public async ValueTask ToggleMute(CancellationToken ct)
    {
        var isMuted = await IsMuted.Value(ct);
        await IsMuted.SetAsync(!isMuted, ct);
        await MuteIcon.SetAsync(isMuted ? VolumeGlyph : MuteGlyph, ct);
    }

    public async ValueTask StartTracking(CancellationToken ct)
    {
        var query = await QueryText.Value(ct);
        if (string.IsNullOrWhiteSpace(query)) return;

        await IsProcessing.SetAsync(true, ct);
        await ShowEmptyState.SetAsync(false, ct);
        await MediaError.SetAsync(string.Empty, ct);

        try
        {
            _logger.LogInformation("Starting live tracking: query='{Query}'", query);
            var progress = new Progress<string>(s => _ = MediaError.SetAsync(s, CancellationToken.None));
            await _engine.StartTrackingAsync(query, progress, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tracking init failed");
            await MediaError.SetAsync($"Tracking error: {ex.Message}", ct);
            await ShowEmptyState.SetAsync(true, ct);
        }
        finally
        {
            await IsProcessing.SetAsync(false, ct);
        }
    }
}
