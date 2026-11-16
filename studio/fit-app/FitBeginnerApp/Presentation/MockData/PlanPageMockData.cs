namespace FitBeginnerApp.Presentation.MockData;

// Design-time DataContext for PlanPage. Feed-shaped for the same reason as HomePageMockData — see
// that file for the full rationale.
[Uno.Extensions.Reactive.ReactiveBindable]
public partial record PlanPageMockData
{
    // Default design-time state: a full training week.
    public static PlanPageMockDataViewModel Data => new();

    // A member who hasn't picked a plan: the week list falls through to its NoneTemplate while the
    // suggestions below it stay populated, which is exactly what that empty state is for.
    public static PlanPageMockDataViewModel EmptyWeek =>
        PlanPageMockDataViewModel.ForModel(new()
        {
            ScheduledCount = 0,
            CompletedCount = 0,
            WeeklyPlan = ListFeed.Async(_ =>
                ValueTask.FromResult<IImmutableList<WorkoutEntry>>(ImmutableList<WorkoutEntry>.Empty)),
        });

    public string WeekLabel { get; init; } = FitData.Plan.WeekLabel;
    public int ScheduledCount { get; init; } = FitData.Plan.ScheduledCount;
    public int CompletedCount { get; init; } = FitData.Plan.CompletedCount;

    public IListFeed<WorkoutEntry> WeeklyPlan { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult(FitData.WeeklyPlan));

    public IListFeed<SuggestedPlan> SuggestedPlans { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult(FitData.SuggestedPlans));
}

public partial class PlanPageMockDataViewModel
{
    internal static PlanPageMockDataViewModel ForModel(PlanPageMockData model) => new(model);
}
