using BrewHouse.Presentation.MockData;

namespace BrewHouse.Presentation;

public sealed partial class MenuPage : Page
{
    public MenuPage()
    {
        this.InitializeComponent();

        // Hot Design fallback for the preview; at runtime Navigation injects the generated MenuModel
        // VM as the page's DataContext. Set on the *page* (this.DataContext), never a child element:
        // a child with its own DataContext would shadow the injected VM, leaving every binding on the
        // inert mock.
        this.DataContext = MenuPageMockData.Data;
    }
}
