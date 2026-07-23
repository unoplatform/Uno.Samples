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
            "https://images.pexels.com/photos/28491959/pexels-photo-28491959.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "From EUR 249", 4.9, 2341),
        new Destination("d-002", "Maldives", "Indian Ocean", "Crystal waters, endless horizons",
            "https://images.pexels.com/photos/28843967/pexels-photo-28843967.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "From EUR 899", 4.8, 1875),
        new Destination("d-003", "Kyoto", "Japan", "Ancient temples, timeless beauty",
            "https://images.pexels.com/photos/16481404/pexels-photo-16481404.jpeg?auto=compress&cs=tinysrgb&w=1200",
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
            "https://images.pexels.com/photos/1010657/pexels-photo-1010657.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "From EUR 399", 4.8, 1562),
        new Destination("d-005", "Bali", "Indonesia", "Lush terraces and spiritual calm",
            "https://images.pexels.com/photos/5933066/pexels-photo-5933066.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "From EUR 549", 4.7, 2087),
        new Destination("d-006", "Paris", "France", "Romance, cuisine, and art",
            "https://images.pexels.com/photos/532826/pexels-photo-532826.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "From EUR 299", 4.6, 4210),
        new Destination("d-007", "Machu Picchu", "Peru", "Lost city high in the Andes",
            "https://images.pexels.com/photos/2929906/pexels-photo-2929906.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "From EUR 729", 4.9, 987),
    };

    // Explore categories
    public IReadOnlyList<ExploreCategory> ExploreCategories { get; } = new[]
    {
        new ExploreCategory("ec-01", "City Breaks",
            "https://images.pexels.com/photos/20847307/pexels-photo-20847307.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "Urban adventures in iconic metropolises"),
        new ExploreCategory("ec-02", "Nature Escapes",
            "https://images.pexels.com/photos/417074/pexels-photo-417074.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "Reconnect with the wild"),
        new ExploreCategory("ec-03", "Romantic Getaways",
            "https://images.pexels.com/photos/3546189/pexels-photo-3546189.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "Unforgettable moments for two"),
        new ExploreCategory("ec-04", "Cultural Trips",
            "https://images.pexels.com/photos/15890613/pexels-photo-15890613.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "History, heritage, and discovery"),
    };
}

// Page-local record — QuickAction is only used on HomePage
public partial record QuickAction(string Id, string Label);