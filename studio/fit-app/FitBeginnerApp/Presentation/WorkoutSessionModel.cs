using System.Threading.Tasks;
using Uno.Extensions.Reactive;

namespace FitBeginnerApp.Presentation;

// Bound to a single workout via DataViewMap<WorkoutSessionPage, WorkoutSessionModel, WorkoutEntry>:
// Navigation passes the tapped WorkoutEntry as the record's parameter, so the header reflects the
// chosen workout. The exercise list is mock (shared for the demo).
public partial record WorkoutSessionModel(WorkoutEntry Workout, IFitnessService Fitness)
{
    public string WorkoutTitle => Workout.Title;
    public string WorkoutType => Workout.Type;
    public int TotalDurationMinutes => Workout.DurationMinutes;
    public string Difficulty => Workout.Difficulty;
    // Exercises is now a list FEED, so this is exactly the trap the comment here used to warn
    // about: an empty list feed emits None and Select() skips None, which would render the count
    // BLANK rather than "0". SelectData is handed the Option itself, so it can turn None into 0.
    public IFeed<int> TotalExercises =>
        Exercises
            .AsFeed()
            .SelectData<IImmutableList<ExerciseItem>, int>(items => items.SomeOrDefault()?.Count ?? 0);

    public string MotivationQuote { get; } = "Every rep counts. You've got this!";

    // The routine's exercises and form notes come from the service, keyed by the workout that
    // Navigation passed in. The exercise list is the page's primary content, so it is rendered by a
    // FeedView; the short notes card binds directly.
    private IListFeed<ExerciseItem>? _exercises;
    public IListFeed<ExerciseItem> Exercises =>
        _exercises ??= ListFeed.Async(ct => Fitness.GetExercisesAsync(Workout.Id, ct));

    private IListFeed<SessionTip>? _warmUpNotes;
    public IListFeed<SessionTip> WarmUpNotes =>
        _warmUpNotes ??= ListFeed.Async(Fitness.GetSessionTipsAsync);

    // Whether the guided session has been started. MVUX state; the Begin command flips it and the
    // page swaps the "Begin workout" CTA for an in-progress confirmation.
    public IState<bool> IsStarted => State.Value(this, () => false);

    public async ValueTask Begin() => await IsStarted.UpdateAsync(_ => true);
}

public partial record SessionTip(string Note);
