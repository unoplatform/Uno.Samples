namespace BrewHouse.Presentation;

public sealed partial class ProductDetailPage : Page
{
    public ProductDetailPage()
    {
        this.InitializeComponent();

        // Hot Design fallback (unconditional): at runtime the DataViewMap injects a
        // ProductDetailModel built from the tapped product and overrides this.
        this.DataContext = new ProductDetailModel(AppState.Current.AllProducts[0], AppState.Current);
    }
}
