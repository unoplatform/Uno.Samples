namespace BrewHouse.Presentation.MockData;

// Design-time DataContext for HomePage. Mirrors HomeModel's binding surface with the real
// catalogue plus a small sample cart so the carousel, specials, featured grid and summary strip
// render in Hot Design / Studio. At runtime the navigation-injected generated HomeModel VM wins.
public partial record HomePageMockData
{
    public static HomePageMockData Data { get; } = new();

    public IReadOnlyList<HeroBanner> HeroBanners => CatalogData.HeroBanners;
    public IReadOnlyList<ProductItem> Specials =>
        CatalogData.AllProducts.Where(p => p.IsSpecial).ToList();
    public IReadOnlyList<ProductItem> FeaturedProducts =>
        CatalogData.AllProducts.Where(p => p.IsFeatured).ToList();
    public IReadOnlyList<CategoryItem> Categories => CatalogData.Categories;

    private static readonly IImmutableList<CartItem> SampleCart =
    [
        new("p-001", "Classic Latte", "", 5.50, 2),
    ];

    public IFeed<CartSummary> Summary => Feed.Async(async _ => new CartSummary(SampleCart));

    public void AddToCart(ProductItem product) { }
    public void OrderNow() { }
    public void GoToCart() { }
    public void GoToMenu() { }
    public void ViewProduct(ProductItem product) { }
}
