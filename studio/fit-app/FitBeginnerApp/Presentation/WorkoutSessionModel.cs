namespace FitBeginnerApp.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record WorkoutSessionModel
{
    public string WorkoutTitle { get; } = "Morning Energizer";
    public string WorkoutType { get; } = "Full Body";
    public int TotalDurationMinutes { get; } = 20;
    public string Difficulty { get; } = "Beginner";
    public int TotalExercises { get; } = 5;
    public string MotivationQuote { get; } = "Every rep counts. You've got this!";

    public IReadOnlyList<ExerciseItem> Exercises { get; } = new[]
    {
        new ExerciseItem("e-001", "Jumping Jacks", "Warm-up", 60, 3, 0, "Beginner", "\uE945",
            "Stand with feet together, jump while raising arms overhead and spreading legs."),
        new ExerciseItem("e-002", "Bodyweight Squats", "Lower Body", 45, 3, 12, "Beginner", "\uE9D9",
            "Stand shoulder-width apart, lower hips as if sitting, keep back straight."),
        new ExerciseItem("e-003", "Push-Ups (Knee)", "Upper Body", 40, 3, 10, "Beginner", "\uEB51",
            "On all fours, lower chest toward the ground keeping core engaged. Knees stay down."),
        new ExerciseItem("e-004", "Plank Hold", "Core", 30, 3, 0, "Beginner", "\uE76E",
            "Hold a flat body position on forearms and toes for 30 seconds."),
        new ExerciseItem("e-005", "Cool-Down Walk", "Cool-down", 120, 1, 0, "Beginner", "\uE946",
            "Walk slowly in place and take deep breaths to lower heart rate."),
    };

    public IReadOnlyList<SessionTip> WarmUpNotes { get; } = new[]
    {
        new SessionTip("Rest 30–60s between sets."),
        new SessionTip("Breathe out on the effort, in on the recovery."),
        new SessionTip("Stop if you feel sharp pain — discomfort is normal, pain is not."),
    };
}

public partial record SessionTip(string Note);