using System.Threading.Tasks;
using Uno.Extensions.Reactive;

namespace Voyago.Presentation;

// Bound to a single destination via DataViewMap<DestinationDetailPage, DestinationDetailModel, Destination>:
// Navigation passes the tapped Destination as the record's parameter, so each card opens its own detail.
public partial record DestinationDetailModel(Destination Destination)
{
    public string Name => Destination.Name;
    public string Country => Destination.Country;
    public string Tagline => Destination.Tagline;
    public string ImageUrl => Destination.ImageUrl;
    public string PriceFrom => Destination.PriceFrom;
    public double Rating => Destination.Rating;
    public string ReviewsText => $"{Destination.ReviewCount:N0} reviews";

    // Whether the trip has been booked. MVUX state; the Book command flips it and the page swaps
    // the "Book this trip" CTA for a confirmation.
    public IState<bool> IsBooked => State.Value(this, () => false);

    public async ValueTask Book() => await IsBooked.UpdateAsync(_ => true);
}
