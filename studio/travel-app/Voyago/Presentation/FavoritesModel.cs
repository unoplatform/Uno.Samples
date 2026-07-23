namespace Voyago.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record FavoritesModel
{
    public int TotalFavorites { get; } = 7;

    public IReadOnlyList<Destination> SavedDestinations { get; } = new[]
    {
        new Destination("d-004", "Santorini", "Greece", "Cliffs, caldera views, and sunsets",
            "https://images.pexels.com/photos/1010657/pexels-photo-1010657.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "From EUR 399", 4.8, 1562),
        new Destination("d-001", "Dolomites", "Italy", "Alpine serenity above the clouds",
            "https://images.pexels.com/photos/28491959/pexels-photo-28491959.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "From EUR 249", 4.9, 2341),
        new Destination("d-002", "Maldives", "Indian Ocean", "Crystal waters, endless horizons",
            "https://images.pexels.com/photos/28843967/pexels-photo-28843967.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "From EUR 899", 4.8, 1875),
        new Destination("d-007", "Machu Picchu", "Peru", "Lost city high in the Andes",
            "https://images.pexels.com/photos/2929906/pexels-photo-2929906.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "From EUR 729", 4.9, 987),
        new Destination("d-003", "Kyoto", "Japan", "Ancient temples, timeless beauty",
            "https://images.pexels.com/photos/16481404/pexels-photo-16481404.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "From EUR 629", 4.7, 3102),
        new Destination("d-005", "Bali", "Indonesia", "Lush terraces and spiritual calm",
            "https://images.pexels.com/photos/5933066/pexels-photo-5933066.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "From EUR 549", 4.7, 2087),
        new Destination("d-006", "Paris", "France", "Romance, cuisine, and art",
            "https://images.pexels.com/photos/532826/pexels-photo-532826.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "From EUR 299", 4.6, 4210),
    };
}
