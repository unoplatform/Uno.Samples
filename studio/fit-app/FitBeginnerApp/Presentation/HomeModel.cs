namespace FitBeginnerApp.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record HomeModel
{
    public UserProfile CurrentUser { get; } = new(
        "Alex Rivera",
        "Beginner",
        "Build a healthy habit",
        3,
        5,
        12);

    public string GreetingMessage { get; } = "Good morning, Alex!";
    public string MotivationMessage { get; } = "5-day streak — keep it up!";

    public WorkoutEntry TodayWorkout { get; } = new(
        "w-001",
        "Morning Energizer",
        "Full Body",
        new DateOnly(2026, 6, 1),
        20,
        false,
        "Beginner");

    public int WeeklyCompletedDays { get; } = 2;
    public int WeeklyGoalDays { get; } = 3;
    public int TotalMinutesThisWeek { get; } = 40;
    public int CaloriesBurnedThisWeek { get; } = 320;

    // "completed / goal" days this week, formatted for the weekly-stats tile.
    public string WeeklyDaysText => $"{WeeklyCompletedDays}/{WeeklyGoalDays}";

    public IReadOnlyList<WorkoutResult> RecentResults { get; } = new[]
    {
        new WorkoutResult("r-001", "Morning Energizer", new DateOnly(2026, 5, 31), 20, 160, "Great"),
        new WorkoutResult("r-002", "Flexibility Flow", new DateOnly(2026, 5, 29), 15, 90, "Good"),
        new WorkoutResult("r-003", "Beginner Cardio Blast", new DateOnly(2026, 5, 27), 25, 200, "Tough"),
    };

    public IReadOnlyList<QuickTip> Tips { get; } = new[]
    {
        new QuickTip("Stay Hydrated", "Drink water before, during, and after exercise."),
        new QuickTip("Warm Up First", "Spend 5 min stretching to prevent injury."),
        new QuickTip("Rest Days Matter", "Muscles grow during rest — don't skip them."),
    };
}

public partial record QuickTip(string Title, string Body);
