namespace BrewHouse.Presentation.MockData;

// Design-time DataContext for MenuPage. Mirrors MenuModel's binding surface with plain, materialized
// values (not feeds) so the chip bar and product grid render in Hot Design / Studio with no errors —
// the live MenuModel surfaces feeds at runtime, but ItemsSource/Visibility/two-way bindings accept
// these plain values just the same. At runtime the navigation-injected generated MenuModel VM wins.
public partial record MenuPageMockData
{
    // Default design-time state: the whole catalogue.
    public static MenuPageMockData Data { get; } = new();

    // A second design-time state: an active search that matches nothing, so the product grid is
    // empty and the "No matches found" panel shows. The "Menu — No Results" preview uses this to
    // demonstrate previewing the same page in more than one data state.
    public static MenuPageMockData NoResults { get; } = new()
    {
        SearchText = "unicorn frappé",
        FilteredProducts = [],
        HasNoResults = true,
    };

    // Plain settable strings so the AutoSuggestBox/search two-way binds in Hot Design.
    public string SearchText { get; set; } = string.Empty;
    public string CategoryId { get; set; } = "all";

    public IReadOnlyList<CategoryItem> Categories => CatalogData.Categories;

    // Init-settable so a variant (see NoResults) can supply an empty result set; defaults to the
    // full catalogue for the standard design-time DataContext.
    public IReadOnlyList<ProductItem> FilteredProducts { get; init; } = CatalogData.AllProducts;

    public bool HasNoResults { get; init; }

    public void AddToCart(ProductItem product) { }
    public void ViewProduct(ProductItem product) { }
    public void FilterByCategory(string categoryId) { }
}
