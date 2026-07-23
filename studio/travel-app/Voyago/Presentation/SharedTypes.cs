using System.Globalization;

namespace Voyago.Presentation;

// Destination record — used on HomePage (hero cards, recommended trips) and FavoritesPage
public partial record Destination(
    string Id,
    string Name,
    string Country,
    string Tagline,
    string ImageUrl,
    string PriceFrom,
    double Rating,
    int ReviewCount);

// TripItem record — used on TripsPage and HomePage (upcoming trips summary)
public partial record TripItem(
    string Id,
    string Destination,
    string Country,
    string ImageUrl,
    DateOnly DepartureDate,
    DateOnly ReturnDate,
    string Status,
    string BookingRef)
{
    // Pre-formatted for display (a raw DateOnly binds as a culture-dependent machine date).
    public string DepartureLabel => DepartureDate.ToString("d MMM yyyy", CultureInfo.InvariantCulture);
}

// ExploreCategory record — used on HomePage and SearchPage. Featured is the representative
// destination the tile opens when tapped (see Catalog.Categories).
public partial record ExploreCategory(
    string Id,
    string Label,
    string ImageUrl,
    string Description,
    Destination Featured);
