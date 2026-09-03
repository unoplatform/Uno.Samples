namespace FitBeginnerApp.Presentation.MockData;

// Design-time DataContext for ProgressPage. Feed-shaped for the same reason as HomePageMockData.
[Uno.Extensions.Reactive.ReactiveBindable]
public partial record ProgressPageMockData
{
    // Default design-time state: an established member with history and badges.
    public static ProgressPageMockDataViewModel Data => new();

    // Day one: zeroed stats, no history, nothing unlocked — both FeedViews on their NoneTemplate.
    public static ProgressPageMockDataViewModel JustStarted =>
        ProgressPageMockDataViewModel.ForModel(new()
        {
            TotalWorkouts = 0,
            TotalMinutes = 0,
            TotalCalories = 0,
            CurrentStreak = 0,
            LongestStreak = 0,
            WorkoutHistory = ListFeed.Async(_ =>
                ValueTask.FromResult<IImmutableList<WorkoutResult>>(ImmutableList<WorkoutResult>.Empty)),
            Milestones = ListFeed.Async(_ =>
                ValueTask.FromResult<IImmutableList<MilestoneBadge>>(ImmutableList<MilestoneBadge>.Empty)),
        });

    public int TotalWorkouts { get; init; } = FitData.Progress.TotalWorkouts;
    public int TotalMinutes { get; init; } = FitData.Progress.TotalMinutes;
    public int TotalCalories { get; init; } = FitData.Progress.TotalCalories;
    public int CurrentStreak { get; init; } = FitData.Progress.CurrentStreak;
    public int LongestStreak { get; init; } = FitData.Progress.LongestStreak;

    public IListFeed<WorkoutResult> WorkoutHistory { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult(FitData.WorkoutHistory));

    public IListFeed<MilestoneBadge> Milestones { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult(FitData.Milestones));
}

public partial class ProgressPageMockDataViewModel
{
    internal static ProgressPageMockDataViewModel ForModel(ProgressPageMockData model) => new(model);
}
