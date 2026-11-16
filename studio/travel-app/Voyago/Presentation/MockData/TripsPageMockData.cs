namespace Voyago.Presentation.MockData;

/// <summary>
/// Design-time DataContext for <see cref="TripsPage"/> in Hot Design / Studio, built to the recipe
/// documented on <see cref="HomePageMockData"/>. The page draws two lists — upcoming and past — each
/// through its own FeedView, so both stay list feeds.
/// </summary>
[ReactiveBindable]
public partial record TripsPageMockData
{
    // Declared first (rule 3): these two bookings exist only as design-time data.
    private static readonly TripItem Santorini = new("tr-001", "Santorini", "Greece",
        "https://images.pexels.com/photos/1010657/pexels-photo-1010657.jpeg?auto=compress&cs=tinysrgb&w=1200",
        new DateOnly(2026, 7, 12), new DateOnly(2026, 7, 22), "Confirmed", "VYG-48291");

    private static readonly TripItem Paris = new("tr-004", "Paris", "France",
        "https://images.pexels.com/photos/532826/pexels-photo-532826.jpeg?auto=compress&cs=tinysrgb&w=1200",
        new DateOnly(2025, 10, 14), new DateOnly(2025, 10, 21), "Completed", "VYG-39812");

    // Default: a traveller with trips booked and history behind them.
    public static TripsPageMockDataViewModel Data => new();

    // A brand-new traveller: both FeedViews on their empty state. The running app cannot reach this
    // against the in-memory service, which is exactly why it needs a preview.
    public static TripsPageMockDataViewModel FirstTrip =>
        TripsPageMockDataViewModel.ForModel(new()
        {
            UpcomingTrips = ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<TripItem>>(
                ImmutableList<TripItem>.Empty)),
            PastTrips = ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<TripItem>>(
                ImmutableList<TripItem>.Empty)),
        });

    public IListFeed<TripItem> UpcomingTrips { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<TripItem>>(
            ImmutableList.Create(Santorini)));

    public IListFeed<TripItem> PastTrips { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<TripItem>>(
            ImmutableList.Create(Paris)));
}

// Reaches the generated ViewModel's protected model-taking constructor, so FirstTrip can wrap a
// customized model. See HomePageMockDataViewModel for the full explanation.
public partial class TripsPageMockDataViewModel
{
    internal static TripsPageMockDataViewModel ForModel(TripsPageMockData model) => new(model);
}
