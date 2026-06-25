using BrewHouse.Presentation.MockData;

namespace BrewHouse.Presentation;

public sealed partial class OrdersPage : Page
{
    public OrdersPage()
    {
        this.InitializeComponent();

        // Hot Design renders this page without running Navigation, so seed a representative
        // DataContext for the preview. At runtime Uno.Extensions Navigation injects the
        // generated OrdersModel bindable VM onto the page and overrides this.
        Root.DataContext = OrdersPageMockData.Data;
    }
}
