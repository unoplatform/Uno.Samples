using Voyago.Presentation.Services;

namespace Voyago.Presentation;

// The search page. The query is a two-way state and the results are a list feed that asks
// IDiscoveryService for the matching set whenever it changes — the search runs on the SERVICE, not
// over a local copy of the catalogue, which is what gives the page real loading, empty and failure
// states to render.
//
// Before this, SearchModel was a [ReactiveBindable(false)] record with three fixed lists and a
// SearchPlaceholder string bound to a TextBlock: a search page you could not search with.
public partial record SearchModel(IDiscoveryService Discovery)
{
    public string SearchPlaceholder { get; } = "Search destinations, flights, hotels...";

    // Two-way bound to the search box.
    public IState<string> Query => State.Value(this, () => string.Empty);

    // The results grid's source. An empty query returns the popular set (what the page shows before
    // the traveller types); a query matching nothing returns an empty list, which the FeedView
    // renders as its NoneTemplate.
    public IListFeed<Destination> Results =>
        Query
            .SelectAsync(async (query, ct) => await Discovery.SearchDestinationsAsync(query, ct))
            .AsListFeed();

    private IListFeed<string>? _popularSearches;
    public IListFeed<string> PopularSearches =>
        _popularSearches ??= ListFeed.Async(Discovery.GetPopularSearchesAsync);

    // Shared with Home — each tile opens its Featured destination when tapped.
    private IListFeed<ExploreCategory>? _categories;
    public IListFeed<ExploreCategory> ExploreCategories =>
        _categories ??= ListFeed.Async(Discovery.GetCategoriesAsync);

    // Tapping a popular-search chip fills the box, which re-runs the service query.
    public async ValueTask ApplySearch(string term, CancellationToken ct)
        => await Query.SetAsync(term ?? string.Empty, ct);
}
