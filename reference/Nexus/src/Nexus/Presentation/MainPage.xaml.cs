using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Animation;

namespace Nexus.Presentation;

public sealed partial class MainPage : Page
{
    private DispatcherTimer? _clockTimer;

    public MainPage()
    {
        this.InitializeComponent();
        this.Loaded += MainPage_Loaded;
    }

    private void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _clockTimer.Tick += (s, args) => ClockText.Text = DateTime.Now.ToString("HH:mm:ss");
        ClockText.Text = DateTime.Now.ToString("HH:mm:ss");
        _clockTimer.Start();

        StartConnectionPulse();
    }

    // Brief 05: the live connection dot pulses opacity 1 -> 0.5 -> 1 over 2s, ease-in-out, forever.
    // Explicit three-keyframe loop because Timeline.AutoReverse is not implemented on Skia desktop.
    private void StartConnectionPulse()
    {
        var ease = new SineEase { EasingMode = EasingMode.EaseInOut };

        var pulse = new DoubleAnimationUsingKeyFrames();
        pulse.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero),
            Value = 1.0
        });
        pulse.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)),
            Value = 0.5,
            EasingFunction = ease
        });
        pulse.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(2)),
            Value = 1.0,
            EasingFunction = ease
        });

        Storyboard.SetTarget(pulse, ConnectionDot);
        Storyboard.SetTargetProperty(pulse, "Opacity");

        var storyboard = new Storyboard
        {
            RepeatBehavior = RepeatBehavior.Forever
        };
        storyboard.Children.Add(pulse);
        storyboard.Begin();
    }
}
