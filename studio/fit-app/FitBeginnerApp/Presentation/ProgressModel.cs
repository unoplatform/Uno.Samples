namespace FitBeginnerApp.Presentation;

// All-time stats, milestones and workout history — all read from IFitnessService. Both lists are
// rendered by a FeedView, so a brand-new account (no history, nothing unlocked) has real UI instead
// of an empty page.
public partial record ProgressModel(IFitnessService Fitness)
{
    private IFeed<ProgressStats>? _stats;
    private IFeed<ProgressStats> Stats => _stats ??= Feed.Async(Fitness.GetProgressStatsAsync);

    public IFeed<int> TotalWorkouts => Stats.Select(s => s.TotalWorkouts);
    public IFeed<int> TotalMinutes => Stats.Select(s => s.TotalMinutes);
    public IFeed<int> TotalCalories => Stats.Select(s => s.TotalCalories);
    public IFeed<int> CurrentStreak => Stats.Select(s => s.CurrentStreak);
    public IFeed<int> LongestStreak => Stats.Select(s => s.LongestStreak);

    private IListFeed<WorkoutResult>? _history;
    public IListFeed<WorkoutResult> WorkoutHistory =>
        _history ??= ListFeed.Async(Fitness.GetWorkoutHistoryAsync);

    private IListFeed<MilestoneBadge>? _milestones;
    public IListFeed<MilestoneBadge> Milestones =>
        _milestones ??= ListFeed.Async(Fitness.GetMilestonesAsync);
}

public partial record MilestoneBadge(
    string Title,
    string Description,
    bool IsUnlocked);
