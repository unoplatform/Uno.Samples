namespace BrewHouse.Presentation;

public sealed partial class OrdersPage : Page
{
    public OrdersPage()
    {
        this.InitializeComponent();

        // Hot Design fallback (unconditional); Navigation injects the DI-resolved OrdersPageData at
        // runtime and overrides this. Set on the page, not a child element.
        this.DataContext = new OrdersPageData(AppState.Current);
    }
}
