namespace FitBeginnerApp.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record PlanModel
{
    public string WeekLabel { get; } = "Week of Jun 1 – 7, 2026";
    public int ScheduledCount { get; } = 3;
    public int CompletedCount { get; } = 0;

    public IReadOnlyList<WorkoutEntry> WeeklyPlan { get; } = new[]
    {
        new WorkoutEntry("w-001", "Morning Energizer", "Full Body", new DateOnly(2026, 6, 1), 20, false, "Beginner", "\uE945"),
        new WorkoutEntry("w-002", "Flexibility Flow", "Stretching", new DateOnly(2026, 6, 3), 15, false, "Beginner", "\uEB51"),
        new WorkoutEntry("w-003", "Beginner Cardio Blast", "Cardio", new DateOnly(2026, 6, 5), 25, false, "Beginner", "\uE76E"),
        new WorkoutEntry("w-004", "Rest Day", "Recovery", new DateOnly(2026, 6, 2), 0, true, "Rest", "\uE946"),
        new WorkoutEntry("w-005", "Core Intro", "Core", new DateOnly(2026, 6, 7), 20, false, "Beginner", "\uE9D9"),
    };

    public IReadOnlyList<SuggestedPlan> SuggestedPlans { get; } = new[]
    {
        new SuggestedPlan("3-Day Starter", "Perfect for total beginners. Three 20-min sessions per week.", 3, 20, "Full Body"),
        new SuggestedPlan("Morning Mover", "Energizing routines to start your day right. Low impact.", 4, 15, "Cardio"),
        new SuggestedPlan("Flex & Stretch", "Improve flexibility and reduce muscle tension.", 3, 25, "Flexibility"),
        new SuggestedPlan("Strength Basics", "Learn foundational movements with no equipment needed.", 3, 30, "Strength"),
    };
}

public partial record SuggestedPlan(
    string Name,
    string Description,
    int DaysPerWeek,
    int MinutesPerSession,
    string Focus);