using BrewHouse.Presentation.MockData;

namespace BrewHouse.Presentation;

public sealed partial class ProductDetailPage : Page
{
    public ProductDetailPage()
    {
        this.InitializeComponent();

        // Hot Design fallback: at runtime the DataViewMap injects a ProductDetailModel built from
        // the tapped product and overrides this. Set on the *page* DataContext (not a child) so the
        // injected per-product model wins — setting a child's DataContext would shadow it.
        this.DataContext = ProductDetailPageMockData.Data;
    }
}
