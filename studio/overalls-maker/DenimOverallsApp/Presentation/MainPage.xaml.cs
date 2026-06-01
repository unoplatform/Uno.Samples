namespace DenimOverallsApp.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        // DataContext is provided by navigation (the initial "Main" route resolves MainModel from DI).
    }
}
