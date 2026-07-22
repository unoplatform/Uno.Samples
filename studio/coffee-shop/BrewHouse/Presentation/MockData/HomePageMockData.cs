namespace BrewHouse.Presentation.MockData;

// Design-time DataContext for HomePage. Mirrors HomeModel's binding surface with the real
// catalogue plus a small sample cart so the carousel, specials, featured grid and summary strip
// render in Hot Design / Studio. At runtime the navigation-injected generated HomeModel VM wins.
public partial record HomePageMockData
{
    public static HomePageMockData Data { get; } = new();

    public IReadOnlyList<HeroBanner> HeroBanners => CatalogData.HeroBanners;
    // Mirror HomeModel: the PipsPager binds NumberOfPages to this, so the design-time pager shows the
    // right page count in Hot Design instead of the control's default.
    public int HeroBannerCount => HeroBanners.Count;
    public IReadOnlyList<ProductItem> Specials =>
        CatalogData.AllProducts.Where(p => p.IsSpecial).ToList();
    public IReadOnlyList<ProductItem> FeaturedProducts =>
        CatalogData.AllProducts.Where(p => p.IsFeatured).ToList();
    public IReadOnlyList<CategoryItem> Categories => CatalogData.Categories;

    private static readonly IImmutableList<CartItem> SampleCart =
    [
        new("p-001", "Classic Latte", "", 5.50, 2),
    ];

    // Plain CartSummary (not a feed) so the summary strip binds directly (e.g.
    // {Binding Summary.SubtotalFormatted}) in Hot Design; CartHasItems chooses the visible branch.
    public CartSummary Summary => new(SampleCart);
    public bool CartHasItems => Summary.HasItems;

    public void AddToCart(ProductItem product) { }
    public void OrderNow() { }
    public void GoToCart() { }
    public void GoToMenu() { }
    public void ViewProduct(ProductItem product) { }
}
