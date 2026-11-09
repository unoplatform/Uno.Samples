namespace BrewHouse.Presentation.MockData;

// Design-time DataContext for ProductDetailPage. Exposes the same binding surface as
// ProductDetailModel (a Product plus the command method names) with a representative product.
// At runtime the DataViewMap injects the tapped product's generated VM onto the page.
public partial record ProductDetailPageMockData
{
    // Default: a "Today's Special" product (Classic Latte), so the special badge shows.
    public static ProductDetailPageMockData Data { get; } = new();

    // A standard (non-special) product (Cappuccino), so the special badge is hidden. The
    // "Product — Standard" preview uses this to show the page without the badge.
    public static ProductDetailPageMockData Standard { get; } = new() { Product = CatalogData.AllProducts[1] };

    // Init-settable so a variant (see Standard) can swap the product; defaults to the first
    // catalogue item.
    public ProductItem Product { get; init; } = CatalogData.AllProducts[0];

    public void AddToCart() { }
    public void GoBack() { }
}
