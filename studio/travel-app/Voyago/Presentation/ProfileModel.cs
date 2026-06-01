namespace Voyago.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record ProfileModel
{
    public string FullName { get; } = "Alex Jordan";
    public string Email { get; } = "alex.jordan@voyago.com";
    public string UserInitials { get; } = "AJ";
    public string MemberSince { get; } = "Member since 2022";
    public string MemberTier { get; } = "Gold Explorer";

    // Travel stats
    public int TripsCompleted { get; } = 14;
    public int CountriesVisited { get; } = 11;
    public int SavedDestinations { get; } = 7;
    public int ReviewsWritten { get; } = 23;

    // Settings sections
    public IReadOnlyList<ProfileSettingItem> AccountSettings { get; } = new[]
    {
        new ProfileSettingItem("ps-01", "Personal Information", "\uE77B", "Update your details"),
        new ProfileSettingItem("ps-02", "Payment Methods", "\uE8F0", "Manage cards and billing"),
        new ProfileSettingItem("ps-03", "Notifications", "\uEA8F", "Alerts and preferences"),
        new ProfileSettingItem("ps-04", "Privacy & Security", "\uE72E", "Account security settings"),
    };

    public IReadOnlyList<ProfileSettingItem> AppSettings { get; } = new[]
    {
        new ProfileSettingItem("ps-05", "Language", "\uE8C1", "English (UK)"),
        new ProfileSettingItem("ps-06", "Currency", "\uE8D0", "EUR — Euro"),
        new ProfileSettingItem("ps-07", "Help & Support", "\uE897", "FAQs and contact us"),
        new ProfileSettingItem("ps-08", "About Voyago", "\uE946", "Version 2.4.1"),
    };
}

// Page-local record — only used on ProfilePage
public partial record ProfileSettingItem(string Id, string Label, string Glyph, string Subtitle);
