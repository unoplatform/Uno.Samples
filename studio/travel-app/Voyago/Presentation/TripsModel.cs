namespace Voyago.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record TripsModel
{
    public IReadOnlyList<TripItem> UpcomingTrips { get; } = new[]
    {
        new TripItem("tr-001", "Santorini", "Greece",
            "https://images.pexels.com/photos/1010657/pexels-photo-1010657.jpeg?auto=compress&cs=tinysrgb&w=1200",
            new DateOnly(2026, 7, 12), new DateOnly(2026, 7, 22),
            "Confirmed", "VYG-48291"),
        new TripItem("tr-002", "Bali", "Indonesia",
            "https://images.pexels.com/photos/5933066/pexels-photo-5933066.jpeg?auto=compress&cs=tinysrgb&w=1200",
            new DateOnly(2026, 9, 5), new DateOnly(2026, 9, 18),
            "Confirmed", "VYG-50134"),
        new TripItem("tr-003", "Machu Picchu", "Peru",
            "https://images.pexels.com/photos/2929906/pexels-photo-2929906.jpeg?auto=compress&cs=tinysrgb&w=1200",
            new DateOnly(2026, 11, 3), new DateOnly(2026, 11, 13),
            "Pending", "VYG-52067"),
    };

    public IReadOnlyList<TripItem> PastTrips { get; } = new[]
    {
        new TripItem("tr-004", "Paris", "France",
            "https://images.pexels.com/photos/532826/pexels-photo-532826.jpeg?auto=compress&cs=tinysrgb&w=1200",
            new DateOnly(2025, 10, 14), new DateOnly(2025, 10, 21),
            "Completed", "VYG-39812"),
        new TripItem("tr-005", "Dolomites", "Italy",
            "https://images.pexels.com/photos/28491959/pexels-photo-28491959.jpeg?auto=compress&cs=tinysrgb&w=1200",
            new DateOnly(2025, 6, 1), new DateOnly(2025, 6, 9),
            "Completed", "VYG-36450"),
        new TripItem("tr-006", "Tokyo", "Japan",
            "https://images.pexels.com/photos/29662430/pexels-photo-29662430.jpeg?auto=compress&cs=tinysrgb&w=1200",
            new DateOnly(2024, 12, 20), new DateOnly(2025, 1, 4),
            "Completed", "VYG-31199"),
    };
}
