namespace UnoGrpc;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; } = new MainViewModel(new Services.WeatherService());

    public MainPage()
    {
        this.InitializeComponent();
    }
}
