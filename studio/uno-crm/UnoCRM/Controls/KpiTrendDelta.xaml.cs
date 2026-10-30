namespace UnoCRM.Controls;

/// <summary>
/// A KPI's period-over-period change: the figure plus an arrow for its direction. Set
/// <see cref="Text"/> to the figure as it should read and <see cref="Trend"/> to the direction it
/// moved; the control pairs each direction with its own arrow and colour.
/// </summary>
public sealed partial class KpiTrendDelta : UserControl
{
    public KpiTrendDelta()
    {
        this.InitializeComponent();
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(KpiTrendDelta), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty TrendProperty =
        DependencyProperty.Register(nameof(Trend), typeof(KpiTrend), typeof(KpiTrendDelta), new PropertyMetadata(KpiTrend.Flat));

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

    public KpiTrend Trend { get => (KpiTrend)GetValue(TrendProperty); set => SetValue(TrendProperty, value); }

    // One row at a time. A function per direction because x:Bind cannot name an enum member in an
    // argument list, and a helper holding the value to compare against would have to be an instance
    // member for x:Bind to reach it. Consumed only by this control's own x:Bind.
    private Visibility UpVisibility(KpiTrend trend) => When(trend, KpiTrend.Up);

    private Visibility DownVisibility(KpiTrend trend) => When(trend, KpiTrend.Down);

    private Visibility FlatVisibility(KpiTrend trend) => When(trend, KpiTrend.Flat);

    private static Visibility When(KpiTrend trend, KpiTrend match) =>
        trend == match ? Visibility.Visible : Visibility.Collapsed;
}
