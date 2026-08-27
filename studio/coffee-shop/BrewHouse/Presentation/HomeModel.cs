using BrewHouse.Presentation.MockData;
using BrewHouse.Presentation.Services;

namespace BrewHouse.Presentation;

// Landing page: hero carousel, today's specials, category shortcuts, featured products, and a cart
// summary strip. Reads the catalogue and the shared cart from the injected service; navigation
// commands go through the injected INavigator.
public partial record HomeModel(ICartService Cart, INavigator Navigator)
{
    public IReadOnlyList<HeroBanner> HeroBanners { get; } = CatalogData.HeroBanners;

    // Bind the PipsPager's NumberOfPages to this, NOT to {Binding HeroBanners.Count}: the banners are an
    // IReadOnlyList backed by an array, and classic {Binding} can't reach an array's explicit-interface
    // Count, so the pager silently falls back to its default of 5 pips (showing phantom slides).
    public int HeroBannerCount => HeroBanners.Count;

    // Derived from the shared catalogue so Home stays in sync with the Menu.
    public IReadOnlyList<ProductItem> Specials { get; } =
        CatalogData.AllProducts.Where(p => p.IsSpecial).ToList();
    public IReadOnlyList<ProductItem> FeaturedProducts { get; } =
        CatalogData.AllProducts.Where(p => p.IsFeatured).ToList();
    public IReadOnlyList<CategoryItem> Categories { get; } = CatalogData.Categories;

    // Cart summary strip. The shared always-scalar summary feed off the cart state, so the strip's
    // totals bind directly (e.g. {Binding Summary.SubtotalFormatted}) and update reactively.
    public IFeed<CartSummary> Summary => Cart.Summary;

    // Whether the cart has anything in it — drives which branch (summary strip vs. empty-cup card)
    // is visible, via a bool + BoolToVisibility converter in XAML.
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
