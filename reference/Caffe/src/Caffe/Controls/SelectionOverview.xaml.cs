namespace Caffe.Controls;

public sealed partial class SelectionOverview : UserControl
{
    public static readonly DependencyProperty EspressoNameProperty =
        DependencyProperty.Register(nameof(EspressoName), typeof(string), typeof(SelectionOverview),
            new PropertyMetadata("Espresso"));

    public static readonly DependencyProperty TemperatureProperty =
        DependencyProperty.Register(nameof(Temperature), typeof(int), typeof(SelectionOverview),
            new PropertyMetadata(93));

    public static readonly DependencyProperty GrindAbbreviationProperty =
        DependencyProperty.Register(nameof(GrindAbbreviation), typeof(string), typeof(SelectionOverview),
            new PropertyMetadata("F"));

    public static readonly DependencyProperty ExtractionTimeProperty =
        DependencyProperty.Register(nameof(ExtractionTime), typeof(int), typeof(SelectionOverview),
            new PropertyMetadata(27));

    public string EspressoName
    {
        get => (string)GetValue(EspressoNameProperty);
        set => SetValue(EspressoNameProperty, value);
    }

    public int Temperature
    {
        get => (int)GetValue(TemperatureProperty);
        set => SetValue(TemperatureProperty, value);
    }

    public string GrindAbbreviation
    {
        get => (string)GetValue(GrindAbbreviationProperty);
        set => SetValue(GrindAbbreviationProperty, value);
    }

    public int ExtractionTime
    {
        get => (int)GetValue(ExtractionTimeProperty);
        set => SetValue(ExtractionTimeProperty, value);
    }

    public SelectionOverview()
    {
        this.InitializeComponent();
    }

    // x:Bind function bindings (see SelectionOverview.xaml).
    private string FormatDegrees(int value) => $"{value}°";
    private string FormatSeconds(int value) => $"{value}s";
}
