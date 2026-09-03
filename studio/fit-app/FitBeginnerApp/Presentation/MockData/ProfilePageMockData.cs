namespace FitBeginnerApp.Presentation.MockData;

// Design-time DataContext for ProfilePage. ProfilePage has no FeedView — its goals and settings
// lists bind directly — but its Model is reactive now that the profile is read over the service, so
// the mock still mirrors the Model's shape through the generated ViewModel.
[Uno.Extensions.Reactive.ReactiveBindable]
public partial record ProfilePageMockData
{
    // Default design-time state: an established member.
    public static ProfilePageMockDataViewModel Data => new();

    // A brand-new member: zeroed hero stats and no goal chosen, which is the only way to see the
    // goals list with no "Active" pill on it.
    public static ProfilePageMockDataViewModel NewMember =>
        ProfilePageMockDataViewModel.ForModel(new()
        {
            Profile = new UserProfile("Sam Okafor", "Beginner", "Get moving", 3, 0, 0),
            AvatarInitials = "SO",
            JoinedDate = "Joined today",
            Goals = ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<FitnessGoalItem>>(
            [
                new("Build a healthy habit", false),
                new("Lose weight gradually", false),
                new("Improve flexibility", false),
                new("Increase stamina", false),
            ])),
        });

    public UserProfile Profile { get; init; } = FitData.Profile;

    public string AvatarInitials { get; init; } = FitData.ProfileDetails.AvatarInitials;
    public string JoinedDate { get; init; } = FitData.ProfileDetails.JoinedDate;
    public string PreferredTime { get; init; } = FitData.ProfileDetails.PreferredTime;
    public string EquipmentAvailable { get; init; } = FitData.ProfileDetails.EquipmentAvailable;
    public int SessionLengthMinutes { get; init; } = FitData.ProfileDetails.SessionLengthMinutes;

    public IListFeed<FitnessGoalItem> Goals { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult(FitData.Goals));

    public IListFeed<SettingRow> Settings { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult(FitData.Settings));
}

public partial class ProfilePageMockDataViewModel
{
    internal static ProfilePageMockDataViewModel ForModel(ProfilePageMockData model) => new(model);
}
