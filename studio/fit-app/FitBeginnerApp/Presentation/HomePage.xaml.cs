namespace FitBeginnerApp.Presentation;

public sealed partial class HomePage : Page
{
    public HomePage()
    {
        this.InitializeComponent();

        // Hot Design fallback (the preview bypasses Navigation). Set on the *page* DataContext so
        // Navigation can override it with the injected HomeModel at runtime.
        this.DataContext = new HomeModel();
    }
}
