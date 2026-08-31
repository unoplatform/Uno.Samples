namespace Voyago.Presentation.Services;

// Everything Voyago reads rather than owns: the destination catalogue, the explore categories, the
// traveller's saved list and profile. Every member is asynchronous and cancellable, because that is
// the shape a real endpoint has — which is what lets the Models expose IListFeed<T>/IFeed<T> and the
// pages render results, empty and failed states through a FeedView instead of hand-rolling them.
//
// Search is a SERVICE call taking the query, not a client-side filter over a local copy, the same way
// a real catalogue API works.
public interface IDiscoveryService
{
    ValueTask<HomeGreeting> GetGreetingAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<Destination>> GetHeroDestinationsAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<QuickAction>> GetQuickActionsAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<Destination>> GetRecommendedAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<ExploreCategory>> GetCategoriesAsync(CancellationToken ct = default);

    ValueTask<IImmutableList<string>> GetPopularSearchesAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<Destination>> SearchDestinationsAsync(string? query, CancellationToken ct = default);

    ValueTask<IImmutableList<Destination>> GetSavedDestinationsAsync(CancellationToken ct = default);

    ValueTask<TravellerProfile> GetProfileAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<ProfileSettingItem>> GetAccountSettingsAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<ProfileSettingItem>> GetAppSettingsAsync(CancellationToken ct = default);
}

// The in-memory implementation, standing in for an HTTP endpoint. Replacing this with a real client
// is the only change a live backend needs: the interface, the Models and every page stay as they are.
//
// The short delay is deliberate and load-bearing, not padding — without any latency a feed resolves on
// the first frame and a FeedView's ProgressTemplate would never be seen.
public sealed class DiscoveryService : IDiscoveryService
{
    private static readonly TimeSpan Latency = TimeSpan.FromMilliseconds(300);

    private static async ValueTask<T> Fetch<T>(T value, CancellationToken ct)
    {
        await Task.Delay(Latency, ct);
        return value;
    }

    public ValueTask<HomeGreeting> GetGreetingAsync(CancellationToken ct = default)
        => Fetch(new HomeGreeting("Where do you want to explore today?", "AJ"), ct);

    public ValueTask<IImmutableList<Destination>> GetHeroDestinationsAsync(CancellationToken ct = default)
        => Fetch<IImmutableList<Destination>>(
            [Catalog.Dolomites, Catalog.Maldives, Catalog.Kyoto], ct);

    public ValueTask<IImmutableList<QuickAction>> GetQuickActionsAsync(CancellationToken ct = default)
        => Fetch<IImmutableList<QuickAction>>(
        [
            new("qa-01", "Flights"), new("qa-02", "Hotels"), new("qa-03", "Experiences"),
            new("qa-04", "Cars"), new("qa-05", "Trips"), new("qa-06", "Map"),
        ], ct);

    public ValueTask<IImmutableList<Destination>> GetRecommendedAsync(CancellationToken ct = default)
        => Fetch<IImmutableList<Destination>>(
            [Catalog.Santorini, Catalog.Bali, Catalog.Paris, Catalog.MachuPicchu], ct);

    public ValueTask<IImmutableList<ExploreCategory>> GetCategoriesAsync(CancellationToken ct = default)
        => Fetch(Catalog.Categories.ToImmutableList() as IImmutableList<ExploreCategory>, ct);

    public ValueTask<IImmutableList<string>> GetPopularSearchesAsync(CancellationToken ct = default)
        => Fetch<IImmutableList<string>>(
            ["Beach holidays", "City breaks", "Safari", "Ski resorts", "Island hopping"], ct);

    // The catalogue search. An empty query returns the popular set, which is what the page shows
    // before the traveller types anything; a query that matches nothing returns an empty list, and
    // the page's FeedView renders that as its NoneTemplate.
    public async ValueTask<IImmutableList<Destination>> SearchDestinationsAsync(
        string? query,
        CancellationToken ct = default)
    {
        await Task.Delay(Latency, ct);

        var term = (query ?? string.Empty).Trim();
        if (term.Length == 0)
        {
            return [Catalog.Dolomites, Catalog.Santorini, Catalog.Maldives,
                    Catalog.Bali, Catalog.MachuPicchu, Catalog.Paris];
        }

        return Catalog.All
            .Where(d => d.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                        || d.Country.Contains(term, StringComparison.OrdinalIgnoreCase)
                        || d.Tagline.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToImmutableList();
    }

    public ValueTask<IImmutableList<Destination>> GetSavedDestinationsAsync(CancellationToken ct = default)
        => Fetch<IImmutableList<Destination>>(
        [
            Catalog.Santorini, Catalog.Dolomites, Catalog.Maldives, Catalog.MachuPicchu,
            Catalog.Kyoto, Catalog.Bali, Catalog.Paris,
        ], ct);

    public ValueTask<TravellerProfile> GetProfileAsync(CancellationToken ct = default)
        => Fetch(new TravellerProfile(
            "Alex Jordan", "alex.jordan@voyago.com", "AJ", "Member since 2022", "Gold Explorer",
            TripsCompleted: 14, CountriesVisited: 11, SavedDestinations: 7, ReviewsWritten: 23), ct);

    public ValueTask<IImmutableList<ProfileSettingItem>> GetAccountSettingsAsync(CancellationToken ct = default)
        => Fetch<IImmutableList<ProfileSettingItem>>(
        [
            new("ps-01", "Personal Information", "Update your details"),
            new("ps-02", "Payment Methods", "Manage cards and billing"),
            new("ps-03", "Notifications", "Alerts and preferences"),
            new("ps-04", "Privacy & Security", "Account security settings"),
        ], ct);

    public ValueTask<IImmutableList<ProfileSettingItem>> GetAppSettingsAsync(CancellationToken ct = default)
        => Fetch<IImmutableList<ProfileSettingItem>>(
        [
            new("ps-05", "Language", "English (UK)"),
            new("ps-06", "Currency", "EUR — Euro"),
            new("ps-07", "Help & Support", "FAQs and contact us"),
            new("ps-08", "About Voyago", "Version 2.4.1"),
        ], ct);
}
