namespace BrewHouse.Presentation.MockData;

// Design-time DataContext for HomePage in Hot Design / Studio. The specials and featured sections
// are FeedViews, which can only subscribe to a FEED, so this mock is [ReactiveBindable] and its
// statics return the GENERATED ViewModel. See MenuPageMockData for the full rationale.
[Uno.Extensions.Reactive.ReactiveBindable]
public partial record HomePageMockData
{
    // Default design-time state: full catalogue, a small non-empty cart so the summary strip shows.
    public static HomePageMockDataViewModel Data => new();

    // A second design-time state: an empty cart, so the "your cart is empty" card shows in place of
    // the summary strip. The catalogue sections are unaffected.
    public static HomePageMockDataViewModel EmptyCart =>
        HomePageMockDataViewModel.ForModel(new()
        {
            Summary = new CartSummary(ImmutableList<CartItem>.Empty),
        });

    // A third design-time state: the catalogue came back with nothing, so both content sections fall
    // through to their NoneTemplate. Unreachable in the running app against the in-memory service,
    // which is precisely why it needs a preview.
    public static HomePageMockDataViewModel EmptyCatalog =>
        HomePageMockDataViewModel.ForModel(new()
        {
            Specials = EmptyProducts,
            FeaturedProducts = EmptyProducts,
        });

    private static IListFeed<ProductItem> EmptyProducts =>
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<ProductItem>>(ImmutableList<ProductItem>.Empty));

    public IListFeed<HeroBanner> HeroBanners { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<HeroBanner>>(CatalogData.HeroBanners.ToImmutableList()));

    // Mirrors the Model: the PipsPager binds NumberOfPages to this, so the design-time pager shows
    // the right page count instead of the control's default of 5.
    public int HeroBannerCount { get; init; } = CatalogData.HeroBanners.Count;

    public IListFeed<ProductItem> Specials { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<ProductItem>>(
            CatalogData.AllProducts.Where(p => p.IsSpecial).ToImmutableList()));

    public IListFeed<ProductItem> FeaturedProducts { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<ProductItem>>(
            CatalogData.AllProducts.Where(p => p.IsFeatured).ToImmutableList()));

    public IListFeed<CategoryItem> Categories { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<CategoryItem>>(CatalogData.Categories.ToImmutableList()));

    // The cart summary is the app's own state, not a request, so it stays a plain value here just as
    // the page binds it plainly. Init-settable so the EmptyCart variant can zero it.
    public CartSummary Summary { get; init; } =
        new(ImmutableList.Create(new CartItem("p-001", "Classic Latte", "", 5.50, 2)));

    public bool CartHasItems => Summary.HasItems;

    public void AddToCart(ProductItem product) { }
    public void OrderNow() { }
    public void GoToCart() { }
    public void GoToMenu() { }
    public void ViewProduct(ProductItem product) { }
}

// The generator's model-taking ViewModel constructor is protected; this partial reaches it from
// inside the class so the variants above can wrap a customized model.
public partial class HomePageMockDataViewModel
{
    internal static HomePageMockDataViewModel ForModel(HomePageMockData model) => new(model);
}
