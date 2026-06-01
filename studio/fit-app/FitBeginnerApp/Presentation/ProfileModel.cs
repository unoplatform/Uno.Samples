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
        new SettingRow("Workout Reminders", "Daily at 7:00 AM", "\uEA8F"),
        new SettingRow("Rest Day Alerts", "Notify me on over-training", "\uE946"),
        new SettingRow("Weekly Summary", "Every Sunday evening", "\uE787"),
        new SettingRow("Language", "English", "\uE713"),
        new SettingRow("Help & FAQ", "Tips for beginners", "\uE897"),
    };

    public IReadOnlyList<FitnessGoalItem> Goals { get; } = new[]
    {
        new FitnessGoalItem("Build a healthy habit", true, "\uEB51"),
        new FitnessGoalItem("Lose weight gradually", false, "\uE945"),
        new FitnessGoalItem("Improve flexibility", false, "\uE9D9"),
        new FitnessGoalItem("Increase stamina", false, "\uE76E"),
    };
}

public partial record SettingRow(string Label, string Value, string IconGlyph);
public partial record FitnessGoalItem(string Label, bool IsSelected, string IconGlyph);