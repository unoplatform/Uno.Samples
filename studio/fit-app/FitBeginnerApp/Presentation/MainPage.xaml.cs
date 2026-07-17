namespace FitBeginnerApp.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

        // Hot Design fallback (the preview bypasses Navigation). Set on the *page* DataContext so
        // Navigation can override it with the injected MainModel at runtime.
        this.DataContext = new MainModel();
    }
}
