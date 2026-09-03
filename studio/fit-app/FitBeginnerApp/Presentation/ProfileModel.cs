namespace FitBeginnerApp.Presentation;

// The member's profile, preferences, goals and app settings, read from IFitnessService.
public partial record ProfileModel(IFitnessService Fitness)
{
    private IFeed<UserProfile>? _profile;
    public IFeed<UserProfile> Profile => _profile ??= Feed.Async(Fitness.GetProfileAsync);

    private IFeed<ProfileDetails>? _details;
    private IFeed<ProfileDetails> Details => _details ??= Feed.Async(Fitness.GetProfileDetailsAsync);

    public IFeed<string> AvatarInitials => Details.Select(d => d.AvatarInitials);
    public IFeed<string> JoinedDate => Details.Select(d => d.JoinedDate);
    public IFeed<string> PreferredTime => Details.Select(d => d.PreferredTime);
    public IFeed<string> EquipmentAvailable => Details.Select(d => d.EquipmentAvailable);
    public IFeed<int> SessionLengthMinutes => Details.Select(d => d.SessionLengthMinutes);

    // Both lists bind directly rather than through a FeedView. They are short, fixed-shape parts of
    // a card — a goals picker and an app-settings menu — and neither "no goals" nor "settings
    // failed" is a state with UI worth designing.
    private IListFeed<FitnessGoalItem>? _goals;
    public IListFeed<FitnessGoalItem> Goals => _goals ??= ListFeed.Async(Fitness.GetGoalsAsync);

    private IListFeed<SettingRow>? _settings;
    public IListFeed<SettingRow> Settings => _settings ??= ListFeed.Async(Fitness.GetSettingsAsync);
}

public partial record SettingRow(string Label, string Value);
public partial record FitnessGoalItem(string Label, bool IsSelected);
