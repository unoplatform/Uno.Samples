namespace Voyago.Presentation.MockData;

/// <summary>
/// Design-time DataContext for <see cref="SearchPage"/> in Hot Design / Studio, built to the recipe
/// documented on <see cref="HomePageMockData"/>. The query is a plain settable string so the search
/// box two-way binds at design time; the results and the popular-search chips stay list feeds because
/// FeedViews render them.
/// </summary>
[ReactiveBindable]
public partial record SearchPageMockData
{
    // Declared first (rule 3): the popular searches are chrome, defined nowhere else.
    private static readonly IImmutableList<string> PopularSearchSeed =
        ["Beach holidays", "City breaks", "Safari", "Ski resorts", "Island hopping"];

    // Default: the empty query, which the service answers with the popular set.
    public static SearchPageMockDataViewModel Data => new();

    // A query that matches nothing, so the results FeedView falls through to its NoneTemplate.
    public static SearchPageMockDataViewModel NoResults =>
        SearchPageMockDataViewModel.ForModel(new()
        {
            Query = "atlantis",
            Results = ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<Destination>>(
                ImmutableList<Destination>.Empty)),
        });

    public string SearchPlaceholder { get; init; } = "Search destinations, flights, hotels...";
    public string Query { get; set; } = string.Empty;

    public IListFeed<Destination> Results { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<Destination>>(
            ImmutableList.Create(
                Catalog.Dolomites, Catalog.Santorini, Catalog.Maldives,
                Catalog.Bali, Catalog.MachuPicchu, Catalog.Paris)));

    public IListFeed<string> PopularSearches { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult(PopularSearchSeed));

    public IListFeed<ExploreCategory> ExploreCategories { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<ExploreCategory>>(
            Catalog.Categories.ToImmutableList()));

    public async ValueTask ApplySearch(string term, CancellationToken ct) => await ValueTask.CompletedTask;
}

// Reaches the generated ViewModel's protected model-taking constructor, so NoResults can wrap a
// customized model. See HomePageMockDataViewModel for the full explanation.
public partial class SearchPageMockDataViewModel
{
    internal static SearchPageMockDataViewModel ForModel(SearchPageMockData model) => new(model);
}
