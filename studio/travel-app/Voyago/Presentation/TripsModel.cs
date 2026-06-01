namespace Voyago.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record TripsModel
{
    public IReadOnlyList<TripItem> UpcomingTrips { get; } = new[]
    {
        new TripItem("tr-001", "Santorini", "Greece",
            "https://picsum.photos/seed/santorini%20greece%20island/1280/720",
            new DateOnly(2026, 7, 12), new DateOnly(2026, 7, 22),
            "Confirmed", "VYG-48291"),
        new TripItem("tr-002", "Bali", "Indonesia",
            "https://picsum.photos/seed/bali%20rice%20terraces%20nature/1280/720",
            new DateOnly(2026, 9, 5), new DateOnly(2026, 9, 18),
            "Confirmed", "VYG-50134"),
        new TripItem("tr-003", "Machu Picchu", "Peru",
            "https://picsum.photos/seed/machu%20picchu%20peru%20ancient%20ruins/1280/720",
            new DateOnly(2026, 11, 3), new DateOnly(2026, 11, 13),
            "Pending", "VYG-52067"),
    };

    public IReadOnlyList<TripItem> PastTrips { get; } = new[]
    {
        new TripItem("tr-004", "Paris", "France",
            "https://picsum.photos/seed/paris%20eiffel%20tower/1280/720",
            new DateOnly(2025, 10, 14), new DateOnly(2025, 10, 21),
            "Completed", "VYG-39812"),
        new TripItem("tr-005", "Dolomites", "Italy",
            "https://picsum.photos/seed/mountain%20landscape%20travel/1280/720",
            new DateOnly(2025, 6, 1), new DateOnly(2025, 6, 9),
            "Completed", "VYG-36450"),
        new TripItem("tr-006", "Tokyo", "Japan",
            "https://picsum.photos/seed/tokyo%20japan%20cityscape%20night/1280/720",
            new DateOnly(2024, 12, 20), new DateOnly(2025, 1, 4),
            "Completed", "VYG-31199"),
    };
}
