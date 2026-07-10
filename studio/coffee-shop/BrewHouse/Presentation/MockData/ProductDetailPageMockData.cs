namespace BrewHouse.Presentation.MockData;

// Design-time DataContext for ProductDetailPage. Exposes the same binding surface as
// ProductDetailModel (a Product plus the command method names) with a representative product.
// At runtime the DataViewMap injects the tapped product's generated VM onto the page.
public partial record ProductDetailPageMockData
{
    public static ProductDetailPageMockData Data { get; } = new();

    public ProductItem Product => CatalogData.AllProducts[0];

    public void AddToCart() { }
    public void GoBack() { }
}
