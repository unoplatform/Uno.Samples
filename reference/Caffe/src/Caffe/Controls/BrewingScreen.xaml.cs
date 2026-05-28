namespace Caffe.Controls;

public sealed partial class BrewingScreen : UserControl
{
    public static readonly DependencyProperty EspressoNameProperty =
        DependencyProperty.Register(nameof(EspressoName), typeof(string), typeof(BrewingScreen),
            new PropertyMetadata("Espresso"));

    public static readonly DependencyProperty ParametersTextProperty =
        DependencyProperty.Register(nameof(ParametersText), typeof(string), typeof(BrewingScreen),
            new PropertyMetadata("93°C · Fine · 27s"));

    public static readonly DependencyProperty BrewProgressProperty =
        DependencyProperty.Register(nameof(BrewProgress), typeof(double), typeof(BrewingScreen),
            new PropertyMetadata(0.0, OnBrewProgressChanged));

    public string EspressoName
    {
        get => (string)GetValue(EspressoNameProperty);
        set => SetValue(EspressoNameProperty, value);
    }

    public string ParametersText
    {
        get => (string)GetValue(ParametersTextProperty);
        set => SetValue(ParametersTextProperty, value);
    }

    public double BrewProgress
    {
        get => (double)GetValue(BrewProgressProperty);
        set => SetValue(BrewProgressProperty, value);
    }

    public BrewingScreen()
    {
        this.InitializeComponent();
    }

    // x:Bind function binding (see BrewingScreen.xaml).
    private string FormatBrewing(string name) => $"Brewing {name}";

    private static void OnBrewProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BrewingScreen screen)
        {
            var progress = (double)e.NewValue;
            // Fill height: 0 to 45 (65% of 70px cup height)
            screen.CoffeeFill.Height = progress * 45;

            // Pulse effect on parameters text
            var opacity = 0.5 + 0.5 * Math.Sin(progress * Math.PI * 4);
            screen.ParamsText.Opacity = opacity;
        }
    }
}
