namespace FitBeginnerApp.Presentation;

// Bound to a single workout via DataViewMap<WorkoutSessionPage, WorkoutSessionModel, WorkoutEntry>:
// Navigation passes the tapped WorkoutEntry as the record's parameter, so the header reflects the
// chosen workout. The exercise list is mock (shared for the demo).
[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record WorkoutSessionModel(WorkoutEntry Workout)
{
    public string WorkoutTitle => Workout.Title;
    public string WorkoutType => Workout.Type;
    public int TotalDurationMinutes => Workout.DurationMinutes;
    public string Difficulty => Workout.Difficulty;
    public int TotalExercises => Exercises.Count;

    public string MotivationQuote { get; } = "Every rep counts. You've got this!";

    public IReadOnlyList<ExerciseItem> Exercises { get; } = new[]
    {
        new ExerciseItem("e-001", "Jumping Jacks", "Warm-up", 60, 3, 0, "Beginner",
            "Stand with feet together, jump while raising arms overhead and spreading legs."),
        new ExerciseItem("e-002", "Bodyweight Squats", "Lower Body", 45, 3, 12, "Beginner",
            "Stand shoulder-width apart, lower hips as if sitting, keep back straight."),
        new ExerciseItem("e-003", "Push-Ups (Knee)", "Upper Body", 40, 3, 10, "Beginner",
            "On all fours, lower chest toward the ground keeping core engaged. Knees stay down."),
        new ExerciseItem("e-004", "Plank Hold", "Core", 30, 3, 0, "Beginner",
            "Hold a flat body position on forearms and toes for 30 seconds."),
        new ExerciseItem("e-005", "Cool-Down Walk", "Cool-down", 120, 1, 0, "Beginner",
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
