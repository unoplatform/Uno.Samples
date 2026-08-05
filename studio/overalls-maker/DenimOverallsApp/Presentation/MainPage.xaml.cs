namespace DenimOverallsApp.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        // DataContext is provided by navigation (the initial "Main" route resolves MainModel from DI).
        // The wide/narrow split is driven declaratively by {utu:Responsive} in XAML — no code-behind sizing.
    }
}
