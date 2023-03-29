namespace TheFancyWeddingHall;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();

        DataContext = new BindableHallCrowdednessModel(new HallCrowdednessService());
    }
}