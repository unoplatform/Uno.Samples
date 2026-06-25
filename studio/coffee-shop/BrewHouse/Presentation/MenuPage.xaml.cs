using BrewHouse.Presentation.MockData;

namespace BrewHouse.Presentation;

public sealed partial class MenuPage : Page
{
    public MenuPage()
    {
        this.InitializeComponent();

        // Hot Design fallback; Navigation injects the generated MenuModel VM at runtime and
        // overrides this. Set on the page's root element, not a deeper child.
        Root.DataContext = MenuPageMockData.Data;
    }
}
