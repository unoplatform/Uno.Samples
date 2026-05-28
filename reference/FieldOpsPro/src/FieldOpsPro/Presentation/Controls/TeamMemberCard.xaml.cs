using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using FieldOpsPro.Models;

namespace FieldOpsPro.Presentation.Controls;

public sealed partial class TeamMemberCard : UserControl
{
    private static Brush B(string key) => Utils.ColorUtils.GetBrush(key);

    public TeamMemberCard()
    {
        this.InitializeComponent();
    }

    public static readonly DependencyProperty MemberProperty =
        DependencyProperty.Register(nameof(Member), typeof(TeamMember), typeof(TeamMemberCard),
            new PropertyMetadata(null));

    public TeamMember? Member
    {
        get => (TeamMember?)GetValue(MemberProperty);
        set => SetValue(MemberProperty, value);
    }

    // ---- x:Bind helper methods (invoked from XAML) ----

    /// <summary>Battery fill width in px: 16px = full, scales linearly with level.</summary>
    public double BatteryFillWidth(int level)
        => System.Math.Max(0, System.Math.Min(16, level * 16.0 / 100.0));

    /// <summary>Battery fill brush: white when critical (≤20) or normal (&gt;40), grey when low (21..40).</summary>
    public Brush BatteryBrush(int level)
        => level switch
        {
            <= 20 => B("AccentPrimaryBrush"),    // Critical
            <= 40 => B("AccentSecondaryBrush"),  // Low
            _ => B("AccentPrimaryBrush"),        // Normal
        };

    /// <summary>Low-battery warning glyph is visible only at ≤20% level.</summary>
    public Visibility LowBatteryVisibility(int level)
        => level <= 20 ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>Signal bar brush: white when active (bar ≤ strength), dark grey otherwise.</summary>
    public Brush SignalBrush(int strength, int bar)
        => bar <= strength ? B("AccentPrimaryBrush") : B("BorderStrongBrush");

    // ---- Hover animation ----

    private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
    {
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

        CardBorder.BorderBrush = B("BorderStrongBrush");
    }

    private void OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
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

        CardBorder.BorderBrush = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
    }
}
