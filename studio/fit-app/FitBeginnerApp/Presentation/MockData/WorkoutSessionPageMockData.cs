namespace FitBeginnerApp.Presentation.MockData;

// Design-time DataContext for WorkoutSessionPage. Mirrors WorkoutSessionModel's binding surface with
// plain, materialized values — the live Model exposes IsStarted as an IState<bool>, which a design
// surface has no context to pump, so the Begin / In-progress branch would render as neither at
// design time. A plain bool satisfies the same {Binding IsStarted, Converter=...} and lets each
// preview pin the branch it wants. At runtime the DataViewMap injects the real generated VM.
public partial record WorkoutSessionPageMockData
{
    // Default design-time state: the session as first opened, showing the "Begin workout" CTA.
    public static WorkoutSessionPageMockData Data { get; } = new();

    // A second design-time state: the session started, so the in-progress confirmation replaces the
    // CTA. The "In Progress" preview uses this.
    public static WorkoutSessionPageMockData InProgress { get; } = new() { IsStarted = true };

    // Init-settable so a variant (see InProgress) can flip the branch; defaults to not started.
    public bool IsStarted { get; init; }

    public string WorkoutTitle => "Morning Energizer";
    public string WorkoutType => "Full Body";
    public int TotalDurationMinutes => 20;
    public string Difficulty => "Beginner";
    public int TotalExercises => Exercises.Count;

    public string MotivationQuote => "Every rep counts. You've got this!";

    public IReadOnlyList<ExerciseItem> Exercises { get; } = new[]
    {
        new ExerciseItem("e-001", "Jumping Jacks", "Warm-up", 60, 3, 0, "Beginner",
            "Stand with feet together, jump while raising arms overhead and spreading legs."),
        new ExerciseItem("e-002", "Bodyweight Squats", "Lower Body", 45, 3, 12, "Beginner",
            "Stand shoulder-width apart, lower hips as if sitting, keep back straight."),
        new ExerciseItem("e-004", "Plank Hold", "Core", 30, 3, 0, "Beginner",
            "Hold a flat body position on forearms and toes for 30 seconds."),
    };

    public IReadOnlyList<SessionTip> WarmUpNotes { get; } = new[]
    {
        new SessionTip("Rest 30–60s between sets."),
        new SessionTip("Breathe out on the effort, in on the recovery."),
    };

    public void Begin() { }
}
