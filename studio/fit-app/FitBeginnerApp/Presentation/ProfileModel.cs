namespace FitBeginnerApp.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record ProfileModel
{
    public UserProfile Profile { get; } = new(
        "Alex Rivera",
        "Beginner",
        "Build a healthy habit",
        3,
        5,
        12);

    public string AvatarInitials { get; } = "AR";
    public string JoinedDate { get; } = "Joined April 2026";
    public int Age { get; } = 28;
    public string PreferredTime { get; } = "Morning";
    public string EquipmentAvailable { get; } = "No equipment";
    public int SessionLengthMinutes { get; } = 20;

    public IReadOnlyList<SettingRow> Settings { get; } = new[]
    {
        new SettingRow("Workout Reminders", "Daily at 7:00 AM"),
        new SettingRow("Rest Day Alerts", "Notify me on over-training"),
        new SettingRow("Weekly Summary", "Every Sunday evening"),
        new SettingRow("Language", "English"),
        new SettingRow("Help & FAQ", "Tips for beginners"),
    };

    public IReadOnlyList<FitnessGoalItem> Goals { get; } = new[]
    {
        new FitnessGoalItem("Build a healthy habit", true),
        new FitnessGoalItem("Lose weight gradually", false),
        new FitnessGoalItem("Improve flexibility", false),
        new FitnessGoalItem("Increase stamina", false),
    };
}

public partial record SettingRow(string Label, string Value);
public partial record FitnessGoalItem(string Label, bool IsSelected);
