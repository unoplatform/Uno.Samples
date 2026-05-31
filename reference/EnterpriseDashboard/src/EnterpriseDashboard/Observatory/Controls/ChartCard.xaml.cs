using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;

namespace EnterpriseDashboard.Observatory.Controls;

[ContentProperty(Name = nameof(ChartContent))]
public sealed partial class ChartCard : UserControl
{
    public ChartCard()
    {
        InitializeComponent();
        ActualThemeChanged += (_, _) => ApplyDelta();
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(ChartCard),
        new PropertyMetadata(string.Empty, (d, e) => ((ChartCard)d).TitleText.Text = (string)e.NewValue));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty BadgeProperty = DependencyProperty.Register(
        nameof(Badge), typeof(string), typeof(ChartCard),
        new PropertyMetadata(string.Empty, (d, e) => ((ChartCard)d).BadgeText.Text = (string)e.NewValue));

    public string Badge
    {
        get => (string)GetValue(BadgeProperty);
        set => SetValue(BadgeProperty, value);
    }

    public static readonly DependencyProperty StatValueProperty = DependencyProperty.Register(
        nameof(StatValue), typeof(string), typeof(ChartCard),
        new PropertyMetadata(string.Empty, (d, e) => ((ChartCard)d).StatText.Text = (string)e.NewValue));

    public string StatValue
    {
        get => (string)GetValue(StatValueProperty);
        set => SetValue(StatValueProperty, value);
    }

    public static readonly DependencyProperty StatUnitProperty = DependencyProperty.Register(
        nameof(StatUnit), typeof(string), typeof(ChartCard),
        new PropertyMetadata(string.Empty, (d, e) => ((ChartCard)d).UnitText.Text = (string)e.NewValue));

    public string StatUnit
    {
        get => (string)GetValue(StatUnitProperty);
        set => SetValue(StatUnitProperty, value);
    }

    public static readonly DependencyProperty ChartContentProperty = DependencyProperty.Register(
        nameof(ChartContent), typeof(object), typeof(ChartCard),
        new PropertyMetadata(null, (d, e) => ((ChartCard)d).ChartHost.Content = e.NewValue));

    public object? ChartContent
    {
        get => GetValue(ChartContentProperty);
        set => SetValue(ChartContentProperty, value);
    }

    public static readonly DependencyProperty LegendProperty = DependencyProperty.Register(
        nameof(Legend), typeof(object), typeof(ChartCard),
        new PropertyMetadata(null, (d, e) => ((ChartCard)d).LegendHost.Content = e.NewValue));

    public object? Legend
    {
        get => GetValue(LegendProperty);
        set => SetValue(LegendProperty, value);
    }

    public static readonly DependencyProperty DeltaTextProperty = DependencyProperty.Register(
        nameof(DeltaText), typeof(string), typeof(ChartCard),
        new PropertyMetadata(string.Empty, (d, _) => ((ChartCard)d).ApplyDelta()));

    public string DeltaText
    {
        get => (string)GetValue(DeltaTextProperty);
        set => SetValue(DeltaTextProperty, value);
    }

    public static readonly DependencyProperty DeltaPositiveProperty = DependencyProperty.Register(
        nameof(DeltaPositive), typeof(bool), typeof(ChartCard),
        new PropertyMetadata(true, (d, _) => ((ChartCard)d).ApplyDelta()));

    public bool DeltaPositive
    {
        get => (bool)GetValue(DeltaPositiveProperty);
        set => SetValue(DeltaPositiveProperty, value);
    }

    public static readonly DependencyProperty FooterTextProperty = DependencyProperty.Register(
        nameof(FooterText), typeof(string), typeof(ChartCard),
        new PropertyMetadata(string.Empty, (d, e) =>
        {
            var c = (ChartCard)d;
            var s = (string)e.NewValue;
            c.FooterTextBlock.Text = s;
            c.FooterTextBlock.Visibility = string.IsNullOrEmpty(s) ? Visibility.Collapsed : Visibility.Visible;
        }));

    public string FooterText
    {
        get => (string)GetValue(FooterTextProperty);
        set => SetValue(FooterTextProperty, value);
    }

    // Positive delta reads green with a ▲; negative reads red with a ▼ — semantic state.
    private void ApplyDelta()
    {
        DeltaTextBlock.Text = DeltaText;
        DeltaPanel.Visibility = string.IsNullOrEmpty(DeltaText) ? Visibility.Collapsed : Visibility.Visible;
        DeltaGlyph.Text = DeltaPositive ? "▲" : "▼";
        var key = DeltaPositive ? "ObsPositiveBrush" : "ObsNegativeBrush";
        if (Application.Current.Resources[key] is Brush brush)
        {
            DeltaGlyph.Foreground = brush;
            DeltaTextBlock.Foreground = brush;
        }
    }
}
