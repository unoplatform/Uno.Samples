using FitBeginnerApp.Presentation.Data;

namespace FitBeginnerApp.Presentation.Services;

// Everything the app reads rather than owns. Every member is asynchronous and cancellable, because
// that is the shape a real endpoint has — which is what lets the Models expose IListFeed<T>/IFeed<T>
// and the pages render results, empty and failed states through a FeedView instead of hand-rolling
// them. Each page fetches its header payload as ONE call and its lists as their own, mirroring how
// a real screen loads.
public interface IFitnessService
{
    ValueTask<HomeSummary> GetHomeSummaryAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<WorkoutResult>> GetRecentResultsAsync(int count, CancellationToken ct = default);
    ValueTask<IImmutableList<QuickTip>> GetTipsAsync(CancellationToken ct = default);

    ValueTask<PlanSummary> GetPlanSummaryAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<WorkoutEntry>> GetWeeklyPlanAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<SuggestedPlan>> GetSuggestedPlansAsync(CancellationToken ct = default);

    ValueTask<ProgressStats> GetProgressStatsAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<WorkoutResult>> GetWorkoutHistoryAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<MilestoneBadge>> GetMilestonesAsync(CancellationToken ct = default);

    ValueTask<UserProfile> GetProfileAsync(CancellationToken ct = default);
    ValueTask<ProfileDetails> GetProfileDetailsAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<FitnessGoalItem>> GetGoalsAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<SettingRow>> GetSettingsAsync(CancellationToken ct = default);

    ValueTask<IImmutableList<ExerciseItem>> GetExercisesAsync(string workoutId, CancellationToken ct = default);
    ValueTask<IImmutableList<SessionTip>> GetSessionTipsAsync(CancellationToken ct = default);
}

// The in-memory implementation, standing in for an HTTP endpoint. Replacing this with a real client
// is the only change a live backend needs: the interface, the Models and every page stay as they are.
//
// The short delay is deliberate and load-bearing, not padding — without any latency a feed resolves
// on the first frame and a FeedView's ProgressTemplate would never be seen. It is kept small so the
// app still feels immediate.
public sealed class FitnessService : IFitnessService
{
    private static readonly TimeSpan Latency = TimeSpan.FromMilliseconds(300);

    private static async ValueTask<T> Fetch<T>(T value, CancellationToken ct)
    {
        await Task.Delay(Latency, ct);
        return value;
    }

    public ValueTask<HomeSummary> GetHomeSummaryAsync(CancellationToken ct = default)
        => Fetch(FitData.Home, ct);

    public ValueTask<IImmutableList<WorkoutResult>> GetRecentResultsAsync(int count, CancellationToken ct = default)
        => Fetch((IImmutableList<WorkoutResult>)FitData.WorkoutHistory.Take(count).ToImmutableList(), ct);

    public ValueTask<IImmutableList<QuickTip>> GetTipsAsync(CancellationToken ct = default)
        => Fetch(FitData.Tips, ct);

    public ValueTask<PlanSummary> GetPlanSummaryAsync(CancellationToken ct = default)
        => Fetch(FitData.Plan, ct);

    public ValueTask<IImmutableList<WorkoutEntry>> GetWeeklyPlanAsync(CancellationToken ct = default)
        => Fetch(FitData.WeeklyPlan, ct);

    public ValueTask<IImmutableList<SuggestedPlan>> GetSuggestedPlansAsync(CancellationToken ct = default)
        => Fetch(FitData.SuggestedPlans, ct);

    public ValueTask<ProgressStats> GetProgressStatsAsync(CancellationToken ct = default)
        => Fetch(FitData.Progress, ct);

    public ValueTask<IImmutableList<WorkoutResult>> GetWorkoutHistoryAsync(CancellationToken ct = default)
        => Fetch(FitData.WorkoutHistory, ct);

    public ValueTask<IImmutableList<MilestoneBadge>> GetMilestonesAsync(CancellationToken ct = default)
        => Fetch(FitData.Milestones, ct);

    public ValueTask<UserProfile> GetProfileAsync(CancellationToken ct = default)
        => Fetch(FitData.Profile, ct);

    public ValueTask<ProfileDetails> GetProfileDetailsAsync(CancellationToken ct = default)
        => Fetch(FitData.ProfileDetails, ct);

    public ValueTask<IImmutableList<FitnessGoalItem>> GetGoalsAsync(CancellationToken ct = default)
        => Fetch(FitData.Goals, ct);

    public ValueTask<IImmutableList<SettingRow>> GetSettingsAsync(CancellationToken ct = default)
        => Fetch(FitData.Settings, ct);

    // Takes the workout id the way a real endpoint would, even though the demo catalogue returns one
    // routine — the signature is what a live backend needs, and what the Model already has to hand.
    public ValueTask<IImmutableList<ExerciseItem>> GetExercisesAsync(string workoutId, CancellationToken ct = default)
        => Fetch(FitData.Exercises, ct);

    public ValueTask<IImmutableList<SessionTip>> GetSessionTipsAsync(CancellationToken ct = default)
        => Fetch(FitData.SessionTips, ct);
}
