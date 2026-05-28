using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

namespace FieldOpsPro.Presentation.Controls;

public sealed partial class QuickActionButton : UserControl
{
    // Brushes resolved from FieldOps design tokens.
    private static Brush B(string key) => Utils.ColorUtils.GetBrush(key);
    private static Windows.UI.Color C(string key) => Utils.ColorUtils.GetColor(key);

    public QuickActionButton()
    {
        this.InitializeComponent();
    }

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(QuickActionButton),
            new PropertyMetadata(""));

    public static readonly DependencyProperty IconGlyphProperty =
        DependencyProperty.Register(nameof(IconGlyph), typeof(string), typeof(QuickActionButton),
            new PropertyMetadata(""));

    public static readonly DependencyProperty AccentColorProperty =
        DependencyProperty.Register(nameof(AccentColor), typeof(QuickActionAccent), typeof(QuickActionButton),
            new PropertyMetadata(QuickActionAccent.Primary));

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string IconGlyph
    {
        get => (string)GetValue(IconGlyphProperty);
        set => SetValue(IconGlyphProperty, value);
    }

    public QuickActionAccent AccentColor
    {
        get => (QuickActionAccent)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    public event EventHandler? ActionClicked;

    private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        AnimateScale(1.03);
        ActionBorder.Background = B("QuickActionHoverGradientBrush");
    }

    private void OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        AnimateScale(1.0);
        ActionBorder.Background = B("QuickActionGradientBrush");
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        AnimateScale(0.96);
        ActionBorder.Background = B("QuickActionPressedGradientBrush");
    }

    private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        AnimateScale(1.03);
        ActionBorder.Background = B("QuickActionHoverGradientBrush");
        ActionClicked?.Invoke(this, EventArgs.Empty);
    }

    private void AnimateScale(double targetScale)
    {
        var scaleXAnimation = new DoubleAnimation
        {
            To = targetScale,
            Duration = new Duration(TimeSpan.FromMilliseconds(150)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(scaleXAnimation, BorderTransform);
        Storyboard.SetTargetProperty(scaleXAnimation, "ScaleX");

        var scaleYAnimation = new DoubleAnimation
        {
            To = targetScale,
            Duration = new Duration(TimeSpan.FromMilliseconds(150)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(scaleYAnimation, BorderTransform);
        Storyboard.SetTargetProperty(scaleYAnimation, "ScaleY");

        var storyboard = new Storyboard();
        storyboard.Children.Add(scaleXAnimation);
        storyboard.Children.Add(scaleYAnimation);
        storyboard.Begin();
    }
}

public enum QuickActionAccent
{
    Primary,
    Secondary,
    Tertiary,
    Success
}
