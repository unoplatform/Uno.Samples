using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace FieldOpsPro.Presentation.Controls;

public sealed partial class StatCard : UserControl
{
    private static readonly Random _random = new();

    // Brushes resolved from FieldOps design tokens (Styles/FieldOpsResources.xaml).
    private static Brush B(string key) => Utils.ColorUtils.GetBrush(key);
    private static Windows.UI.Color C(string key) => Utils.ColorUtils.GetColor(key);

    public StatCard()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Label / Value / IconGlyph / icon container bg / icon foreground are bound declaratively
        // in XAML now. Only the trend-change panel and the LiveCharts sparkline still need
        // imperative setup, because their inputs combine multiple DPs with conditional logic.
        UpdateChangePanel();
        SetupChart();
    }

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(StatCard),
            new PropertyMetadata("", OnPropertyChanged));

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(string), typeof(StatCard),
            new PropertyMetadata("", OnPropertyChanged));

    public static readonly DependencyProperty ChangePercentProperty =
        DependencyProperty.Register(nameof(ChangePercent), typeof(double?), typeof(StatCard),
            new PropertyMetadata(null, OnPropertyChanged));

    public static readonly DependencyProperty IconGlyphProperty =
        DependencyProperty.Register(nameof(IconGlyph), typeof(string), typeof(StatCard),
            new PropertyMetadata(""));

    public static readonly DependencyProperty AccentColorProperty =
        DependencyProperty.Register(nameof(AccentColor), typeof(StatAccentColor), typeof(StatCard),
            new PropertyMetadata(StatAccentColor.Primary, OnPropertyChanged));

    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(StatCard),
            new PropertyMetadata(null, OnPropertyChanged));

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public double? ChangePercent
    {
        get => (double?)GetValue(ChangePercentProperty);
        set => SetValue(ChangePercentProperty, value);
    }

    public string IconGlyph
    {
        get => (string)GetValue(IconGlyphProperty);
        set => SetValue(IconGlyphProperty, value);
    }

    public StatAccentColor AccentColor
    {
        get => (StatAccentColor)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    public string? Subtitle
    {
        get => (string?)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is StatCard card)
        {
            card.UpdateChangePanel();
            card.SetupChart();
        }
    }

    private void UpdateChangePanel()
    {
        if (ChangePanel == null) return;

        // Change indicator
        if (ChangePercent.HasValue && ChangePercent.Value != 0)
        {
            ChangePanel.Visibility = Visibility.Visible;
            TrendIcon.Visibility = Visibility.Visible;
            var isPositive = ChangePercent.Value > 0;

            // For response time (Tertiary), lower is better
            var isGood = AccentColor == StatAccentColor.Tertiary
                ? !isPositive
                : isPositive;

            var changeColor = isGood ? C("Success") : C("TextFaint");
            var changeBrush = new SolidColorBrush(changeColor);

            TrendIcon.Glyph = isPositive ? "" : "";
            TrendIcon.Foreground = changeBrush;

            ChangeText.Text = $"{(isPositive ? "+" : "")}{ChangePercent.Value:F1}%";
            ChangeText.Foreground = changeBrush;
        }
        else if (!string.IsNullOrEmpty(Subtitle))
        {
            ChangePanel.Visibility = Visibility.Visible;
            TrendIcon.Visibility = Visibility.Collapsed;
            ChangeText.Text = Subtitle;
            ChangeText.Foreground = B("TextMutedBrush");
        }
        else
        {
            ChangePanel.Visibility = Visibility.Collapsed;
        }
    }

    private void SetupChart()
    {
        if (MiniChart == null) return;

        var accentColor = GetAccentColor(AccentColor);
        var skColor = new SKColor(accentColor.R, accentColor.G, accentColor.B);
        var skColorFaded = skColor.WithAlpha(60);

        // Generate trend data
        var data = GenerateTrendData();

        var lineSeries = new LineSeries<double>
        {
            Values = data,
            Fill = new LinearGradientPaint(
                new[] { skColorFaded, SKColors.Transparent },
                new SKPoint(0.5f, 0),
                new SKPoint(0.5f, 1)),
            Stroke = new SolidColorPaint(skColor, 2),
            GeometryFill = null,
            GeometryStroke = null,
            LineSmoothness = 0.65
        };

        MiniChart.Series = new ISeries[] { lineSeries };

        // Hide axes for minimal look
        MiniChart.XAxes = new Axis[]
        {
            new Axis
            {
                IsVisible = false,
                ShowSeparatorLines = false
            }
        };

        MiniChart.YAxes = new Axis[]
        {
            new Axis
            {
                IsVisible = false,
                ShowSeparatorLines = false
            }
        };

        // Remove legend and other UI elements
        MiniChart.DrawMarginFrame = null;
    }

    private double[] GenerateTrendData()
    {
        // Generate realistic trend data based on the stat type
        var baseValue = AccentColor switch
        {
            StatAccentColor.Primary => 45.0,    // Work orders
            StatAccentColor.Secondary => 12.0,  // Agents
            StatAccentColor.Tertiary => 25.0,   // Response time
            StatAccentColor.Success => 92.0,    // Completion rate
            _ => 50.0
        };

        var trend = ChangePercent.HasValue && ChangePercent.Value > 0 ? 0.02 : -0.01;
        var data = new double[12];

        for (int i = 0; i < 12; i++)
        {
            var variation = (_random.NextDouble() - 0.5) * 0.1;
            var trendFactor = 1 + (trend * i) + variation;
            data[i] = baseValue * trendFactor;
        }

        return data;
    }

    /// <summary>Resolves the accent enum to a raw Color (still needed for SkiaSharp chart paints).</summary>
    private static Windows.UI.Color GetAccentColor(StatAccentColor color)
    {
        return color switch
        {
            StatAccentColor.Primary => C("AccentPrimary"),
            StatAccentColor.Secondary => C("AccentSecondary"),
            StatAccentColor.Tertiary => C("AccentMedium"),
            StatAccentColor.Success => C("Success"),
            StatAccentColor.Warning => C("Warning"),
            StatAccentColor.Danger => C("AccentTertiary"),
            _ => C("AccentPrimary")
        };
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
        CardBorder.BorderBrush = B("BorderHoverBrush");
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

        // Reset border
        CardBorder.BorderBrush = B("BorderColorBrush");
    }
}

public enum StatAccentColor
{
    Primary,
    Secondary,
    Tertiary,
    Success,
    Warning,
    Danger
}
