namespace BrewHouse.Presentation.MockData;

// Design-time DataContext for HomePage. Mirrors HomeModel's binding surface with the real
// catalogue plus a small sample cart so the carousel, specials, featured grid and summary strip
// render in Hot Design / Studio. At runtime the navigation-injected generated HomeModel VM wins.
public partial record HomePageMockData
{
    public IReadOnlyList<HeroBanner> HeroBanners => CatalogData.HeroBanners;
    // Mirror HomeModel: the PipsPager binds NumberOfPages to this, so the design-time pager shows the
    // right page count in Hot Design instead of the control's default.
    public int HeroBannerCount => HeroBanners.Count;
    public IReadOnlyList<ProductItem> Specials =>
        CatalogData.AllProducts.Where(p => p.IsSpecial).ToList();
    public IReadOnlyList<ProductItem> FeaturedProducts =>
        CatalogData.AllProducts.Where(p => p.IsFeatured).ToList();
    public IReadOnlyList<CategoryItem> Categories => CatalogData.Categories;

    // DECLARED FIRST, above the statics below that construct instances: static members initialize in
    // textual order, so a `Data { get; } = new()` placed above this field would run the instance
    // initializer while SampleCart is still null — leaving Data.Cart null with no exception at all,
    // and the summary strip silently rendering as an empty cart.
    private static readonly IImmutableList<CartItem> SampleCart =
    [
        new("p-001", "Classic Latte", "", 5.50, 2),
    ];

    // Default design-time state: a small non-empty cart, so the summary strip shows.
    public static HomePageMockData Data { get; } = new();

    // A second design-time state: an empty cart, so the "add something" card shows in place of the
    // summary strip. The "Home — Empty Cart" preview uses this.
    public static HomePageMockData EmptyCart { get; } = new() { Cart = [] };

    // Init-settable so a variant (see EmptyCart) can supply no items; defaults to the sample cart.
    public IImmutableList<CartItem> Cart { get; init; } = SampleCart;

    // Plain CartSummary (not a feed) so the summary strip binds directly (e.g.
    // {Binding Summary.SubtotalFormatted}) in Hot Design; CartHasItems chooses the visible branch.
    public CartSummary Summary => new(Cart);
    public bool CartHasItems => Summary.HasItems;

    public void AddToCart(ProductItem product) { }
    public void OrderNow() { }
    public void GoToCart() { }
    public void GoToMenu() { }
    public void ViewProduct(ProductItem product) { }
}
