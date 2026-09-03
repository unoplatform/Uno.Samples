namespace FitBeginnerApp.Presentation.MockData;

// Design-time DataContext for WorkoutSessionPage. Two things need the generated ViewModel here: the
// exercise list is a FeedView (which can only subscribe to a feed), and IsStarted is live MVUX state
// a design surface cannot pump — the converter would see an IState<bool> object rather than a bool
// and fall through to false, so the Begin branch would be right only by accident and the started
// state unreachable. A plain init-settable bool fixes both. See HomePageMockData for the rest.
[Uno.Extensions.Reactive.ReactiveBindable]
public partial record WorkoutSessionPageMockData
{
    // Default design-time state: the session as first opened, showing the "Begin workout" CTA.
    public static WorkoutSessionPageMockDataViewModel Data => new();

    // The session started, so the in-progress confirmation replaces the CTA.
    public static WorkoutSessionPageMockDataViewModel InProgress =>
        WorkoutSessionPageMockDataViewModel.ForModel(new() { IsStarted = true });

    public bool IsStarted { get; init; }

    public string WorkoutTitle { get; init; } = "Morning Energizer";
    public string WorkoutType { get; init; } = "Full Body";
    public int TotalDurationMinutes { get; init; } = 20;
    public string Difficulty { get; init; } = "Beginner";
    public int TotalExercises { get; init; } = FitData.Exercises.Count;

    public string MotivationQuote { get; init; } = "Every rep counts. You've got this!";

    public IListFeed<ExerciseItem> Exercises { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult(FitData.Exercises));

    public IListFeed<SessionTip> WarmUpNotes { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult(FitData.SessionTips));

    public void Begin() { }
}

public partial class WorkoutSessionPageMockDataViewModel
{
    internal static WorkoutSessionPageMockDataViewModel ForModel(WorkoutSessionPageMockData model) => new(model);
}
