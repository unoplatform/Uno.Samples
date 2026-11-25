using DestinationRolodex.Models;

namespace DestinationRolodex.Presentation;

/// <summary>
/// Demo page state: effect comparison, tuning, timeline and diagnostics. It never owns navigation, photos or frames;
/// the page forwards replay/seek requests to the card, which performs them.
/// </summary>
public sealed partial class DestinationDemoViewModel : ObservableObject
{
    [ObservableProperty]
    private IReadOnlyList<Destination> _destinations = Array.Empty<Destination>();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsMorph))]
    [NotifyPropertyChangedFor(nameof(IsLiquid))]
    private PhotoTransitionKind _transitionKind = PhotoTransitionKind.MorphBlend;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Duration))]
    [NotifyPropertyChangedFor(nameof(DurationText))]
    private double _durationMs = 1200;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Strength))]
    [NotifyPropertyChangedFor(nameof(StrengthText))]
    private double _strengthPercent = 60;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TimelineText))]
    private double _timeline;

    [ObservableProperty]
    private string _routeFrom = "Philippines";

    [ObservableProperty]
    private string _routeTo = "Japan";

    [ObservableProperty]
    private string _rendererStatus = "Preparing renderer";

    [ObservableProperty]
    private bool _isFallback;

    [ObservableProperty]
    private bool _isReducedMotion;

    [ObservableProperty]
    private bool _showSecondCard;

    [ObservableProperty]
    private bool _isReplaying;

    /// <summary>Lab-only diagnostic dissolve; the two production effects stay the only advertised modes.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsMorph))]
    [NotifyPropertyChangedFor(nameof(IsLiquid))]
    private bool _isDiagnosticCrossfade;

    public bool IsMorph => !IsDiagnosticCrossfade && TransitionKind == PhotoTransitionKind.MorphBlend;
    public bool IsLiquid => !IsDiagnosticCrossfade && TransitionKind == PhotoTransitionKind.LiquidWarp;
    public TimeSpan Duration => TimeSpan.FromMilliseconds(DurationMs);
    public double Strength => StrengthPercent / 100d;
    public string DurationText => $"{DurationMs / 1000d:0.00} s";
    public string StrengthText => $"{StrengthPercent:0}%";
    public string TimelineText => $"{Timeline / 10d:0}%";
    public string CountText => $"{Destinations.Count} destinations";

    /// <summary>The page performs replay/seek on the card and reports back through <see cref="RendererStatus"/>.</summary>
    public event EventHandler? ReplayRequested;
    public event EventHandler<double>? SeekRequested;

    partial void OnDestinationsChanged(IReadOnlyList<Destination> value) => OnPropertyChanged(nameof(CountText));

    [RelayCommand]
    private void ChooseMorph() => Choose(PhotoTransitionKind.MorphBlend);

    [RelayCommand]
    private void ChooseLiquid() => Choose(PhotoTransitionKind.LiquidWarp);

    public void ChooseDiagnosticCrossfade()
    {
        IsDiagnosticCrossfade = true;
        Replay();
    }

    public void Choose(PhotoTransitionKind kind, bool replay = true)
    {
        IsDiagnosticCrossfade = false;
        TransitionKind = kind;
        if (replay)
        {
            Replay();
        }
    }

    [RelayCommand]
    public void Replay() => ReplayRequested?.Invoke(this, EventArgs.Empty);

    public void Seek(double progress)
    {
        Timeline = Math.Clamp(progress, 0, 1) * 1000;
        SeekRequested?.Invoke(this, Math.Clamp(progress, 0, 1));
    }

    public void ReportTransitionStart(string from, string to)
    {
        RouteFrom = from;
        RouteTo = to;
        RendererStatus = "Skia · live";
        IsFallback = false;
    }

    public void ReportProgress(double progress) => Timeline = progress * 1000;

    public void ReportRendererReady(PhotoRendererStatus status, string? detail)
    {
        switch (status)
        {
            case PhotoRendererStatus.Shader:
                RendererStatus = IsReducedMotion ? "Reduced motion enabled" : "Skia runtime effect ready";
                IsFallback = false;
                break;
            case PhotoRendererStatus.PlainImage:
                RendererStatus = $"Plain photo crossfade · runtime effect unavailable ({detail})";
                IsFallback = true;
                break;
            default:
                RendererStatus = "Plain photo · Skia surface unavailable on this target";
                IsFallback = true;
                break;
        }
    }

    public void ReportFallback(string message)
    {
        RendererStatus = message;
        IsFallback = true;
    }
}
