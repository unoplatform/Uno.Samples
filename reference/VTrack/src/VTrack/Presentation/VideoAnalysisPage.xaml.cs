using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace VTrack.Presentation;

public sealed partial class VideoAnalysisPage : Page
{
    private VideoAnalysisModel? _model;
    private DispatcherQueueTimer? _frameTimer;
    private bool _suppressSliderSeek;

    public VideoAnalysisPage()
    {
        this.InitializeComponent();
        this.DataContextChanged += OnDataContextChanged;
        this.Unloaded += OnUnloaded;
    }

    private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if (args.NewValue is not VideoAnalysisViewModel vm) return;
        if (_model == vm.Model) return;

        StopSession();

        _model = vm.Model;
        var result = _model.StartSession();
        if (!result.Success) return;

        var fps = result.Fps > 0 ? result.Fps : 30;
        _frameTimer = DispatcherQueue.CreateTimer();
        _frameTimer.Interval = TimeSpan.FromMilliseconds(1000.0 / fps);
        _frameTimer.IsRepeating = true;
        _frameTimer.Tick += OnFrameTick;
        _frameTimer.Start();
    }

    private void OnFrameTick(DispatcherQueueTimer sender, object args)
    {
        var frame = _model?.RenderTick();
        if (frame is null) return;

        if (frame.Frame is not null)
        {
            VideoCanvas.SetFrame(frame.Frame);
        }
        VideoCanvas.SetOverlay(frame.Boxes, frame.OverlaySubjects);

        // Keep the timeline slider in sync without re-triggering a seek.
        _suppressSliderSeek = true;
        TimelineSlider.Value = frame.PositionSeconds;
        _suppressSliderSeek = false;
    }

    private void OnTimelineSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (_suppressSliderSeek) return;
        _model?.Seek(e.NewValue);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e) => StopSession();

    private void StopSession()
    {
        if (_frameTimer is not null)
        {
            _frameTimer.Stop();
            _frameTimer.Tick -= OnFrameTick;
            _frameTimer = null;
        }
        _model?.EndSession();
    }
}
