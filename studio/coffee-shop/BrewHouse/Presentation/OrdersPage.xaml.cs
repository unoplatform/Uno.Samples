using BrewHouse.Presentation.MockData;

namespace BrewHouse.Presentation;

public sealed partial class OrdersPage : Page
{
    public OrdersPage()
    {
        this.InitializeComponent();

        // Hot Design renders this page without running Navigation, so seed a design-time DataContext
        // for the preview. Set it on the *page* (this.DataContext), never on a child element: at
        // runtime Navigation injects the generated OrdersModel VM as the page's DataContext, and a
        // child carrying its own explicit DataContext would shadow it.
        this.DataContext = OrdersPageMockData.Data;
    }
}
