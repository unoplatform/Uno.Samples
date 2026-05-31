using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using FieldOpsPro.Models.Enums;

namespace FieldOpsPro.Presentation.Controls;

public sealed partial class MobileShiftHeader : UserControl
{
    private DispatcherTimer? _timer;
    private DispatcherTimer? _mobileAlertTimer;
    private Storyboard? _mobileAlertPopIn;

    public MobileShiftHeader()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
        this.Unloaded += OnUnloaded;
    }

    // ---- DPs: all eight are bound declaratively in XAML via {x:Bind}, so no
    // DP-changed handlers needed. Default values match the previous defaults so the
    // control still has something to show when used without a binding (e.g. in designers).

    public static readonly DependencyProperty UserNameProperty =
        DependencyProperty.Register(nameof(UserName), typeof(string), typeof(MobileShiftHeader),
            new PropertyMetadata("Jake"));

    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(nameof(Status), typeof(string), typeof(MobileShiftHeader),
            new PropertyMetadata("En Route to Site"));

    public static readonly DependencyProperty DestinationProperty =
        DependencyProperty.Register(nameof(Destination), typeof(string), typeof(MobileShiftHeader),
            new PropertyMetadata("142 Oak Street"));

    public static readonly DependencyProperty EtaProperty =
        DependencyProperty.Register(nameof(Eta), typeof(string), typeof(MobileShiftHeader),
            new PropertyMetadata("ETA 8 min"));

    public static readonly DependencyProperty AvatarInitialsProperty =
        DependencyProperty.Register(nameof(AvatarInitials), typeof(string), typeof(MobileShiftHeader),
            new PropertyMetadata("JD"));

    public static readonly DependencyProperty AvatarColorProperty =
        DependencyProperty.Register(nameof(AvatarColor), typeof(AvatarColor), typeof(MobileShiftHeader),
            new PropertyMetadata(AvatarColor.Blue));

    public static readonly DependencyProperty AvatarStatusProperty =
        DependencyProperty.Register(nameof(AvatarStatus), typeof(AgentStatus?), typeof(MobileShiftHeader),
            new PropertyMetadata(null));

    public static readonly DependencyProperty TasksSummaryProperty =
        DependencyProperty.Register(nameof(TasksSummary), typeof(string), typeof(MobileShiftHeader),
            new PropertyMetadata("3/5"));

    public string UserName
    {
        get => (string)GetValue(UserNameProperty);
        set => SetValue(UserNameProperty, value);
    }

    public string Status
    {
        get => (string)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public string Destination
    {
        get => (string)GetValue(DestinationProperty);
        set => SetValue(DestinationProperty, value);
    }

    public string Eta
    {
        get => (string)GetValue(EtaProperty);
        set => SetValue(EtaProperty, value);
    }

    public string AvatarInitials
    {
        get => (string)GetValue(AvatarInitialsProperty);
        set => SetValue(AvatarInitialsProperty, value);
    }

    public AvatarColor AvatarColor
    {
        get => (AvatarColor)GetValue(AvatarColorProperty);
        set => SetValue(AvatarColorProperty, value);
    }

    public AgentStatus? AvatarStatus
    {
        get => (AgentStatus?)GetValue(AvatarStatusProperty);
        set => SetValue(AvatarStatusProperty, value);
    }

    public string TasksSummary
    {
        get => (string)GetValue(TasksSummaryProperty);
        set => SetValue(TasksSummaryProperty, value);
    }

    // ---- Timer-driven UI (greeting, clock, time remaining, shift %) stays in code-behind
    // because the inputs are wall-clock time (not a DP) and the outputs combine multiple
    // strings + a layout-dependent ProgressFill width.

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateTime();
        UpdateProgress();
        StartTimer();
        StartMobileAlertDelay();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        StopTimer();
        StopMobileAlertDelay();
    }

    private void StartMobileAlertDelay()
    {
        if (MobileAlertBanner == null) return;

        MobileAlertBanner.Visibility = Visibility.Collapsed;
        MobileAlertBanner.Opacity = 0;
        if (MobileAlertBanner.RenderTransform is ScaleTransform st)
        {
            st.ScaleX = 0.92;
            st.ScaleY = 0.92;
        }

        StopMobileAlertDelay();
        _mobileAlertTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(15) };
        _mobileAlertTimer.Tick += OnMobileAlertTimerTick;
        _mobileAlertTimer.Start();
    }

    private void StopMobileAlertDelay()
    {
        if (_mobileAlertTimer != null)
        {
            _mobileAlertTimer.Stop();
            _mobileAlertTimer.Tick -= OnMobileAlertTimerTick;
            _mobileAlertTimer = null;
        }
    }

    private void OnMobileAlertTimerTick(object? sender, object? e)
    {
        StopMobileAlertDelay();
        ShowMobileAlert();
    }

    private void ShowMobileAlert()
    {
        if (MobileAlertBanner == null) return;

        MobileAlertBanner.Visibility = Visibility.Visible;

        if (_mobileAlertPopIn == null)
        {
            var scaleX = new DoubleAnimation { From = 0.92, To = 1.0, Duration = new Duration(TimeSpan.FromMilliseconds(220)), EnableDependentAnimation = true };
            var scaleY = new DoubleAnimation { From = 0.92, To = 1.0, Duration = new Duration(TimeSpan.FromMilliseconds(220)), EnableDependentAnimation = true };
            var fade = new DoubleAnimation { From = 0.0, To = 1.0, Duration = new Duration(TimeSpan.FromMilliseconds(220)), EnableDependentAnimation = true };

            _mobileAlertPopIn = new Storyboard();
            Storyboard.SetTarget(scaleX, MobileAlertBanner);
            Storyboard.SetTargetProperty(scaleX, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");
            Storyboard.SetTarget(scaleY, MobileAlertBanner);
            Storyboard.SetTargetProperty(scaleY, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");
            Storyboard.SetTarget(fade, MobileAlertBanner);
            Storyboard.SetTargetProperty(fade, "Opacity");

            _mobileAlertPopIn.Children.Add(scaleX);
            _mobileAlertPopIn.Children.Add(scaleY);
            _mobileAlertPopIn.Children.Add(fade);
        }

        _mobileAlertPopIn.Begin();
    }

    private void OnMobileDismissAlert(object sender, RoutedEventArgs e)
    {
        if (MobileAlertBanner != null)
            MobileAlertBanner.Visibility = Visibility.Collapsed;
        StopMobileAlertDelay();
    }

    private void StartTimer()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(30)
        };
        _timer.Tick += OnTimerTick;
        _timer.Start();
    }

    private void StopTimer()
    {
        _timer?.Stop();
        _timer = null;
    }

    private void OnTimerTick(object? sender, object e)
    {
        UpdateTime();
        UpdateProgress();
    }

    private void UpdateTime()
    {
        var now = DateTime.Now;
        CurrentTimeText.Text = now.ToString("h:mm tt");

        var endTime = DateTime.Today.AddHours(17);
        var remaining = endTime - now;

        if (remaining.TotalMinutes > 0)
        {
            TimeRemainingText.Text = remaining.TotalHours >= 1
                ? $"{(int)remaining.TotalHours}h {remaining.Minutes}m left"
                : $"{remaining.Minutes}m left";
        }
        else
        {
            TimeRemainingText.Text = "Shift complete";
        }

        GreetingText.Text = now.Hour switch
        {
            < 12 => "Good Morning",
            < 17 => "Good Afternoon",
            _ => "Good Evening"
        };
    }

    private void UpdateProgress()
    {
        // Shift progress (8 AM to 5 PM = 9 hours)
        var now = DateTime.Now;
        var shiftStart = DateTime.Today.AddHours(8);
        var shiftEnd = DateTime.Today.AddHours(17);
        var shiftDuration = (shiftEnd - shiftStart).TotalMinutes;
        var elapsed = (now - shiftStart).TotalMinutes;

        var progress = Math.Max(0, Math.Min(1, elapsed / shiftDuration));
        var percentage = (int)(progress * 100);

        ShiftPercentText.Text = $"{percentage}%";

        // Progress bar width is based on the parent's ActualWidth, which isn't bindable
        // declaratively without a layout hook — keep it imperative.
        if (ProgressFill != null && ProgressFill.Parent is Grid parent)
        {
            var maxWidth = parent.ActualWidth > 0 ? parent.ActualWidth : 280;
            ProgressFill.Width = maxWidth * progress;
        }
    }
}
