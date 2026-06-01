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
            "https://picsum.photos/seed/mountain%20landscape%20travel/1280/720",
            "From EUR 249", 4.9, 2341),
        new Destination("d-004", "Santorini", "Greece", "Cliffs, caldera views, and sunsets",
            "https://picsum.photos/seed/santorini%20greece%20island/1280/720",
            "From EUR 399", 4.8, 1562),
        new Destination("d-002", "Maldives", "Indian Ocean", "Crystal waters, endless horizons",
            "https://picsum.photos/seed/tropical%20beach%20destination/1280/720",
            "From EUR 899", 4.8, 1875),
        new Destination("d-005", "Bali", "Indonesia", "Lush terraces and spiritual calm",
            "https://picsum.photos/seed/bali%20rice%20terraces%20nature/1280/720",
            "From EUR 549", 4.7, 2087),
        new Destination("d-007", "Machu Picchu", "Peru", "Lost city high in the Andes",
            "https://picsum.photos/seed/machu%20picchu%20peru%20ancient%20ruins/1280/720",
            "From EUR 729", 4.9, 987),
        new Destination("d-006", "Paris", "France", "Romance, cuisine, and art",
            "https://picsum.photos/seed/paris%20eiffel%20tower/1280/720",
            "From EUR 299", 4.6, 4210),
    };

    public IReadOnlyList<ExploreCategory> ExploreCategories { get; } = new[]
    {
        new ExploreCategory("ec-01", "City Breaks",
            "https://picsum.photos/seed/city%20breaks%20travel%20urban/1280/720",
            "Urban adventures in iconic metropolises"),
        new ExploreCategory("ec-02", "Nature Escapes",
            "https://picsum.photos/seed/nature%20escape%20forest%20mountains%20hiking/1280/720",
            "Reconnect with the wild"),
        new ExploreCategory("ec-03", "Romantic Getaways",
            "https://picsum.photos/seed/romantic%20couples%20travel%20sunset/1280/720",
            "Unforgettable moments for two"),
        new ExploreCategory("ec-04", "Cultural Trips",
            "https://picsum.photos/seed/cultural%20heritage%20ancient%20temple/1280/720",
            "History, heritage, and discovery"),
    };
}
