namespace FitBeginnerApp.Presentation;

// The weekly training plan. The header counters and both lists come from IFitnessService, so each
// list is a feed the page renders through a FeedView — an empty week and a failed request are
// designed states rather than a blank gap.
public partial record PlanModel(IFitnessService Fitness)
{
    private IFeed<PlanSummary>? _summary;
    private IFeed<PlanSummary> Summary => _summary ??= Feed.Async(Fitness.GetPlanSummaryAsync);

    public IFeed<string> WeekLabel => Summary.Select(s => s.WeekLabel);
    public IFeed<int> ScheduledCount => Summary.Select(s => s.ScheduledCount);
    public IFeed<int> CompletedCount => Summary.Select(s => s.CompletedCount);

    private IListFeed<WorkoutEntry>? _weeklyPlan;
    public IListFeed<WorkoutEntry> WeeklyPlan =>
        _weeklyPlan ??= ListFeed.Async(Fitness.GetWeeklyPlanAsync);

    private IListFeed<SuggestedPlan>? _suggestedPlans;
    public IListFeed<SuggestedPlan> SuggestedPlans =>
        _suggestedPlans ??= ListFeed.Async(Fitness.GetSuggestedPlansAsync);
}

public partial record SuggestedPlan(
    string Name,
    string Description,
    int DaysPerWeek,
    int MinutesPerSession,
    string Focus);
