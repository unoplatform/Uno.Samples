namespace FitBeginnerApp.Presentation.MockData;

// Design-time data for the Hot Design *component* previews — the individual entities each keyed
// DataTemplate renders. Referenced from the preview XAML via {x:Bind}, so no preview needs a
// LoadDataContext code-behind override.
//
// The page previews don't appear here: every page's ctor already seeds a real, fully-populated
// Model (the page Models are [ReactiveBindable(false)] projections of fixed data), so a page's
// auto-Default preview renders correctly with no mock at all. Only WorkoutSessionPage — whose
// IsStarted is live MVUX state a design surface can't pump — needs one, and it has its own file.
public static class PreviewData
{
    // Feeling pills: the selector picks a template per value, so one entity per value.
    public const string FeelingGreat = "Great";
    public const string FeelingGood = "Good";
    public const string FeelingTough = "Tough";
    public const string FeelingEasy = "Easy";

    // Planned workout — the card renders the type glyph (via WorkoutIcon), title, type, duration
    // and difficulty, so the states worth previewing are the ones that change the glyph or the pill.
    public static WorkoutEntry StrengthSession { get; } =
        new("w-001", "Morning Energizer", "Full Body", new DateOnly(2026, 6, 1), 20, false, "Beginner");

    public static WorkoutEntry CardioSession { get; } =
        new("w-003", "Beginner Cardio Blast", "Cardio", new DateOnly(2026, 6, 5), 25, false, "Beginner");

    // A rest day: the moon glyph, no duration, and a "Rest" pill instead of a difficulty.
    public static WorkoutEntry RestDay { get; } =
        new("w-004", "Rest Day", "Recovery", new DateOnly(2026, 6, 2), 0, true, "Rest");

    // Completed workout result (the card shared by Home's recent strip and Progress's history).
    public static WorkoutResult RecentResult { get; } =
        new("r-001", "Morning Energizer", new DateOnly(2026, 5, 31), 20, 160, "Great");

    // Milestones: the same card in its two states — unlocked shows the earned check, locked dims.
    public static MilestoneBadge UnlockedMilestone { get; } =
        new("3-Day Streak", "Worked out 3 days in a row.", true);

    public static MilestoneBadge LockedMilestone { get; } =
        new("7-Day Streak", "Worked out every day for a week.", false);

    // Fitness goals: only the chosen one shows its selected mark.
    public static FitnessGoalItem SelectedGoal { get; } = new("Build a healthy habit", true);
    public static FitnessGoalItem UnselectedGoal { get; } = new("Improve flexibility", false);

    // The richest item card in the app: glyph, name, sets/duration and the how-to description.
    public static ExerciseItem Exercise { get; } =
        new("e-002", "Bodyweight Squats", "Lower Body", 45, 3, 12, "Beginner",
            "Stand shoulder-width apart, lower hips as if sitting, keep back straight.");
}
