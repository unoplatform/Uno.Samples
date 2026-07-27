namespace MovieStreamApp.Presentation;

/// <summary>
/// Backs <see cref="ProfilePage"/>. Reactive so the watchlist count reflects the shared
/// <see cref="WatchlistService"/> live; the rest is static profile data. Settings rows carry only
/// their domain values (label / subtitle / action) — the leading icon is chosen in XAML from the
/// label, never stored as a glyph (lessons 11, 28).
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
            new SettingsItem("Video Quality", "4K Ultra HD", SettingsAction.Toggle, true),
            new SettingsItem("Audio Language", "English", SettingsAction.Navigate, false),
            new SettingsItem("Subtitles", "English (CC)", SettingsAction.Navigate, false),
            new SettingsItem("Autoplay Next Episode", "On", SettingsAction.Toggle, true),
        }),
        new SettingsGroup("Downloads", new[]
        {
            new SettingsItem("Download Quality", "High", SettingsAction.Navigate, false),
            new SettingsItem("Storage Location", "Internal (12.4 GB free)", SettingsAction.Navigate, false),
            new SettingsItem("Download Over Wi-Fi Only", "On", SettingsAction.Toggle, true),
        }),
        new SettingsGroup("Account", new[]
        {
            new SettingsItem("Manage Subscription", "Premium 4K", SettingsAction.Navigate, false),
            new SettingsItem("Edit Profile", "", SettingsAction.Navigate, false),
            new SettingsItem("Privacy Settings", "", SettingsAction.Navigate, false),
            new SettingsItem("Notifications", "Enabled", SettingsAction.Toggle, true),
        }),
        new SettingsGroup("Support", new[]
        {
            new SettingsItem("Help Center", "", SettingsAction.Navigate, false),
            new SettingsItem("Send Feedback", "", SettingsAction.Navigate, false),
            new SettingsItem("About CineStream", "v4.2.1", SettingsAction.Navigate, false),
        }),
    };
}

public enum SettingsAction { Navigate, Toggle }

public partial record SettingsGroup(
    string Title,
    IReadOnlyList<SettingsItem> Items);

public partial record SettingsItem(
    string Label,
    string Subtitle,
    SettingsAction Action,
    bool IsEnabled);
