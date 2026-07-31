using System.Threading;
using System.Threading.Tasks;
using Uno.Extensions.Reactive;
using Voyago.Presentation.Services;

namespace Voyago.Presentation;

// Bound to a single destination via DataViewMap<DestinationDetailPage, DestinationDetailModel, Destination>:
// navigation passes the tapped Destination as the record's parameter, so each card opens its own
// detail. ITripsService (a DI singleton) is the shared trip book the Book command writes into.
public partial record DestinationDetailModel(Destination Destination, ITripsService Trips)
{
    public string Name => Destination.Name;
    public string Country => Destination.Country;
    public string Tagline => Destination.Tagline;
    public string ImageUrl => Destination.ImageUrl;
    public string PriceFrom => Destination.PriceFrom;
    public double Rating => Destination.Rating;
    public string ReviewsText => $"{Destination.ReviewCount:N0} reviews";

    // Whether this destination is already an upcoming trip — derived from the shared trip book, so
    // it flips to "Booked" the moment Book() adds it (and shows "Booked" up front for a destination
    // that was already on the Trips tab). The page swaps the CTA for a confirmation on this.
    public IFeed<bool> IsBooked => Trips.Upcoming
        .AsFeed()
        .Select(trips => trips.Any(t => t.Destination == Destination.Name));

    // Add this destination to the shared upcoming-trips list; the Trips tab and the CTA update
    // reactively. Booking the same destination twice is a no-op (guarded in the service).
    public async ValueTask Book(CancellationToken ct)
        => await Trips.BookAsync(Destination, ct);
}
