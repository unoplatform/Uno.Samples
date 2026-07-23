using Uno.Extensions.Reactive;

namespace Voyago.Presentation.Services;

// One shared trip book for the whole app. Registered as a DI singleton and injected into both
// TripsModel (which lists it) and DestinationDetailModel (which books into it), so a "Book this
// trip" tap on a detail page shows up live on the Trips tab — no messaging or manual fan-out.
public interface ITripsService
{
    // The shared, mutable upcoming-trips list. A freshly booked trip is prepended here.
    IListState<TripItem> Upcoming { get; }

    // Past trips are immutable history.
    IReadOnlyList<TripItem> Past { get; }

    ValueTask BookAsync(Destination destination, CancellationToken ct = default);
}

public sealed class TripsService : ITripsService
{
    // Once-initialized in the ctor (NOT an expression-bodied getter, which would rebuild a fresh
    // state on every access and break sharing). As a singleton the service is the reactive owner
    // and lives for the app's lifetime.
    public IListState<TripItem> Upcoming { get; }

    public IReadOnlyList<TripItem> Past { get; } = new[]
    {
        new TripItem("tr-004", "Paris", "France",
            "https://images.pexels.com/photos/532826/pexels-photo-532826.jpeg?auto=compress&cs=tinysrgb&w=1200",
            new DateOnly(2025, 10, 14), new DateOnly(2025, 10, 21), "Completed", "VYG-39812"),
        new TripItem("tr-005", "Dolomites", "Italy",
            "https://images.pexels.com/photos/28491959/pexels-photo-28491959.jpeg?auto=compress&cs=tinysrgb&w=1200",
            new DateOnly(2025, 6, 1), new DateOnly(2025, 6, 9), "Completed", "VYG-36450"),
        new TripItem("tr-006", "Tokyo", "Japan",
            "https://images.pexels.com/photos/29662430/pexels-photo-29662430.jpeg?auto=compress&cs=tinysrgb&w=1200",
            new DateOnly(2024, 12, 20), new DateOnly(2025, 1, 4), "Completed", "VYG-31199"),
    };

    private static readonly IImmutableList<TripItem> SeedUpcoming = ImmutableList.Create(
        new TripItem("tr-001", "Santorini", "Greece",
            "https://images.pexels.com/photos/1010657/pexels-photo-1010657.jpeg?auto=compress&cs=tinysrgb&w=1200",
            new DateOnly(2026, 7, 12), new DateOnly(2026, 7, 22), "Confirmed", "VYG-48291"),
        new TripItem("tr-002", "Bali", "Indonesia",
            "https://images.pexels.com/photos/5933066/pexels-photo-5933066.jpeg?auto=compress&cs=tinysrgb&w=1200",
            new DateOnly(2026, 9, 5), new DateOnly(2026, 9, 18), "Confirmed", "VYG-50134"),
        new TripItem("tr-003", "Machu Picchu", "Peru",
            "https://images.pexels.com/photos/2929906/pexels-photo-2929906.jpeg?auto=compress&cs=tinysrgb&w=1200",
            new DateOnly(2026, 11, 3), new DateOnly(2026, 11, 13), "Pending", "VYG-52067"));

    public TripsService()
    {
        Upcoming = ListState.Value(this, () => SeedUpcoming);
    }

    // Book a destination: prepend a new "Pending" upcoming trip. If this destination is already in
    // the upcoming list (matched by name), do nothing — a second tap shouldn't duplicate it.
    public async ValueTask BookAsync(Destination destination, CancellationToken ct = default)
    {
        var current = await Upcoming.Value(ct);
        if (current is not null && current.Any(t => t.Destination == destination.Name))
        {
            return;
        }

        var departure = DateOnly.FromDateTime(DateTime.Now).AddDays(45);
        var bookingRef = $"VYG-{60000 + (current?.Count ?? 0) * 173 + destination.Name.Length:D5}";
        var trip = new TripItem(
            $"tr-{destination.Id}", destination.Name, destination.Country, destination.ImageUrl,
            departure, departure.AddDays(9), "Pending", bookingRef);

        await Upcoming.InsertAsync(trip, ct);
    }
}
