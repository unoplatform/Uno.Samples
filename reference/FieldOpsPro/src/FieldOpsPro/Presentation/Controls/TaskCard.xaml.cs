using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using FieldOpsPro.Models;

namespace FieldOpsPro.Presentation.Controls;

public sealed partial class TaskCard : UserControl
{
    private DispatcherTimer? _slaFlashTimer;
    private bool _slaFlashState;

    // Brushes resolved from the FieldOps design tokens (Styles/FieldOpsResources.xaml)
    // so the card stays in sync with the active theme.
    private static Brush B(string key) => Utils.ColorUtils.GetBrush(key);

    public TaskCard()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
        this.Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateSlaTimer();
        UpdatePhotoThumbnails();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        StopSlaTimer();
    }

    public static readonly DependencyProperty TaskItemProperty =
        DependencyProperty.Register(nameof(TaskItem), typeof(TaskItem), typeof(TaskCard),
            new PropertyMetadata(null, OnTaskItemChanged));

    public TaskItem? TaskItem
    {
        get => (TaskItem?)GetValue(TaskItemProperty);
        set => SetValue(TaskItemProperty, value);
    }

    private static void OnTaskItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TaskCard card)
        {
            // Display bits (title, location, duration, priority indicator, assignee avatar,
            // status badge) are now bound declaratively in XAML via {x:Bind} + converters.
            // The two pieces with logic — the SLA timer state machine and photo URL loading —
            // still run from code-behind in response to TaskItem changes.
            card.UpdateSlaTimer();
            card.UpdatePhotoThumbnails();
        }
    }

    /// <summary>Friendly location label: prefers Name, falls back to Address, then "Unknown". Invoked from x:Bind.</summary>
    public string LocationLabel(TaskItem? item)
        => item?.Location?.Name ?? item?.Location?.Address ?? "Unknown";

    private void UpdateSlaTimer()
    {
        if (SlaTimerBorder == null || TaskItem?.SlaDeadline == null)
        {
            if (SlaTimerBorder != null)
                SlaTimerBorder.Visibility = Visibility.Collapsed;
            StopSlaTimer();
            return;
        }

        var deadline = TaskItem.SlaDeadline.Value;
        var remaining = deadline - DateTime.Now;

        if (remaining.TotalMinutes <= 0)
        {
            // SLA breached
            SlaTimerBorder.Visibility = Visibility.Visible;
            SlaTimerBorder.Background = B("BorderStrongBrush");
            SlaTimerText.Text = "OVERDUE";
            SlaTimerText.Foreground = B("AccentPrimaryBrush");
            SlaIcon.Foreground = B("AccentPrimaryBrush");
            StartSlaFlashTimer(true);
        }
        else if (remaining.TotalMinutes <= 30)
        {
            // Critical - under 30 minutes
            SlaTimerBorder.Visibility = Visibility.Visible;
            SlaTimerBorder.Background = B("SlaCriticalBgBrush");
            SlaTimerText.Text = FormatTimeRemaining(remaining);
            SlaTimerText.Foreground = B("AccentPrimaryBrush");
            SlaIcon.Foreground = B("AccentPrimaryBrush");
            StartSlaFlashTimer(true);
        }
        else if (remaining.TotalHours <= 2)
        {
            // Warning - under 2 hours
            SlaTimerBorder.Visibility = Visibility.Visible;
            SlaTimerBorder.Background = B("SlaWarningBgBrush");
            SlaTimerText.Text = FormatTimeRemaining(remaining);
            SlaTimerText.Foreground = B("AccentSecondaryBrush");
            SlaIcon.Foreground = B("AccentSecondaryBrush");
            StopSlaTimer();
        }
        else
        {
            // Normal - show timer but no urgency
            SlaTimerBorder.Visibility = Visibility.Visible;
            SlaTimerBorder.Background = B("BgElevatedBrush");
            SlaTimerText.Text = FormatTimeRemaining(remaining);
            SlaTimerText.Foreground = B("AccentTertiaryBrush");
            SlaIcon.Foreground = B("AccentTertiaryBrush");
            StopSlaTimer();
        }
    }

    private static string FormatTimeRemaining(TimeSpan remaining)
    {
        if (remaining.TotalHours >= 24)
        {
            return $"{(int)remaining.TotalDays}d {remaining.Hours}h";
        }
        else if (remaining.TotalHours >= 1)
        {
            return $"{(int)remaining.TotalHours}h {remaining.Minutes}m";
        }
        else
        {
            return $"{remaining.Minutes}m";
        }
    }

    private void StartSlaFlashTimer(bool urgent)
    {
        if (!urgent)
        {
            StopSlaTimer();
            return;
        }

        if (_slaFlashTimer == null)
        {
            _slaFlashTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _slaFlashTimer.Tick += OnSlaFlashTick;
        }
        _slaFlashTimer.Start();
    }

    private void StopSlaTimer()
    {
        _slaFlashTimer?.Stop();
    }

    private void OnSlaFlashTick(object? sender, object e)
    {
        if (SlaTimerBorder == null) return;

        _slaFlashState = !_slaFlashState;

        // Flash between white and grey backgrounds
        if (_slaFlashState)
        {
            SlaTimerBorder.Background = B("AccentPrimaryBrush");
            SlaTimerText.Foreground = B("TextOnAccentBrush");
            SlaIcon.Foreground = B("TextOnAccentBrush");
        }
        else
        {
            SlaTimerBorder.Background = B("BorderStrongBrush");
            SlaTimerText.Foreground = B("AccentPrimaryBrush");
            SlaIcon.Foreground = B("AccentPrimaryBrush");
        }
    }

    private void UpdatePhotoThumbnails()
    {
        if (PhotosPanel == null) return;

        var photos = TaskItem?.PhotoUrls;
        if (photos == null || photos.Length == 0)
        {
            PhotosPanel.Visibility = Visibility.Collapsed;
            return;
        }

        PhotosPanel.Visibility = Visibility.Visible;

        // Show up to 3 photos
        Photo1.Visibility = photos.Length >= 1 ? Visibility.Visible : Visibility.Collapsed;
        Photo2.Visibility = photos.Length >= 2 ? Visibility.Visible : Visibility.Collapsed;
        Photo3.Visibility = photos.Length >= 3 ? Visibility.Visible : Visibility.Collapsed;

        try
        {
            if (photos.Length >= 1 && PhotoImage1 != null && Uri.TryCreate(photos[0], UriKind.Absolute, out var uri1))
                PhotoImage1.Source = new BitmapImage(uri1);
            if (photos.Length >= 2 && PhotoImage2 != null && Uri.TryCreate(photos[1], UriKind.Absolute, out var uri2))
                PhotoImage2.Source = new BitmapImage(uri2);
            if (photos.Length >= 3 && PhotoImage3 != null && Uri.TryCreate(photos[2], UriKind.Absolute, out var uri3))
                PhotoImage3.Source = new BitmapImage(uri3);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading photo thumbnails: {ex.Message}");
        }

        // Show "+N" indicator if more than 3 photos
        if (photos.Length > 3)
        {
            MorePhotosIndicator.Visibility = Visibility.Visible;
            MorePhotosText.Text = $"+{photos.Length - 3}";
        }
        else
        {
            MorePhotosIndicator.Visibility = Visibility.Collapsed;
        }
    }

    private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        // Animate lift effect
        var liftAnimation = new DoubleAnimation
        {
            To = -4,
            Duration = new Duration(TimeSpan.FromMilliseconds(200)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(liftAnimation, CardTransform);
        Storyboard.SetTargetProperty(liftAnimation, "TranslateY");

        var storyboard = new Storyboard();
        storyboard.Children.Add(liftAnimation);
        storyboard.Begin();

        // Glow border effect
        CardBorder.BorderBrush = B("BorderStrongBrush");

        // Additional hover: scale up and brighten background/icons (match QuickAction feel)
        AnimateScale(1.03);

        // Brighter gradient background on hover
        CardBorder.Background = B("CardHoverGradientBrush");

        // Brighten the small icons if available
        if (LocationIcon != null) LocationIcon.Foreground = B("AccentPrimaryBrush");
        if (DurationIcon != null) DurationIcon.Foreground = B("AccentPrimaryBrush");
    }

    private void OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        // Animate back down
        var dropAnimation = new DoubleAnimation
        {
            To = 0,
            Duration = new Duration(TimeSpan.FromMilliseconds(200)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(dropAnimation, CardTransform);
        Storyboard.SetTargetProperty(dropAnimation, "TranslateY");

        var storyboard = new Storyboard();
        storyboard.Children.Add(dropAnimation);
        storyboard.Begin();

        // Remove glow
        CardBorder.BorderBrush = new SolidColorBrush(Microsoft.UI.Colors.Transparent);

        // Scale back to normal
        AnimateScale(1.0);

        // Restore background to original resource if available
        if (Application.Current != null && Application.Current.Resources.ContainsKey("BgTertiaryBrush"))
        {
            var brush = Application.Current.Resources["BgTertiaryBrush"] as Brush;
            if (brush != null)
                CardBorder.Background = brush;
        }

        // Restore icons
        if (LocationIcon != null && Application.Current.Resources.TryGetValue("TextMutedBrush", out var locBrush) && locBrush is Brush locBrushTyped)
            LocationIcon.Foreground = locBrushTyped;
        if (DurationIcon != null && Application.Current.Resources.TryGetValue("TextMutedBrush", out var durBrush) && durBrush is Brush durBrushTyped)
            DurationIcon.Foreground = durBrushTyped;
    }

    private void AnimateScale(double targetScale)
    {
        var scaleXAnimation = new DoubleAnimation
        {
            To = targetScale,
            Duration = new Duration(TimeSpan.FromMilliseconds(150)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(scaleXAnimation, CardTransform);
        Storyboard.SetTargetProperty(scaleXAnimation, "ScaleX");

        var scaleYAnimation = new DoubleAnimation
        {
            To = targetScale,
            Duration = new Duration(TimeSpan.FromMilliseconds(150)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(scaleYAnimation, CardTransform);
        Storyboard.SetTargetProperty(scaleYAnimation, "ScaleY");

        var sb = new Storyboard();
        sb.Children.Add(scaleXAnimation);
        sb.Children.Add(scaleYAnimation);
        sb.Begin();
    }
}
