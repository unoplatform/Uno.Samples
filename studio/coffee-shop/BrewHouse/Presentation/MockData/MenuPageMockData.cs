namespace BrewHouse.Presentation.MockData;

// Design-time DataContext for MenuPage. Mirrors MenuModel's binding surface with constant feeds so
// the chip bar and product grid render in Hot Design / Studio. At runtime the navigation-injected
// generated MenuModel VM overrides this.
public partial record MenuPageMockData
{
    public static MenuPageMockData Data { get; } = new();

    public string PageTitle => "Our Menu";

    public IState<string> SearchText => State.Value(this, () => string.Empty);
    public IState<string> CategoryId => State.Value(this, () => "all");

    public IListFeed<CategoryItem> Categories =>
        ListFeed.Async(async _ => CatalogData.Categories.ToImmutableList());

    public IListFeed<ProductItem> FilteredProducts =>
        ListFeed.Async(async _ => CatalogData.AllProducts.ToImmutableList());

    public IFeed<bool> HasNoResults => Feed.Async(async _ => false);

    public void AddToCart(ProductItem product) { }
    public void ViewProduct(ProductItem product) { }
    public void FilterByCategory(string categoryId) { }
}
