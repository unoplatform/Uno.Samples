using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media;

namespace FieldOpsPro.Presentation.Controls;

public sealed partial class DashboardHeader : UserControl
{
    private DispatcherTimer? _clockTimer;
    private DispatcherTimer? _pulseTimer;
    private DispatcherTimer? _alertPulseTimer;
    private DispatcherTimer? _alertShowTimer;
    private Storyboard? _popInStoryboard;
    private bool _pulseDirection = true;
    private bool _alertPulseDirection = true;
    private double _currentPulseOpacity = 1.0;
    private double _currentAlertOpacity = 0.3;

    private static Brush B(string key) => Utils.ColorUtils.GetBrush(key);

    public DashboardHeader()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
        this.Unloaded += OnUnloaded;
    }

    #region Dependency Properties

    public static readonly DependencyProperty TitleAreaProperty =
        DependencyProperty.Register(nameof(TitleArea), typeof(object), typeof(DashboardHeader),
            new PropertyMetadata(null));

    public static readonly DependencyProperty AlertTitleProperty =
        DependencyProperty.Register(nameof(AlertTitle), typeof(string), typeof(DashboardHeader),
            new PropertyMetadata("URGENT: Power Outage Reported"));

    public static readonly DependencyProperty AlertMessageProperty =
        DependencyProperty.Register(nameof(AlertMessage), typeof(string), typeof(DashboardHeader),
            new PropertyMetadata("3 work orders affected in Downtown District"));

    public static readonly DependencyProperty ShowAlertProperty =
        DependencyProperty.Register(nameof(ShowAlert), typeof(bool), typeof(DashboardHeader),
            new PropertyMetadata(true, OnShowAlertChanged));

    public static readonly DependencyProperty TemperatureProperty =
        DependencyProperty.Register(nameof(Temperature), typeof(int), typeof(DashboardHeader),
            new PropertyMetadata(72));

    public static readonly DependencyProperty WeatherConditionProperty =
        DependencyProperty.Register(nameof(WeatherCondition), typeof(string), typeof(DashboardHeader),
            new PropertyMetadata("Partly Cloudy"));

    public static readonly DependencyProperty IsConnectedProperty =
        DependencyProperty.Register(nameof(IsConnected), typeof(bool), typeof(DashboardHeader),
            new PropertyMetadata(true, OnIsConnectedChanged));

    public object TitleArea
    {
        get => GetValue(TitleAreaProperty);
        set => SetValue(TitleAreaProperty, value);
    }

    public string AlertTitle
    {
        get => (string)GetValue(AlertTitleProperty);
        set => SetValue(AlertTitleProperty, value);
    }

    public string AlertMessage
    {
        get => (string)GetValue(AlertMessageProperty);
        set => SetValue(AlertMessageProperty, value);
    }

    public bool ShowAlert
    {
        get => (bool)GetValue(ShowAlertProperty);
        set => SetValue(ShowAlertProperty, value);
    }

    public int Temperature
    {
        get => (int)GetValue(TemperatureProperty);
        set => SetValue(TemperatureProperty, value);
    }

    public string WeatherCondition
    {
        get => (string)GetValue(WeatherConditionProperty);
        set => SetValue(WeatherConditionProperty, value);
    }

    public bool IsConnected
    {
        get => (bool)GetValue(IsConnectedProperty);
        set => SetValue(IsConnectedProperty, value);
    }

    #endregion

    #region x:Bind helpers

    /// <summary>Picks the weather glyph based on a condition string.</summary>
    public string WeatherGlyph(string? condition)
    {
        var lower = (condition ?? string.Empty).ToLowerInvariant();

        if (lower.Contains("sun") || lower.Contains("clear")) return "";
        if (lower.Contains("cloud") && lower.Contains("part")) return "";
        if (lower.Contains("cloud") || lower.Contains("overcast")) return "";
        if (lower.Contains("rain") || lower.Contains("shower")) return "";
        if (lower.Contains("storm") || lower.Contains("thunder")) return "";
        if (lower.Contains("snow")) return "";
        if (lower.Contains("fog") || lower.Contains("mist")) return "";
        return ""; // Default partly-cloudy
    }

    /// <summary>Connection text: "LIVE" / "OFFLINE".</summary>
    public string ConnectionLabel(bool connected) => connected ? "LIVE" : "OFFLINE";

    /// <summary>Connection dot fill: accent (connected) vs tertiary grey (offline).</summary>
    public Brush ConnectionDotBrush(bool connected)
        => connected ? B("AccentPrimaryBrush") : B("AccentTertiaryBrush");

    #endregion

    #region Loaded / Unloaded

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        StartClockTimer();
        StartPulseAnimation();          // live-dot pulse runs continuously when connected
        UpdateTimeDisplay();
        ApplyShowAlertState();          // initial alert state (visible→start pop-in, hidden→stop)
        SyncPulseToConnection();        // if not connected, freeze pulse + opacity
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        StopClockTimer();
        StopPulseAnimation();
        StopAlertPulseAnimation();
        StopAlertShowDelay();
    }

    private void OnDismissAlertClick(object sender, RoutedEventArgs e)
    {
        ShowAlert = false;
    }

    // Hover scale on the alert banner was too sensitive — left as no-ops so the XAML's
    // PointerEntered/Exited wiring doesn't error.
    private void OnAlertBannerPointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e) { }
    private void OnAlertBannerPointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e) { }

    #endregion

    #region Clock timer

    private void StartClockTimer()
    {
        _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _clockTimer.Tick += OnClockTick;
        _clockTimer.Start();
    }

    private void StopClockTimer()
    {
        if (_clockTimer != null)
        {
            _clockTimer.Stop();
            _clockTimer.Tick -= OnClockTick;
            _clockTimer = null;
        }
    }

    private void OnClockTick(object? sender, object e) => UpdateTimeDisplay();

    private void UpdateTimeDisplay()
    {
        if (TimeText == null || TimezoneText == null) return;

        var now = DateTime.Now;
        TimeText.Text = now.ToString("HH:mm:ss");

        var timezone = TimeZoneInfo.Local;
        var isDst = timezone.IsDaylightSavingTime(now);
        TimezoneText.Text = GetTimezoneAbbreviation(timezone, isDst);
    }

    private static string GetTimezoneAbbreviation(TimeZoneInfo timezone, bool isDaylightSaving)
    {
        var name = isDaylightSaving ? timezone.DaylightName : timezone.StandardName;

        if (name.Contains("Eastern")) return isDaylightSaving ? "EDT" : "EST";
        if (name.Contains("Central")) return isDaylightSaving ? "CDT" : "CST";
        if (name.Contains("Mountain")) return isDaylightSaving ? "MDT" : "MST";
        if (name.Contains("Pacific")) return isDaylightSaving ? "PDT" : "PST";
        if (name.Contains("UTC") || name.Contains("Coordinated")) return "UTC";
        if (name.Contains("GMT") || name.Contains("Greenwich")) return "GMT";

        var words = name.Split(' ');
        if (words.Length >= 2)
        {
            return string.Concat(words.Take(3).Select(w => w.Length > 0 ? w[0].ToString() : ""));
        }

        return timezone.BaseUtcOffset.Hours >= 0
            ? $"UTC+{timezone.BaseUtcOffset.Hours}"
            : $"UTC{timezone.BaseUtcOffset.Hours}";
    }

    #endregion

    #region Pulse animations (live dot + alert glow)

    private void StartPulseAnimation()
    {
        if (_pulseTimer != null) return;

        _pulseTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
        _pulseTimer.Tick += OnPulseTick;
        _pulseTimer.Start();
    }

    private void StopPulseAnimation()
    {
        if (_pulseTimer != null)
        {
            _pulseTimer.Stop();
            _pulseTimer.Tick -= OnPulseTick;
            _pulseTimer = null;
        }
    }

    private void OnPulseTick(object? sender, object e)
    {
        if (LiveStatusDot == null) return;

        const double step = 0.035;
        const double minOpacity = 0.3;
        const double maxOpacity = 1.0;

        if (_pulseDirection)
        {
            _currentPulseOpacity -= step;
            if (_currentPulseOpacity <= minOpacity)
            {
                _currentPulseOpacity = minOpacity;
                _pulseDirection = false;
            }
        }
        else
        {
            _currentPulseOpacity += step;
            if (_currentPulseOpacity >= maxOpacity)
            {
                _currentPulseOpacity = maxOpacity;
                _pulseDirection = true;
            }
        }

        LiveStatusDot.Opacity = _currentPulseOpacity;
    }

    private void StartAlertPulseAnimation()
    {
        if (_alertPulseTimer != null) return;
        _alertPulseTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
        _alertPulseTimer.Tick += OnAlertPulseTick;
        _alertPulseTimer.Start();
    }

    private void StopAlertPulseAnimation()
    {
        if (_alertPulseTimer != null)
        {
            _alertPulseTimer.Stop();
            _alertPulseTimer.Tick -= OnAlertPulseTick;
            _alertPulseTimer = null;
        }
    }

    private void OnAlertPulseTick(object? sender, object e)
    {
        if (AlertIconGlow == null) return;

        // Gentle breathing pulse (calmed in an earlier turn).
        const double step = 0.012;
        const double minOpacity = 0.3;
        const double maxOpacity = 0.55;

        if (_alertPulseDirection)
        {
            _currentAlertOpacity += step;
            if (_currentAlertOpacity >= maxOpacity)
            {
                _currentAlertOpacity = maxOpacity;
                _alertPulseDirection = false;
            }
        }
        else
        {
            _currentAlertOpacity -= step;
            if (_currentAlertOpacity <= minOpacity)
            {
                _currentAlertOpacity = minOpacity;
                _alertPulseDirection = true;
            }
        }

        AlertIconGlow.Opacity = _currentAlertOpacity;
    }

    private void SyncPulseToConnection()
    {
        if (LiveStatusDot == null) return;
        if (IsConnected)
        {
            StartPulseAnimation();
        }
        else
        {
            StopPulseAnimation();
            LiveStatusDot.Opacity = 1.0;
        }
    }

    #endregion

    #region Alert banner state (visibility + pop-in + pulse)

    private void ApplyShowAlertState()
    {
        if (AlertBanner == null) return;

        if (ShowAlert)
        {
            AlertBanner.Visibility = Visibility.Collapsed;
            AlertBanner.BorderBrush = B("AccentPrimaryBrush");
            AlertBanner.BorderThickness = new Thickness(1.5);
            StartAlertShowDelay();
        }
        else
        {
            StopAlertShowDelay();
            AlertBanner.BorderBrush = B("BgElevated2Brush");
            AlertBanner.BorderThickness = new Thickness(1);
            AlertBanner.Visibility = Visibility.Collapsed;
            StopAlertPulseAnimation();
        }
    }

    private void StartAlertShowDelay()
    {
        if (AlertBanner == null) return;

        if (AlertBanner.Visibility == Visibility.Visible)
        {
            StartAlertPulseAnimation();
            return;
        }

        StopAlertShowDelay();

        AlertBanner.Opacity = 0;
        if (AlertBanner.RenderTransform is ScaleTransform st)
        {
            st.ScaleX = 0.92;
            st.ScaleY = 0.92;
        }

        _alertShowTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(15) };
        _alertShowTimer.Tick += OnAlertShowTimerTick;
        _alertShowTimer.Start();
    }

    private void StopAlertShowDelay()
    {
        if (_alertShowTimer != null)
        {
            _alertShowTimer.Stop();
            _alertShowTimer.Tick -= OnAlertShowTimerTick;
            _alertShowTimer = null;
        }
    }

    private void OnAlertShowTimerTick(object? sender, object? e)
    {
        StopAlertShowDelay();
        if (AlertBanner == null) return;

        AlertBanner.Visibility = Visibility.Visible;

        if (_popInStoryboard == null)
            CreatePopInStoryboard();

        _popInStoryboard?.Begin();
        StartAlertPulseAnimation();
    }

    private void CreatePopInStoryboard()
    {
        if (AlertBanner == null) return;

        var scaleX = new DoubleAnimation { From = 0.92, To = 1.0, Duration = new Duration(TimeSpan.FromMilliseconds(220)), EnableDependentAnimation = true };
        var scaleY = new DoubleAnimation { From = 0.92, To = 1.0, Duration = new Duration(TimeSpan.FromMilliseconds(220)), EnableDependentAnimation = true };
        var fade = new DoubleAnimation { From = 0.0, To = 1.0, Duration = new Duration(TimeSpan.FromMilliseconds(220)), EnableDependentAnimation = true };

        _popInStoryboard = new Storyboard();
        Storyboard.SetTarget(scaleX, AlertBanner);
        Storyboard.SetTargetProperty(scaleX, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");
        Storyboard.SetTarget(scaleY, AlertBanner);
        Storyboard.SetTargetProperty(scaleY, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");
        Storyboard.SetTarget(fade, AlertBanner);
        Storyboard.SetTargetProperty(fade, "Opacity");

        _popInStoryboard.Children.Add(scaleX);
        _popInStoryboard.Children.Add(scaleY);
        _popInStoryboard.Children.Add(fade);
    }

    #endregion

    #region Property-change handlers (only state transitions that need code remain)

    private static void OnShowAlertChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DashboardHeader header) header.ApplyShowAlertState();
    }

    private static void OnIsConnectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DashboardHeader header) header.SyncPulseToConnection();
    }

    #endregion
}
