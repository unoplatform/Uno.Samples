namespace FitBeginnerApp.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record ProgressModel
{
    public int TotalWorkouts { get; } = 12;
    public int TotalMinutes { get; } = 245;
    public int TotalCalories { get; } = 1840;
    public int CurrentStreak { get; } = 5;
    public int LongestStreak { get; } = 7;

    public IReadOnlyList<WorkoutResult> WorkoutHistory { get; } = new[]
    {
        new WorkoutResult("r-001", "Morning Energizer", new DateOnly(2026, 5, 31), 20, 160, "Great"),
        new WorkoutResult("r-002", "Flexibility Flow", new DateOnly(2026, 5, 29), 15, 90, "Good"),
        new WorkoutResult("r-003", "Beginner Cardio Blast", new DateOnly(2026, 5, 27), 25, 200, "Tough"),
        new WorkoutResult("r-004", "Core Intro", new DateOnly(2026, 5, 25), 20, 150, "Good"),
        new WorkoutResult("r-005", "Morning Energizer", new DateOnly(2026, 5, 23), 20, 155, "Great"),
        new WorkoutResult("r-006", "Full Body Starter", new DateOnly(2026, 5, 20), 30, 230, "Tough"),
        new WorkoutResult("r-007", "Flexibility Flow", new DateOnly(2026, 5, 18), 15, 85, "Easy"),
    };

    public IReadOnlyList<MilestoneBadge> Milestones { get; } = new[]
    {
        new MilestoneBadge("First Workout!", "You completed your very first session.", true),
        new MilestoneBadge("3-Day Streak", "Worked out 3 days in a row.", true),
        new MilestoneBadge("5 Workouts Done", "You have completed 5 workouts.", true),
        // Unlocked flags follow the stats above: 12 total workouts, 245 minutes, longest streak 7.
        new MilestoneBadge("7-Day Streak", "Worked out every day for a week.", true),
        new MilestoneBadge("10 Workouts", "Reached 10 total sessions.", true),
        new MilestoneBadge("100 Minutes Active", "Clocked 100 minutes of exercise.", true),
        new MilestoneBadge("20 Workouts", "Reached 20 total sessions.", false),
        new MilestoneBadge("30-Day Streak", "A full month, every day.", false),
    };
}

public partial record MilestoneBadge(
    string Title,
    string Description,
    bool IsUnlocked);
