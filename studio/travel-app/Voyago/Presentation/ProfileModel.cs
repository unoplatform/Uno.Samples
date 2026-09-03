using Voyago.Presentation.Services;

namespace Voyago.Presentation;

// The traveller's identity, stats and settings, read from IDiscoveryService.
public partial record ProfileModel(IDiscoveryService Discovery)
{
    private IFeed<TravellerProfile>? _profile;
    private IFeed<TravellerProfile> Profile => _profile ??= Feed.Async(Discovery.GetProfileAsync);

    public IFeed<string> FullName => Profile.Select(p => p.FullName);
    public IFeed<string> Email => Profile.Select(p => p.Email);
    public IFeed<string> UserInitials => Profile.Select(p => p.UserInitials);
    public IFeed<string> MemberSince => Profile.Select(p => p.MemberSince);
    public IFeed<string> MemberTier => Profile.Select(p => p.MemberTier);

    public IFeed<int> TripsCompleted => Profile.Select(p => p.TripsCompleted);
    public IFeed<int> CountriesVisited => Profile.Select(p => p.CountriesVisited);
    public IFeed<int> SavedDestinations => Profile.Select(p => p.SavedDestinations);
    public IFeed<int> ReviewsWritten => Profile.Select(p => p.ReviewsWritten);

    // Both settings lists bind directly rather than through a FeedView: they are fixed-shape menu
    // sections inside a card, and neither "no settings" nor "settings failed" is a state with UI
    // worth designing.
    private IListFeed<ProfileSettingItem>? _account;
    public IListFeed<ProfileSettingItem> AccountSettings =>
        _account ??= ListFeed.Async(Discovery.GetAccountSettingsAsync);

    private IListFeed<ProfileSettingItem>? _app;
    public IListFeed<ProfileSettingItem> AppSettings =>
        _app ??= ListFeed.Async(Discovery.GetAppSettingsAsync);
}

// Page-local record — only used on ProfilePage
public partial record ProfileSettingItem(string Id, string Label, string Subtitle);
