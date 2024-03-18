namespace SimpleCalculator;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        DataContext = new BindableMainModel(this.GetThemeService());
    }
}
