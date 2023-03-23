namespace TheFancyWeddingHall;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

        this.DataContext = new BindableHallCrowdednessModel(new HallCrowdednessService());
    }
}