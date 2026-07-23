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
        new Destination("d-001", "Dolomites", "Italy", "Alpine serenity above the clouds",
            "ms-appx:///Assets/Photos/dolomites.jpg",
            "From EUR 249", 4.9, 2341),
        new Destination("d-002", "Maldives", "Indian Ocean", "Crystal waters, endless horizons",
            "ms-appx:///Assets/Photos/maldives.jpg",
            "From EUR 899", 4.8, 1875),
        new Destination("d-003", "Kyoto", "Japan", "Ancient temples, timeless beauty",
            "ms-appx:///Assets/Photos/kyoto.jpg",
            "From EUR 629", 4.7, 3102),
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
        new Destination("d-004", "Santorini", "Greece", "Cliffs, caldera views, and sunsets",
            "ms-appx:///Assets/Photos/santorini.jpg",
            "From EUR 399", 4.8, 1562),
        new Destination("d-005", "Bali", "Indonesia", "Lush terraces and spiritual calm",
            "ms-appx:///Assets/Photos/bali.jpg",
            "From EUR 549", 4.7, 2087),
        new Destination("d-006", "Paris", "France", "Romance, cuisine, and art",
            "ms-appx:///Assets/Photos/paris.jpg",
            "From EUR 299", 4.6, 4210),
        new Destination("d-007", "Machu Picchu", "Peru", "Lost city high in the Andes",
            "ms-appx:///Assets/Photos/machupicchu.jpg",
            "From EUR 729", 4.9, 987),
    };

    // Explore categories
    public IReadOnlyList<ExploreCategory> ExploreCategories { get; } = new[]
    {
        new ExploreCategory("ec-01", "City Breaks",
            "ms-appx:///Assets/Photos/citybreaks.jpg",
            "Urban adventures in iconic metropolises"),
        new ExploreCategory("ec-02", "Nature Escapes",
            "ms-appx:///Assets/Photos/natureescapes.jpg",
            "Reconnect with the wild"),
        new ExploreCategory("ec-03", "Romantic Getaways",
            "ms-appx:///Assets/Photos/romantic.jpg",
            "Unforgettable moments for two"),
        new ExploreCategory("ec-04", "Cultural Trips",
            "ms-appx:///Assets/Photos/cultural.jpg",
            "History, heritage, and discovery"),
    };
}

// Page-local record — QuickAction is only used on HomePage
public partial record QuickAction(string Id, string Label);