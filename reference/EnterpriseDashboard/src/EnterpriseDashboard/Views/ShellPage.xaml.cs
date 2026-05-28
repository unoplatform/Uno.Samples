using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using EnterpriseDashboard.Observatory.Views;

namespace EnterpriseDashboard.Views;

public sealed partial class ShellPage : Page
{
    private static readonly string[] Ranges = { "24H", "7D", "30D", "QTD" };
    private string _selectedRange = "30D";

    public ShellPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        BuildRangeControl();
    }

    // Refresh / date-range act on whichever section page the navigation system has
    // made visible inside ContentRegion. The Visibility navigator keeps visited
    // pages parented as siblings and toggles their Visibility, so we walk and find
    // the visible one.
    private void OnRefreshClick(object sender, RoutedEventArgs e)
    {
        PlayPressFeedback((UIElement)sender);
        ActiveSection()?.Refresh();
    }

    private void OnThemeToggleClick(object sender, RoutedEventArgs e)
    {
        if (this.XamlRoot?.Content is FrameworkElement root)
        {
            bool toLight = root.RequestedTheme != ElementTheme.Light;
            root.RequestedTheme = toLight ? ElementTheme.Light : ElementTheme.Dark;
            ThemeLabel.Text = toLight ? "LIGHT" : "DARK";
            ThemeGlyph.Glyph = toLight ? char.ConvertFromUtf32(0xE706) : char.ConvertFromUtf32(0xE708); // sun / moon
        }
    }

    private void OnRangeTapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is not Border { Tag: string range }) return;
        _selectedRange = range;
        StyleRangeButtons();
        ActiveSection()?.Refresh();
    }

    private IObservatorySection? ActiveSection()
    {
        foreach (var child in ContentRegion.Children)
        {
            if (child is FrameworkElement { Visibility: Visibility.Visible } fe
                && fe is IObservatorySection section)
            {
                return section;
            }
        }
        return null;
    }

    // --- Date-range segmented control (quiet blue accent on the selected segment) ---
    private void BuildRangeControl()
    {
        RangeHost.Children.Clear();
        var mono = (FontFamily)Application.Current.Resources["ObsMonoFamily"];
        foreach (var r in Ranges)
        {
            var seg = new Border
            {
                Tag = r,
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(14, 5, 14, 5),
                Child = new TextBlock { Text = r, FontFamily = mono, FontSize = 11, VerticalAlignment = VerticalAlignment.Center }
            };
            seg.Tapped += OnRangeTapped;
            RangeHost.Children.Add(seg);
        }
        StyleRangeButtons();
    }

    private void StyleRangeButtons()
    {
        var accent = (Brush)Application.Current.Resources["ObsAccentBrush"];
        var onAccent = (Brush)Application.Current.Resources["ObsOnAccentBrush"];
        var mid = (Brush)Application.Current.Resources["ObsMidBrush"];
        var transparent = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
        foreach (var child in RangeHost.Children)
        {
            if (child is not Border { Tag: string r, Child: TextBlock label } seg) continue;
            bool selected = r == _selectedRange;
            // Active segment is a solid accent pill with high-contrast ink — the strongest accent on screen.
            seg.Background = selected ? accent : transparent;
            label.Foreground = selected ? onAccent : mid;
        }
    }

    private static void PlayPressFeedback(UIElement el)
    {
        if (el.RenderTransform is not ScaleTransform st)
        {
            st = new ScaleTransform { ScaleX = 1, ScaleY = 1 };
            el.RenderTransform = st;
            el.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
        }

        var sb = new Storyboard();
        foreach (var axis in new[] { "ScaleX", "ScaleY" })
        {
            var anim = new DoubleAnimationUsingKeyFrames();
            anim.KeyFrames.Add(new DiscreteDoubleKeyFrame { KeyTime = TimeSpan.Zero, Value = 1 });
            anim.KeyFrames.Add(new SplineDoubleKeyFrame
            {
                KeyTime = TimeSpan.FromMilliseconds(80), Value = 0.97,
                KeySpline = new KeySpline { ControlPoint1 = new Windows.Foundation.Point(0.4, 0), ControlPoint2 = new Windows.Foundation.Point(0.2, 1) }
            });
            anim.KeyFrames.Add(new SplineDoubleKeyFrame
            {
                KeyTime = TimeSpan.FromMilliseconds(220), Value = 1.0,
                KeySpline = new KeySpline { ControlPoint1 = new Windows.Foundation.Point(0.22, 0.61), ControlPoint2 = new Windows.Foundation.Point(0.36, 1.0) }
            });
            Storyboard.SetTarget(anim, el);
            Storyboard.SetTargetProperty(anim, $"(UIElement.RenderTransform).(ScaleTransform.{axis})");
            sb.Children.Add(anim);
        }
        sb.Begin();
    }
}
