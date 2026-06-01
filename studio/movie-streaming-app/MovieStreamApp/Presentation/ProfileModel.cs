namespace MovieStreamApp.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record ProfileModel
{
    public string UserName { get; } = "Jordan Mercer";
    public string UserEmail { get; } = "jordan.mercer@email.com";
    public string AvatarUrl { get; } = "https://picsum.photos/seed/user%20profile%20avatar%20portrait%20photo/1024/1024";
    public string MemberSince { get; } = "Member since Jan 2022";
    public string SubscriptionTier { get; } = "Premium 4K";
    public string SubscriptionRenewal { get; } = "Renews Dec 14, 2025";
    public int WatchlistCount { get; } = 23;
    public int WatchedCount { get; } = 148;
    public int ReviewsCount { get; } = 31;

    public IReadOnlyList<SettingsGroup> SettingGroups { get; } = new[]
    {
        new SettingsGroup("Playback", new[]
        {
            new SettingsItem("\uE714", "Video Quality", "4K Ultra HD", SettingsAction.Toggle, true),
            new SettingsItem("\uE767", "Audio Language", "English", SettingsAction.Navigate, false),
            new SettingsItem("\uE8BD", "Subtitles", "English (CC)", SettingsAction.Navigate, false),
            new SettingsItem("\uE81C", "Autoplay Next Episode", "On", SettingsAction.Toggle, true),
        }),
        new SettingsGroup("Downloads", new[]
        {
            new SettingsItem("\uE896", "Download Quality", "High", SettingsAction.Navigate, false),
            new SettingsItem("\uE8B7", "Storage Location", "Internal (12.4 GB free)", SettingsAction.Navigate, false),
            new SettingsItem("\uE704", "Download Over Wi-Fi Only", "On", SettingsAction.Toggle, true),
        }),
        new SettingsGroup("Account", new[]
        {
            new SettingsItem("\uEB51", "Manage Subscription", "Premium 4K", SettingsAction.Navigate, false),
            new SettingsItem("\uE77B", "Edit Profile", "", SettingsAction.Navigate, false),
            new SettingsItem("\uE8D4", "Privacy Settings", "", SettingsAction.Navigate, false),
            new SettingsItem("\uEA8F", "Notifications", "Enabled", SettingsAction.Toggle, true),
        }),
        new SettingsGroup("Support", new[]
        {
            new SettingsItem("\uE897", "Help Center", "", SettingsAction.Navigate, false),
            new SettingsItem("\uE90A", "Send Feedback", "", SettingsAction.Navigate, false),
            new SettingsItem("\uE946", "About CineStream", "v4.2.1", SettingsAction.Navigate, false),
        }),
    };
}

public enum SettingsAction { Navigate, Toggle }

public partial record SettingsGroup(
    string Title,
    IReadOnlyList<SettingsItem> Items);

public partial record SettingsItem(
    string IconGlyph,
    string Label,
    string Subtitle,
    SettingsAction Action,
    bool IsEnabled);
