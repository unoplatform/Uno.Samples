namespace Voyago.Presentation.MockData;

/// <summary>
/// Design-time DataContext for <see cref="ProfilePage"/> in Hot Design / Studio, built to the recipe
/// documented on <see cref="HomePageMockData"/>. The traveller's details and four stat tiles are plain
/// materialized values; the two settings lists stay list feeds because FeedViews render them.
/// </summary>
[ReactiveBindable]
public partial record ProfilePageMockData
{
    // Declared first (rule 3): both settings lists are chrome, defined nowhere else in the seed data.
    private static readonly IImmutableList<ProfileSettingItem> AccountSettingSeed =
    [
        new("ps-01", "Personal Information", "Update your details"),
        new("ps-02", "Payment Methods", "Manage cards and billing"),
        new("ps-03", "Notifications", "Alerts and preferences"),
        new("ps-04", "Privacy & Security", "Account security settings"),
    ];

    private static readonly IImmutableList<ProfileSettingItem> AppSettingSeed =
    [
        new("ps-05", "Language", "English (UK)"),
        new("ps-06", "Currency", "EUR — Euro"),
        new("ps-07", "Help & Support", "FAQs and contact us"),
        new("ps-08", "About Voyago", "Version 2.4.1"),
    ];

    /// <summary>A long-standing Gold Explorer, with both settings lists populated.</summary>
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

    public IListFeed<ProfileSettingItem> AccountSettings { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult(AccountSettingSeed));

    public IListFeed<ProfileSettingItem> AppSettings { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult(AppSettingSeed));
}

// Reaches the generated ViewModel's protected model-taking constructor for a customized model. See
// HomePageMockDataViewModel for the full explanation.
public partial class ProfilePageMockDataViewModel
{
    internal static ProfilePageMockDataViewModel ForModel(ProfilePageMockData model) => new(model);
}
