namespace BrewHouse.Presentation.MockData;

// Design-time DataContext for MenuPage. Mirrors MenuModel's binding surface with plain, materialized
// values (not feeds) so the chip bar and product grid render in Hot Design / Studio with no errors —
// the live MenuModel surfaces feeds at runtime, but ItemsSource/Visibility/two-way bindings accept
// these plain values just the same. At runtime the navigation-injected generated MenuModel VM wins.
public partial record MenuPageMockData
{
    public static MenuPageMockData Data { get; } = new();

    public string PageTitle => "Our Menu";

    // Plain settable strings so the AutoSuggestBox/search two-way binds in Hot Design.
    public string SearchText { get; set; } = string.Empty;
    public string CategoryId { get; set; } = "all";

    public IReadOnlyList<CategoryItem> Categories => CatalogData.Categories;

    public IReadOnlyList<ProductItem> FilteredProducts => CatalogData.AllProducts;

    public bool HasNoResults => false;

    public void AddToCart(ProductItem product) { }
    public void ViewProduct(ProductItem product) { }
    public void FilterByCategory(string categoryId) { }
}
