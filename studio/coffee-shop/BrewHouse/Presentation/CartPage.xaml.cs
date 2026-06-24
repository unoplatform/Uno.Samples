namespace BrewHouse.Presentation;

public sealed partial class CartPage : Page
{
    public CartPage()
    {
        this.InitializeComponent();

        // Hot Design fallback (unconditional); Navigation injects the DI-resolved CartPageData at
        // runtime and overrides this. Set on the page, not a child element.
        this.DataContext = new CartPageData(AppState.Current);
    }
}
