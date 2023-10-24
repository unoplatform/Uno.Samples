namespace WeddingHallApp;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();

        DataContext = new BindableWeddingHallModel(new WeddingHallService());
    }
}