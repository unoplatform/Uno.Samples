namespace Caffe.Controls;

public sealed partial class BrewButton : UserControl
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(BrewButton),
            new PropertyMetadata("Select your espresso"));

    public static readonly DependencyProperty IsBrewEnabledProperty =
        DependencyProperty.Register(nameof(IsBrewEnabled), typeof(bool), typeof(BrewButton),
            new PropertyMetadata(false));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool IsBrewEnabled
    {
        get => (bool)GetValue(IsBrewEnabledProperty);
        set => SetValue(IsBrewEnabledProperty, value);
    }

    public event EventHandler? BrewRequested;

    public BrewButton()
    {
        this.InitializeComponent();
    }

    private void OnClick(object sender, RoutedEventArgs e)
    {
        if (IsBrewEnabled)
            BrewRequested?.Invoke(this, EventArgs.Empty);
    }

    // x:Bind function binding (see BrewButton.xaml).
    private Brush BackgroundBrush(bool isEnabled) =>
        (Brush)Application.Current.Resources[isEnabled ? "CaffePrimaryBrush" : "CaffePrimaryDisabledBrush"];
}
