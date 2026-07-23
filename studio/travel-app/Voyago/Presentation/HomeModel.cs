namespace Voyago.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record HomeModel
{
    public string GreetingText { get; } = "Where do you want to explore today?";
    public string UserName { get; } = "Alex";
    public string UserInitials { get; } = "AJ";

    // Hero carousel destinations
    public IReadOnlyList<Destination> HeroDestinations { get; } = new[]
    {
        Catalog.Dolomites, Catalog.Maldives, Catalog.Kyoto,
    };

    // Bind PipsPager.NumberOfPages to this int (a {Binding Collection.Count} on an array
    // silently fails and the pager keeps its default 5 pips — lesson 49).
    public int HeroCount => HeroDestinations.Count;

    // Quick actions (the icon is derived from the label in the view, so no glyph lives here)
    public IReadOnlyList<QuickAction> QuickActions { get; } = new[]
    {
        new QuickAction("qa-01", "Flights"),
        new QuickAction("qa-02", "Hotels"),
        new QuickAction("qa-03", "Experiences"),
        new QuickAction("qa-04", "Cars"),
        new QuickAction("qa-05", "Trips"),
        new QuickAction("qa-06", "Map"),
    };

    // Recommended trips
    public IReadOnlyList<Destination> RecommendedTrips { get; } = new[]
    {
        Catalog.Santorini, Catalog.Bali, Catalog.Paris, Catalog.MachuPicchu,
    };

    // Explore categories (shared with Search) — each opens its Featured destination when tapped.
    public IReadOnlyList<ExploreCategory> ExploreCategories { get; } = Catalog.Categories;
}

// Page-local record — QuickAction is only used on HomePage
public partial record QuickAction(string Id, string Label);