using Uno.Extensions.Reactive;
using Voyago.Presentation.Services;

namespace Voyago.Presentation;

// Reads the one shared trip book. A trip booked from a destination detail page (via ITripsService)
// is prepended to Upcoming and shows up here live — no per-page copy, no messaging.
public partial record TripsModel(ITripsService Trips)
{
    public IListState<TripItem> UpcomingTrips => Trips.Upcoming;

    public IReadOnlyList<TripItem> PastTrips => Trips.Past;
}
