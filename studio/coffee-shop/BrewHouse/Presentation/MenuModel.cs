using BrewHouse.Presentation.MockData;
using BrewHouse.Presentation.Services;

namespace BrewHouse.Presentation;

// The full menu with a live text search and a category chip filter. Both inputs are two-way states;
// the product list is a list-feed that asks ICatalogService for the matching set whenever either
// changes — the search runs on the SERVICE, not on a client-side copy of the catalogue, which is
// what gives the page real loading, empty and failure states to render.
public partial record MenuModel(ICatalogService Catalog, ICartService Cart, INavigator Navigator)
{
    // Two-way bound to the search box.
    public IState<string> SearchText => State.Value(this, () => string.Empty);

    // The active category id ("all" by default); a tap on a chip sets it.
    public IState<string> CategoryId => State.Value(this, () => "all");

    // The chip set, fetched once and cached, so tapping a chip re-projects the selected flag
    // locally instead of re-hitting the service (which would flash the whole bar on every tap).
    private IFeed<IImmutableList<CategoryItem>>? _allCategories;
    private IFeed<IImmutableList<CategoryItem>> AllCategories =>
        _allCategories ??= Feed.Async(Catalog.GetCategoriesAsync);

    // Chips with their selected flag recomputed from the active category. Bound directly rather than
    // through a FeedView: the filter bar is chrome, and "no categories" / "categories failed" have
    // no meaningful UI of their own.
    public IListFeed<CategoryItem> Categories =>
        Feed.Combine(AllCategories, CategoryId)
            .Select(criteria => (IImmutableList<CategoryItem>)criteria.Item1
                .Select(c => c with { IsSelected = c.Id == criteria.Item2 })
                .ToImmutableList())
            .AsListFeed();

    // The product grid's source: category + search text combined, then handed to the service. The
    // page renders it through a FeedView, so an empty result is the NoneTemplate ("No matches
    // found") and a failed request is the ErrorTemplate — no bool feed, no visibility converter.
    public IListFeed<ProductItem> FilteredProducts =>
        Feed.Combine(CategoryId, SearchText)
            .SelectAsync(async (criteria, ct) =>
                await Catalog.SearchProductsAsync(criteria.Item1, criteria.Item2, ct))
            .AsListFeed();

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
