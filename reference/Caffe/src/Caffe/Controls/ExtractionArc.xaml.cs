namespace Caffe.Controls;

public sealed partial class ExtractionArc : UserControl
{
    private const int MinTime = 20;
    private const int MaxTime = 35;
    private const int DefaultTime = 27;
    private const double ArcCenterX = 50;
    private const double ArcCenterY = 45;
    private const double ArcRadius = 40;
    private const double ArcStartAngle = -120;
    private const double ArcSweepDegrees = 240;

    public static readonly DependencyProperty ExtractionTimeProperty =
        DependencyProperty.Register(
            nameof(ExtractionTime),
            typeof(int),
            typeof(ExtractionArc),
            new PropertyMetadata(DefaultTime, OnExtractionTimeChanged));

    public int ExtractionTime
    {
        get => (int)GetValue(ExtractionTimeProperty);
        set => SetValue(ExtractionTimeProperty, value);
    }

    public ExtractionArc()
    {
        this.InitializeComponent();
        // Sync the slider thumb to the control's default; the value text follows via
        // the binding and the arc geometry via the property-changed callback.
        TimeSlider.Value = DefaultTime;
    }

    private static void OnExtractionTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ExtractionArc arc)
        {
            var time = (int)e.NewValue;
            arc.TimeSlider.Value = time;
            arc.UpdateArc(time);
        }
    }

    private void OnSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e) =>
        ExtractionTime = (int)e.NewValue;

    private void UpdateArc(int time)
    {
        var percentage = (time - MinTime) / (double)(MaxTime - MinTime);
        var angle = ArcStartAngle + (percentage * ArcSweepDegrees);
        var radians = angle * Math.PI / 180;

        var endX = ArcCenterX + ArcRadius * Math.Sin(radians);
        var endY = ArcCenterY - ArcRadius * Math.Cos(radians);

        ArcSegment.Point = new Windows.Foundation.Point(endX, endY);
        ArcSegment.IsLargeArc = percentage > 0.5;
    }

    // x:Bind function bindings (see ExtractionArc.xaml).
    private string FormatArcValue(int time) => time.ToString();
    private string FormatSeconds(int time) => $"{time}s";
}
