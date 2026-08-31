namespace FitBeginnerApp.Presentation.Data;

// The app's seed data, in one place rather than hardcoded onto each page Model. Nothing outside
// IFitnessService reads this: the Models talk to the service, the service reads from here. Swapping
// the service for an HTTP-backed one is then a single-file change.
public static class FitData
{
    public static UserProfile Profile { get; } =
        new("Alex Rivera", "Beginner", "Build a healthy habit", 3, 5, 12);

    public static HomeSummary Home { get; } = new(
        Greeting: "Good morning, Alex!",
        Motivation: "5-day streak — keep it up!",
        TodayWorkout: new WorkoutEntry("w-001", "Morning Energizer", "Full Body", new DateOnly(2026, 6, 1), 20, false, "Beginner"),
        WeeklyCompletedDays: 2,
        WeeklyGoalDays: 3,
        TotalMinutesThisWeek: 40,
        CaloriesBurnedThisWeek: 320);

    public static PlanSummary Plan { get; } =
        new(WeekLabel: "Week of Jun 1 – 7, 2026", ScheduledCount: 4, CompletedCount: 1);

    public static ProgressStats Progress { get; } = new(
        TotalWorkouts: 12,
        TotalMinutes: 245,
        TotalCalories: 1840,
        CurrentStreak: 5,
        LongestStreak: 7);

    public static ProfileDetails ProfileDetails { get; } = new(
        AvatarInitials: "AR",
        JoinedDate: "Joined April 2026",
        PreferredTime: "Morning",
        EquipmentAvailable: "No equipment",
        SessionLengthMinutes: 20);

    public static IImmutableList<WorkoutEntry> WeeklyPlan { get; } =
    [
        new("w-001", "Morning Energizer", "Full Body", new DateOnly(2026, 6, 1), 20, false, "Beginner"),
        new("w-004", "Rest Day", "Recovery", new DateOnly(2026, 6, 2), 0, true, "Rest"),
        new("w-002", "Flexibility Flow", "Stretching", new DateOnly(2026, 6, 3), 15, false, "Beginner"),
        new("w-003", "Beginner Cardio Blast", "Cardio", new DateOnly(2026, 6, 5), 25, false, "Beginner"),
        new("w-005", "Core Intro", "Core", new DateOnly(2026, 6, 7), 20, false, "Beginner"),
    ];

    public static IImmutableList<SuggestedPlan> SuggestedPlans { get; } =
    [
        new("3-Day Starter", "Perfect for total beginners. Three 20-min sessions per week.", 3, 20, "Full Body"),
        new("Morning Mover", "Energizing routines to start your day right. Low impact.", 4, 15, "Cardio"),
        new("Flex & Stretch", "Improve flexibility and reduce muscle tension.", 3, 25, "Flexibility"),
        new("Strength Basics", "Learn foundational movements with no equipment needed.", 3, 30, "Strength"),
    ];

    // Newest first — the Home strip takes the first few, the Progress page shows all of them.
    public static IImmutableList<WorkoutResult> WorkoutHistory { get; } =
    [
        new("r-001", "Morning Energizer", new DateOnly(2026, 5, 31), 20, 160, "Great"),
        new("r-002", "Flexibility Flow", new DateOnly(2026, 5, 29), 15, 90, "Good"),
        new("r-003", "Beginner Cardio Blast", new DateOnly(2026, 5, 27), 25, 200, "Tough"),
        new("r-004", "Core Intro", new DateOnly(2026, 5, 25), 20, 150, "Good"),
        new("r-005", "Morning Energizer", new DateOnly(2026, 5, 23), 20, 155, "Great"),
        new("r-006", "Full Body Starter", new DateOnly(2026, 5, 20), 30, 230, "Tough"),
        new("r-007", "Flexibility Flow", new DateOnly(2026, 5, 18), 15, 85, "Easy"),
    ];

    // Unlocked flags follow the stats above: 12 total workouts, 245 minutes, longest streak 7.
    public static IImmutableList<MilestoneBadge> Milestones { get; } =
    [
        new("First Workout!", "You completed your very first session.", true),
        new("3-Day Streak", "Worked out 3 days in a row.", true),
        new("5 Workouts Done", "You have completed 5 workouts.", true),
        new("7-Day Streak", "Worked out every day for a week.", true),
        new("10 Workouts", "Reached 10 total sessions.", true),
        new("100 Minutes Active", "Clocked 100 minutes of exercise.", true),
        new("20 Workouts", "Reached 20 total sessions.", false),
        new("30-Day Streak", "A full month, every day.", false),
    ];

    public static IImmutableList<QuickTip> Tips { get; } =
    [
        new("Stay Hydrated", "Drink water before, during, and after exercise."),
        new("Warm Up First", "Spend 5 min stretching to prevent injury."),
        new("Rest Days Matter", "Muscles grow during rest — don't skip them."),
    ];

    public static IImmutableList<SettingRow> Settings { get; } =
    [
        new("Workout Reminders", "Daily at 7:00 AM"),
        new("Rest Day Alerts", "Notify me on over-training"),
        new("Weekly Summary", "Every Sunday evening"),
        new("Language", "English"),
        new("Help & FAQ", "Tips for beginners"),
    ];

    public static IImmutableList<FitnessGoalItem> Goals { get; } =
    [
        new("Build a healthy habit", true),
        new("Lose weight gradually", false),
        new("Improve flexibility", false),
        new("Increase stamina", false),
    ];

    public static IImmutableList<ExerciseItem> Exercises { get; } =
    [
        new("e-001", "Jumping Jacks", "Warm-up", 60, 3, 0, "Beginner",
            "Stand with feet together, jump while raising arms overhead and spreading legs."),
        new("e-002", "Bodyweight Squats", "Lower Body", 45, 3, 12, "Beginner",
            "Stand shoulder-width apart, lower hips as if sitting, keep back straight."),
        new("e-003", "Push-Ups (Knee)", "Upper Body", 40, 3, 10, "Beginner",
            "On all fours, lower chest toward the ground keeping core engaged. Knees stay down."),
        new("e-004", "Plank Hold", "Core", 30, 3, 0, "Beginner",
            "Hold a flat body position on forearms and toes for 30 seconds."),
        new("e-005", "Cool-Down Walk", "Cool-down", 120, 1, 0, "Beginner",
            "Walk slowly in place and take deep breaths to lower heart rate."),
    ];

    public static IImmutableList<SessionTip> SessionTips { get; } =
    [
        new("Rest 30–60s between sets."),
        new("Breathe out on the effort, in on the recovery."),
        new("Stop if you feel sharp pain — discomfort is normal, pain is not."),
    ];
}
