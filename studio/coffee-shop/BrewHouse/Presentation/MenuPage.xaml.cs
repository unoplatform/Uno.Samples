namespace BrewHouse.Presentation;

public sealed partial class MenuPage : Page
{
    public MenuPage()
    {
        this.InitializeComponent();

        // Hot Design fallback (unconditional); Navigation injects the DI-resolved MenuPageData at
        // runtime and overrides this. Set on the page, not a child element.
        this.DataContext = new MenuPageData(AppState.Current);
    }
}
