namespace Voyago.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record FavoritesModel
{
    public int TotalFavorites { get; } = 7;

    public IReadOnlyList<Destination> SavedDestinations { get; } = new[]
    {
        new Destination("d-004", "Santorini", "Greece", "Cliffs, caldera views, and sunsets",
            "https://picsum.photos/seed/santorini%20greece%20island/1280/720",
            "From EUR 399", 4.8, 1562),
        new Destination("d-001", "Dolomites", "Italy", "Alpine serenity above the clouds",
            "https://picsum.photos/seed/mountain%20landscape%20travel/1280/720",
            "From EUR 249", 4.9, 2341),
        new Destination("d-002", "Maldives", "Indian Ocean", "Crystal waters, endless horizons",
            "https://picsum.photos/seed/tropical%20beach%20destination/1280/720",
            "From EUR 899", 4.8, 1875),
        new Destination("d-007", "Machu Picchu", "Peru", "Lost city high in the Andes",
            "https://picsum.photos/seed/machu%20picchu%20peru%20ancient%20ruins/1280/720",
            "From EUR 729", 4.9, 987),
        new Destination("d-003", "Kyoto", "Japan", "Ancient temples, timeless beauty",
            "https://picsum.photos/seed/tokyo%20japan%20cityscape%20night/1280/720",
            "From EUR 629", 4.7, 3102),
        new Destination("d-005", "Bali", "Indonesia", "Lush terraces and spiritual calm",
            "https://picsum.photos/seed/bali%20rice%20terraces%20nature/1280/720",
            "From EUR 549", 4.7, 2087),
        new Destination("d-006", "Paris", "France", "Romance, cuisine, and art",
            "https://picsum.photos/seed/paris%20eiffel%20tower/1280/720",
            "From EUR 299", 4.6, 4210),
    };
}
