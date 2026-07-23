namespace Voyago.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record SearchModel
{
    public string SearchPlaceholder { get; } = "Search destinations, flights, hotels...";

    public IReadOnlyList<string> PopularSearches { get; } = new[]
    {
        "Beach holidays", "City breaks", "Safari", "Ski resorts", "Island hopping"
    };

    public IReadOnlyList<Destination> PopularDestinations { get; } = new[]
    {
        Catalog.Dolomites, Catalog.Santorini, Catalog.Maldives,
        Catalog.Bali, Catalog.MachuPicchu, Catalog.Paris,
    };

    // Shared with Home — each tile opens its Featured destination when tapped.
    public IReadOnlyList<ExploreCategory> ExploreCategories { get; } = Catalog.Categories;
}
