using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls.Primitives;

using System.Diagnostics;

namespace LottieBuiltIn;

public sealed partial class LottieCard : UserControl
{
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(33) };
    private readonly Stopwatch _clock = new();
    private double _startProgress;
    private bool _updatingFromTimer;

    public LottieCard()
    {
        InitializeComponent();
        _timer.Tick += OnTick;
        Loaded += (_, _) =>
        {
            _startProgress = 0;
            _clock.Restart();
            _timer.Start();
        };
        Unloaded += (_, _) => _timer.Stop();
    }

    public string Key { get; set; } = "";

    public string Title
    {
        get => TitleText.Text;
        set => TitleText.Text = value;
    }

    public string Caption
    {
        get => CaptionText.Text;
        set => CaptionText.Text = value;
    }

    public string AnimationUri
    {
        set => Source.UriSource = new Uri(value);
    }

    public double PlaybackRate
    {
        get => Player.PlaybackRate;
        set => Player.PlaybackRate = value;
    }

    public void SetAutomationIds()
    {
        AutomationProperties.SetAutomationId(Player, $"{Key}Player");
        AutomationProperties.SetAutomationId(PlayButton, $"{Key}Play");
        AutomationProperties.SetAutomationId(PauseButton, $"{Key}Pause");
        AutomationProperties.SetAutomationId(StopButton, $"{Key}Stop");
        AutomationProperties.SetAutomationId(ProgressSlider, $"{Key}Progress");
    }

    private void OnPlay(object sender, RoutedEventArgs e)
    {
        _startProgress = ProgressSlider.Value >= 1 ? 0 : ProgressSlider.Value;
        _clock.Restart();
        _timer.Start();
        _ = Player.PlayAsync(_startProgress, 1, looped: true);
    }

    private void OnPause(object sender, RoutedEventArgs e)
    {
        _timer.Stop();
        Player.Pause();
    }

    private void OnStop(object sender, RoutedEventArgs e)
    {
        _timer.Stop();
        Player.Stop();
        SetSlider(0);
    }

    private void OnProgressChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (_updatingFromTimer)
        {
            return;
        }

        _timer.Stop();
        Player.SetProgress(e.NewValue);
    }

    private void OnTick(object? sender, object e)
    {
        var duration = Player.Duration.TotalSeconds;
        if (duration <= 0)
        {
            return;
        }

        var progress = (_startProgress + _clock.Elapsed.TotalSeconds * Player.PlaybackRate / duration) % 1.0;
        SetSlider(progress);
    }

    private void SetSlider(double value)
    {
        _updatingFromTimer = true;
        ProgressSlider.Value = value;
        _updatingFromTimer = false;
    }
}
