namespace Voyago.Presentation.MockData;

// Design-time DataContexts for Hot Design / Studio, one per page.
//
// Every page here renders at least one FeedView, and a FeedView can only subscribe to a FEED — a
// plain list would never reach it and the preview would sit on the empty state. So each mock is
// [ReactiveBindable] and its statics return the GENERATED ViewModel, whose constructor creates the
// SourceContext that makes the feeds pump.
//
// Two rules these all follow:
//  * statics are expression-bodied (a fresh instance per access), never a cached singleton — a
//    generated ViewModel has a view-scoped lifecycle, so a shared instance can be built before Hot
//    Design's dispatcher is ready, or be dead from a previous render, leaving feeds that never emit;
//  * every static input is declared ABOVE the statics that construct instances, or initialized
//    inline, because static members initialize in textual order and an instance initializer reading
//    a not-yet-assigned field gets null with no exception at all.
//
// None of these is ever seeded from a page constructor — see those constructors for why.

internal static class MockFeeds
{
    public static IListFeed<T> Of<T>(params T[] items) =>
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<T>>(items.ToImmutableList()));

    public static IListFeed<T> Empty<T>() =>
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<T>>(ImmutableList<T>.Empty));
}

[Uno.Extensions.Reactive.ReactiveBindable]
public partial record HomePageMockData
{
    public static HomePageMockDataViewModel Data => new();

    public string GreetingText { get; init; } = "Where do you want to explore today?";
    public string UserInitials { get; init; } = "AJ";

    public IListFeed<Destination> HeroDestinations { get; init; } =
        MockFeeds.Of(Catalog.Dolomites, Catalog.Maldives, Catalog.Kyoto);

    public int HeroCount { get; init; } = 3;

    public IListFeed<QuickAction> QuickActions { get; init; } = MockFeeds.Of(
        new QuickAction("qa-01", "Flights"), new QuickAction("qa-02", "Hotels"),
        new QuickAction("qa-03", "Experiences"), new QuickAction("qa-04", "Cars"),
        new QuickAction("qa-05", "Trips"), new QuickAction("qa-06", "Map"));

    public IListFeed<Destination> RecommendedTrips { get; init; } =
        MockFeeds.Of(Catalog.Santorini, Catalog.Bali, Catalog.Paris, Catalog.MachuPicchu);

    public IListFeed<ExploreCategory> ExploreCategories { get; init; } =
        MockFeeds.Of(Catalog.Categories.ToArray());
}

public partial class HomePageMockDataViewModel
{
    internal static HomePageMockDataViewModel ForModel(HomePageMockData model) => new(model);
}

[Uno.Extensions.Reactive.ReactiveBindable]
public partial record SearchPageMockData
{
    // Default: the empty query, which the service answers with the popular set.
    public static SearchPageMockDataViewModel Data => new();

    // A query that matches nothing, so the results FeedView falls through to its NoneTemplate.
    public static SearchPageMockDataViewModel NoResults =>
        SearchPageMockDataViewModel.ForModel(new()
        {
            Query = "atlantis",
            Results = MockFeeds.Empty<Destination>(),
        });

    public string SearchPlaceholder { get; init; } = "Search destinations, flights, hotels...";
    public string Query { get; set; } = string.Empty;

    public IListFeed<Destination> Results { get; init; } = MockFeeds.Of(
        Catalog.Dolomites, Catalog.Santorini, Catalog.Maldives,
        Catalog.Bali, Catalog.MachuPicchu, Catalog.Paris);

    public IListFeed<string> PopularSearches { get; init; } =
        MockFeeds.Of("Beach holidays", "City breaks", "Safari", "Ski resorts", "Island hopping");

    public IListFeed<ExploreCategory> ExploreCategories { get; init; } =
        MockFeeds.Of(Catalog.Categories.ToArray());

    public async ValueTask ApplySearch(string term, CancellationToken ct) => await ValueTask.CompletedTask;
}

public partial class SearchPageMockDataViewModel
{
    internal static SearchPageMockDataViewModel ForModel(SearchPageMockData model) => new(model);
}

[Uno.Extensions.Reactive.ReactiveBindable]
public partial record TripsPageMockData
{
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
            UpcomingTrips = MockFeeds.Empty<TripItem>(),
            PastTrips = MockFeeds.Empty<TripItem>(),
        });

    public IListFeed<TripItem> UpcomingTrips { get; init; } = MockFeeds.Of(Santorini);
    public IListFeed<TripItem> PastTrips { get; init; } = MockFeeds.Of(Paris);
}

public partial class TripsPageMockDataViewModel
{
    internal static TripsPageMockDataViewModel ForModel(TripsPageMockData model) => new(model);
}

[Uno.Extensions.Reactive.ReactiveBindable]
public partial record FavoritesPageMockData
{
    public static FavoritesPageMockDataViewModel Data => new();

    // Nothing saved — the grid's NoneTemplate, and the header count reading 0 rather than blank
    // (which is what the Model's SelectData projection is for).
    public static FavoritesPageMockDataViewModel Empty =>
        FavoritesPageMockDataViewModel.ForModel(new()
        {
            SavedDestinations = MockFeeds.Empty<Destination>(),
            TotalFavorites = 0,
        });

    public IListFeed<Destination> SavedDestinations { get; init; } = MockFeeds.Of(
        Catalog.Santorini, Catalog.Dolomites, Catalog.Maldives, Catalog.MachuPicchu,
        Catalog.Kyoto, Catalog.Bali, Catalog.Paris);

    public int TotalFavorites { get; init; } = 7;
}

public partial class FavoritesPageMockDataViewModel
{
    internal static FavoritesPageMockDataViewModel ForModel(FavoritesPageMockData model) => new(model);
}

[Uno.Extensions.Reactive.ReactiveBindable]
public partial record ProfilePageMockData
{
    public static ProfilePageMockDataViewModel Data => new();

    public string FullName { get; init; } = "Alex Jordan";
    public string Email { get; init; } = "alex.jordan@voyago.com";
    public string UserInitials { get; init; } = "AJ";
    public string MemberSince { get; init; } = "Member since 2022";
    public string MemberTier { get; init; } = "Gold Explorer";

    public int TripsCompleted { get; init; } = 14;
    public int CountriesVisited { get; init; } = 11;
    public int SavedDestinations { get; init; } = 7;
    public int ReviewsWritten { get; init; } = 23;

    public IListFeed<ProfileSettingItem> AccountSettings { get; init; } = MockFeeds.Of(
        new ProfileSettingItem("ps-01", "Personal Information", "Update your details"),
        new ProfileSettingItem("ps-02", "Payment Methods", "Manage cards and billing"),
        new ProfileSettingItem("ps-03", "Notifications", "Alerts and preferences"),
        new ProfileSettingItem("ps-04", "Privacy & Security", "Account security settings"));

    public IListFeed<ProfileSettingItem> AppSettings { get; init; } = MockFeeds.Of(
        new ProfileSettingItem("ps-05", "Language", "English (UK)"),
        new ProfileSettingItem("ps-06", "Currency", "EUR — Euro"),
        new ProfileSettingItem("ps-07", "Help & Support", "FAQs and contact us"),
        new ProfileSettingItem("ps-08", "About Voyago", "Version 2.4.1"));
}

public partial class ProfilePageMockDataViewModel
{
    internal static ProfilePageMockDataViewModel ForModel(ProfilePageMockData model) => new(model);
}

// The detail surfaces (page on mobile, ContentDialog on desktop) share one Model shape. IsBooked is
// a plain bool here: the live Model derives it from the shared trip book, which a design surface has
// no context to pump.
[Uno.Extensions.Reactive.ReactiveBindable]
public partial record DestinationDetailMockData
{
    public static DestinationDetailMockDataViewModel Data => new();

    public static DestinationDetailMockDataViewModel Booked =>
        DestinationDetailMockDataViewModel.ForModel(new() { IsBooked = true });

    public string Name { get; init; } = Catalog.Santorini.Name;
    public string Country { get; init; } = Catalog.Santorini.Country;
    public string Tagline { get; init; } = Catalog.Santorini.Tagline;
    public string ImageUrl { get; init; } = Catalog.Santorini.ImageUrl;
    public string PriceFrom { get; init; } = Catalog.Santorini.PriceFrom;
    public double Rating { get; init; } = Catalog.Santorini.Rating;
    public string ReviewsText { get; init; } = $"{Catalog.Santorini.ReviewCount:N0} reviews";

    public bool IsBooked { get; init; }

    // Mirrors the Model's signature, not a void stub: MVUX generates a command from an
    // `async ValueTask` method, so a `void Book()` here would leave the preview's "Book this trip"
    // CTA bound to nothing and rendered disabled.
    public async ValueTask Book(CancellationToken ct) => await ValueTask.CompletedTask;
}

public partial class DestinationDetailMockDataViewModel
{
    internal static DestinationDetailMockDataViewModel ForModel(DestinationDetailMockData model) => new(model);
}
