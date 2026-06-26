using BrewHouse.Presentation.MockData;
using BrewHouse.Presentation.Services;

namespace BrewHouse.Presentation;

// Landing page: hero carousel, today's specials, category shortcuts, featured products, and a cart
// summary strip. Reads the catalogue and the shared cart from the injected service; navigation
// commands go through the injected INavigator.
public partial record HomeModel(ICartService Cart, INavigator Navigator)
{
    public IReadOnlyList<HeroBanner> HeroBanners { get; } = CatalogData.HeroBanners;

    // Derived from the shared catalogue so Home stays in sync with the Menu.
    public IReadOnlyList<ProductItem> Specials { get; } =
        CatalogData.AllProducts.Where(p => p.IsSpecial).ToList();
    public IReadOnlyList<ProductItem> FeaturedProducts { get; } =
        CatalogData.AllProducts.Where(p => p.IsFeatured).ToList();
    public IReadOnlyList<CategoryItem> Categories { get; } = CatalogData.Categories;

    // Cart summary strip. A scalar feed projection (never None) so the strip's totals bind directly
    // (e.g. {Binding Summary.SubtotalFormatted}) and update reactively as items change.
    public IFeed<CartSummary> Summary => Cart.Cart
        .AsFeed()
        .Select(items => new CartSummary(items));

    // Whether the cart has anything in it — drives which branch (summary strip vs. empty-cup card)
    // is visible, via a bool + BoolToVisibility converter in XAML. A scalar projection
    // so it flips reliably even at zero items.
    public IFeed<bool> CartHasItems => Cart.Cart
        .AsFeed()
        .Select(items => items.Sum(i => i.Quantity) > 0);

    public async ValueTask AddToCart(ProductItem product, CancellationToken ct)
        => await Cart.AddToCartAsync(product, ct);

    // Cross-tab navigation.
    public async ValueTask OrderNow(CancellationToken ct) => await Navigator.NavigateRouteAsync(this, "Menu", cancellation: ct);
    public async ValueTask GoToCart(CancellationToken ct) => await Navigator.NavigateRouteAsync(this, "Cart", cancellation: ct);
    public async ValueTask GoToMenu(CancellationToken ct) => await Navigator.NavigateRouteAsync(this, "Menu", cancellation: ct);

    public async ValueTask ViewProduct(ProductItem product, CancellationToken ct)
        => await Navigator.NavigateRouteAsync(this, "ProductDetail", data: product, cancellation: ct);
}
