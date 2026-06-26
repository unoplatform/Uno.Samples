using BrewHouse.Presentation.MockData;
using BrewHouse.Presentation.Services;

namespace BrewHouse.Presentation;

// The full menu with a live text search and a category chip filter. Both the search box and the
// selected category are two-way mutable states; the filtered product list and the chip selection
// are feeds derived from them, so the grid re-filters reactively as the user types or taps a chip
// (no Clear()+Add(); the derivation rebuilds the immutable list each time).
public partial record MenuModel(ICartService Cart, INavigator Navigator)
{
    public string PageTitle => "Our Menu";

    // Two-way bound to the search box.
    public IState<string> SearchText => State.Value(this, () => string.Empty);

    // The active category id ("all" by default); a tap on a chip sets it.
    public IState<string> CategoryId => State.Value(this, () => "all");

    // Chips with their selected flag recomputed from the active category.
    public IFeed<IImmutableList<CategoryItem>> Categories => CategoryId
        .Select(active => (IImmutableList<CategoryItem>)CatalogData.Categories
            .Select(c => c with { IsSelected = c.Id == active })
            .ToImmutableList());

    // The filtered product list: category + text search combined via Feed.Combine, so it recomputes
    // whenever either state changes — no Clear()+Add(), the whole immutable list is rebuilt and
    // re-projected. Exposed as a list feed so the grid binds its ItemsSource directly
    // (keeping the page VM as the ItemsRepeater DataContext, so item templates reach the page
    // commands via {utu:AncestorBinding}).
    public IListFeed<ProductItem> FilteredProducts =>
        Feed.Combine(CategoryId, SearchText)
            .Select(criteria => (IImmutableList<ProductItem>)Filter(criteria.Item1, criteria.Item2))
            .AsListFeed();

    // True when the current filter/search matches nothing — drives the "no results" empty state.
    // A scalar feed projection (never None), so it flips reliably even at zero matches.
    public IFeed<bool> HasNoResults =>
        Feed.Combine(CategoryId, SearchText)
            .Select(criteria => Filter(criteria.Item1, criteria.Item2).Count == 0);

    private static IImmutableList<ProductItem> Filter(string categoryId, string? searchText)
    {
        var search = (searchText ?? string.Empty).Trim();
        return CatalogData.AllProducts
            .Where(p => (categoryId == "all" || p.CategoryId == categoryId)
                        && (search.Length == 0
                            || p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                            || p.Description.Contains(search, StringComparison.OrdinalIgnoreCase)))
            .ToImmutableList();
    }

    public async ValueTask AddToCart(ProductItem product, CancellationToken ct)
        => await Cart.AddToCartAsync(product, ct);

    public async ValueTask ViewProduct(ProductItem product, CancellationToken ct)
        => await Navigator.NavigateRouteAsync(this, "ProductDetail", data: product, cancellation: ct);

    // The parameter is named so it does NOT match a state/feed property: MVUX injects the current
    // value of any parameter whose name matches a feed (here that would be CategoryId), which would
    // swallow the CommandParameter (the tapped chip's id) and re-select the active category instead.
    public async ValueTask FilterByCategory(string selectedCategoryId, CancellationToken ct)
        => await CategoryId.SetAsync(string.IsNullOrEmpty(selectedCategoryId) ? "all" : selectedCategoryId, ct);
}
