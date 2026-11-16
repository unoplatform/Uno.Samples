namespace MovieStreamApp.Presentation;

/// <summary>
/// Backs <see cref="ProfilePage"/>. Reactive so the watchlist count reflects the shared
/// <see cref="WatchlistService"/> live; the rest is static profile data. Settings rows carry only
/// their display values (label + subtitle) — the leading icon is chosen in XAML from the label,
/// never stored as a glyph (lessons 11, 28).
/// </summary>
public partial record ProfileModel(WatchlistService Watchlist)
{
    public string UserName => "Jordan Mercer";
    public string UserEmail => "jordan.mercer@email.com";
    public string AvatarUrl => MovieData.UserAvatar;
    public string MemberSince => "Member since Jan 2022";
    public string SubscriptionTier => "Premium 4K";
    public string SubscriptionRenewal => "Renews Dec 14, 2025";

    public IFeed<int> WatchlistCount => Watchlist.Movies.AsFeed().Select(list => list.Count);
    public int WatchedCount => 148;
    public int ReviewsCount => 31;

    public IReadOnlyList<SettingsGroup> SettingGroups => new[]
    {
        new SettingsGroup("Playback", new[]
        {
            new SettingsItem("Video Quality", "4K Ultra HD"),
            new SettingsItem("Audio Language", "English"),
            new SettingsItem("Subtitles", "English (CC)"),
            new SettingsItem("Autoplay Next Episode", "On"),
        }),
        new SettingsGroup("Downloads", new[]
        {
            new SettingsItem("Download Quality", "High"),
            new SettingsItem("Storage Location", "Internal (12.4 GB free)"),
            new SettingsItem("Download Over Wi-Fi Only", "On"),
        }),
        new SettingsGroup("Account", new[]
        {
            new SettingsItem("Manage Subscription", "Premium 4K"),
            new SettingsItem("Edit Profile", ""),
            new SettingsItem("Privacy Settings", ""),
            new SettingsItem("Notifications", "Enabled"),
        }),
        new SettingsGroup("Support", new[]
        {
            new SettingsItem("Help Center", ""),
            new SettingsItem("Send Feedback", ""),
            new SettingsItem("About CineStream", "v4.2.1"),
        }),
    };
}

public partial record SettingsGroup(
    string Title,
    IReadOnlyList<SettingsItem> Items);

public partial record SettingsItem(
    string Label,
    string Subtitle);
