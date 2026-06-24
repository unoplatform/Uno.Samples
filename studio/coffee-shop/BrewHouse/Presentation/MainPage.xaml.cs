namespace BrewHouse.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

        // The shell is view-only (no navigation-injected model), so bind its chrome (the Cart
        // count badge) to the shared cart/orders singleton. AppState.Current is the same instance
        // the DI-registered AppState resolves to, so the badge stays in sync with every page.
        this.DataContext = AppState.Current;
    }
}
