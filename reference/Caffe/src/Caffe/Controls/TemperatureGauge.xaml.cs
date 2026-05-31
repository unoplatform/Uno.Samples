namespace Caffe.Controls;

public sealed partial class TemperatureGauge : UserControl
{
    private const int MinTemperature = 88;
    private const int MaxTemperature = 96;
    private const int DefaultTemperature = 93;
    private const double MinFillHeight = 10;
    private const double MaxFillHeight = 50;

    public static readonly DependencyProperty TemperatureProperty =
        DependencyProperty.Register(
            nameof(Temperature),
            typeof(int),
            typeof(TemperatureGauge),
            new PropertyMetadata(DefaultTemperature, OnTemperatureChanged));

    public int Temperature
    {
        get => (int)GetValue(TemperatureProperty);
        set => SetValue(TemperatureProperty, value);
    }

    public TemperatureGauge()
    {
        this.InitializeComponent();
        // Sync the slider thumb to the control's default; the value text and fill
        // follow via the binding and the property-changed callback.
        TempSlider.Value = DefaultTemperature;
    }

    private static void OnTemperatureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TemperatureGauge gauge)
        {
            var temp = (int)e.NewValue;
            gauge.TempSlider.Value = temp;
            gauge.UpdateFill(temp);
        }
    }

    private void OnSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e) =>
        Temperature = (int)e.NewValue;

    private void UpdateFill(int temp)
    {
        var percentage = (temp - MinTemperature) / (double)(MaxTemperature - MinTemperature);
        TempFill.Height = MinFillHeight + (percentage * (MaxFillHeight - MinFillHeight));
    }

    // x:Bind function binding (see TemperatureGauge.xaml).
    private string FormatTemperature(int temp) => $"{temp}°C";
}
