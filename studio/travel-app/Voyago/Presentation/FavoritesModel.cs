namespace Voyago.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record FavoritesModel
{
    public int TotalFavorites { get; } = 7;

    public IReadOnlyList<Destination> SavedDestinations { get; } = new[]
    {
        new Destination("d-004", "Santorini", "Greece", "Cliffs, caldera views, and sunsets",
            "ms-appx:///Assets/Photos/santorini.jpg",
            "From EUR 399", 4.8, 1562),
        new Destination("d-001", "Dolomites", "Italy", "Alpine serenity above the clouds",
            "ms-appx:///Assets/Photos/dolomites.jpg",
            "From EUR 249", 4.9, 2341),
        new Destination("d-002", "Maldives", "Indian Ocean", "Crystal waters, endless horizons",
            "ms-appx:///Assets/Photos/maldives.jpg",
            "From EUR 899", 4.8, 1875),
        new Destination("d-007", "Machu Picchu", "Peru", "Lost city high in the Andes",
            "ms-appx:///Assets/Photos/machupicchu.jpg",
            "From EUR 729", 4.9, 987),
        new Destination("d-003", "Kyoto", "Japan", "Ancient temples, timeless beauty",
            "ms-appx:///Assets/Photos/kyoto.jpg",
            "From EUR 629", 4.7, 3102),
        new Destination("d-005", "Bali", "Indonesia", "Lush terraces and spiritual calm",
            "ms-appx:///Assets/Photos/bali.jpg",
            "From EUR 549", 4.7, 2087),
        new Destination("d-006", "Paris", "France", "Romance, cuisine, and art",
            "ms-appx:///Assets/Photos/paris.jpg",
            "From EUR 299", 4.6, 4210),
    };
}
