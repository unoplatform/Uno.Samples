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
        new Destination("d-001", "Dolomites", "Italy", "Alpine serenity above the clouds",
            "https://images.pexels.com/photos/28491959/pexels-photo-28491959.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "From EUR 249", 4.9, 2341),
        new Destination("d-004", "Santorini", "Greece", "Cliffs, caldera views, and sunsets",
            "https://images.pexels.com/photos/1010657/pexels-photo-1010657.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "From EUR 399", 4.8, 1562),
        new Destination("d-002", "Maldives", "Indian Ocean", "Crystal waters, endless horizons",
            "https://images.pexels.com/photos/28843967/pexels-photo-28843967.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "From EUR 899", 4.8, 1875),
        new Destination("d-005", "Bali", "Indonesia", "Lush terraces and spiritual calm",
            "https://images.pexels.com/photos/5933066/pexels-photo-5933066.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "From EUR 549", 4.7, 2087),
        new Destination("d-007", "Machu Picchu", "Peru", "Lost city high in the Andes",
            "https://images.pexels.com/photos/2929906/pexels-photo-2929906.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "From EUR 729", 4.9, 987),
        new Destination("d-006", "Paris", "France", "Romance, cuisine, and art",
            "https://images.pexels.com/photos/532826/pexels-photo-532826.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "From EUR 299", 4.6, 4210),
    };

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
