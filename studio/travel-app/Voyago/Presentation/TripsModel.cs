namespace Voyago.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record TripsModel
{
    public IReadOnlyList<TripItem> UpcomingTrips { get; } = new[]
    {
        new TripItem("tr-001", "Santorini", "Greece",
            "ms-appx:///Assets/Photos/santorini.jpg",
            new DateOnly(2026, 7, 12), new DateOnly(2026, 7, 22),
            "Confirmed", "VYG-48291"),
        new TripItem("tr-002", "Bali", "Indonesia",
            "ms-appx:///Assets/Photos/bali.jpg",
            new DateOnly(2026, 9, 5), new DateOnly(2026, 9, 18),
            "Confirmed", "VYG-50134"),
        new TripItem("tr-003", "Machu Picchu", "Peru",
            "ms-appx:///Assets/Photos/machupicchu.jpg",
            new DateOnly(2026, 11, 3), new DateOnly(2026, 11, 13),
            "Pending", "VYG-52067"),
    };

    public IReadOnlyList<TripItem> PastTrips { get; } = new[]
    {
        new TripItem("tr-004", "Paris", "France",
            "ms-appx:///Assets/Photos/paris.jpg",
            new DateOnly(2025, 10, 14), new DateOnly(2025, 10, 21),
            "Completed", "VYG-39812"),
        new TripItem("tr-005", "Dolomites", "Italy",
            "ms-appx:///Assets/Photos/dolomites.jpg",
            new DateOnly(2025, 6, 1), new DateOnly(2025, 6, 9),
            "Completed", "VYG-36450"),
        new TripItem("tr-006", "Tokyo", "Japan",
            "ms-appx:///Assets/Photos/tokyo.jpg",
            new DateOnly(2024, 12, 20), new DateOnly(2025, 1, 4),
            "Completed", "VYG-31199"),
    };
}
