using BrewHouse.Presentation.MockData;
using BrewHouse.Presentation.Services;

namespace BrewHouse.Presentation;

// Landing page: hero carousel, today's specials, category shortcuts, featured products, and a cart
// summary strip. The catalogue is read from ICatalogService — asynchronously, like a real endpoint —
// so each section is an IListFeed the page renders through a FeedView (value / empty / failed /
// loading) instead of hand-rolled branches. The cart comes from the shared ICartService; navigation
// commands go through the injected INavigator.
public partial record HomeModel(ICatalogService Catalog, ICartService Cart, INavigator Navigator)
{
    // Cached so each section calls the service once per view lifetime rather than on every binding
    // evaluation.
    private IListFeed<HeroBanner>? _heroBanners;
    private IListFeed<ProductItem>? _specials;
    private IListFeed<ProductItem>? _featured;
    private IListFeed<CategoryItem>? _categories;

    // The promo carousel. Bound straight to the FlipView's ItemsSource rather than through a
    // FeedView: the carousel is decorative chrome whose empty and failed states have no sensible UI
    // of their own — an absent promo strip is simply an absent promo strip.
    public IListFeed<HeroBanner> HeroBanners =>
        _heroBanners ??= ListFeed.Async(Catalog.GetHeroBannersAsync);

    // The PipsPager's NumberOfPages. SelectData, not Select: an empty list feed emits None and
    // Select skips None, which would leave the pager on its default of 5 phantom pips.
    public IFeed<int> HeroBannerCount =>
        HeroBanners
            .AsFeed()
            .SelectData<IImmutableList<HeroBanner>, int>(banners => banners.SomeOrDefault()?.Count ?? 0);

    // The two content sections. Both are rendered by a FeedView, so "no specials today" and "the
    // catalogue is unreachable" are real, designed states rather than an empty gap.
    public IListFeed<ProductItem> Specials =>
        _specials ??= ListFeed.Async(Catalog.GetSpecialsAsync);

    public IListFeed<ProductItem> FeaturedProducts =>
        _featured ??= ListFeed.Async(Catalog.GetFeaturedAsync);

    // Category shortcuts — navigation chrome, like the carousel, so bound directly.
    public IListFeed<CategoryItem> Categories =>
        _categories ??= ListFeed.Async(Catalog.GetCategoriesAsync);

    // Cart summary strip. The shared always-scalar summary feed off the cart state, so the strip's
    // totals bind directly (e.g. {Binding Summary.SubtotalFormatted}) and update reactively.
    public IFeed<CartSummary> Summary => Cart.Summary;

    // Whether the cart has anything in it — drives which branch (summary strip vs. empty-cup card)
    // is visible, via a bool + BoolToVisibility converter in XAML. This is the cart's own state,
    // not a service request, so it stays a converter rather than a FeedView.
    public IFeed<bool> CartHasItems => Cart.Summary.Select(summary => summary.HasItems);

    public async ValueTask AddToCart(ProductItem product, CancellationToken ct)
        => await Cart.AddToCartAsync(product, ct);

    // Cross-tab navigation.
    public async ValueTask OrderNow(CancellationToken ct) => await Navigator.NavigateRouteAsync(this, "Menu", cancellation: ct);
    public async ValueTask GoToCart(CancellationToken ct) => await Navigator.NavigateRouteAsync(this, "Cart", cancellation: ct);
    public async ValueTask GoToMenu(CancellationToken ct) => await Navigator.NavigateRouteAsync(this, "Menu", cancellation: ct);

    public async ValueTask ViewProduct(ProductItem product, CancellationToken ct)
        => await Navigator.NavigateRouteAsync(this, "ProductDetail", data: product, cancellation: ct);
}
