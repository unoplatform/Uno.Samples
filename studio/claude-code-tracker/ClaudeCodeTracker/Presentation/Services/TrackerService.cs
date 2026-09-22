namespace ClaudeCodeTracker.Presentation.Services;

// The tracker's "backend": everything the app READS rather than owns. Every member is asynchronous
// and takes a CancellationToken, because that is the shape a real endpoint has — which is what lets
// the Models expose IListFeed<T> and the pages render Value / None / Progress / Error through a
// FeedView instead of hand-rolling those branches in code-behind.
//
// Session search is a SERVICE call, not a client-side filter: the criteria go to the service and it
// returns the matching set, the way a real usage API works. That is what makes "no sessions match"
// a real empty state rather than a visibility flag someone has to remember to toggle.
public interface ITrackerService
{
    /// <summary>The newest sessions, for the Dashboard's recent-activity strip.</summary>
    ValueTask<IImmutableList<SessionEntry>> GetRecentSessionsAsync(int count, CancellationToken ct = default);

    /// <summary>
    /// Sessions matching a model filter ("All", "Opus", …) and a free-text project query. Either
    /// criterion may be empty; both compose.
    /// </summary>
    ValueTask<IImmutableList<SessionEntry>> SearchSessionsAsync(
        string modelFilter,
        string? query,
        CancellationToken ct = default);

    /// <summary>Current plan rate limits, for the Usage page.</summary>
    ValueTask<IImmutableList<RateLimitInfo>> GetRateLimitsAsync(CancellationToken ct = default);

    /// <summary>Per-model spend and token share, for the Usage page.</summary>
    ValueTask<IImmutableList<ModelUsageBreakdown>> GetModelBreakdownAsync(CancellationToken ct = default);

    /// <summary>The model price list, for the Usage page's pricing table.</summary>
    ValueTask<IImmutableList<ModelInfo>> GetModelPricingAsync(CancellationToken ct = default);
}

// The in-memory implementation, standing in for an HTTP endpoint over the seed data. Swapping this
// for a real client is the only change a live backend needs: the interface, the Models and every page
// stay exactly as they are.
//
// The small delay is deliberate and load-bearing for the sample, not padding — with no latency at all
// a feed resolves on the first frame and a FeedView's ProgressTemplate could never be seen. It is
// kept short so the app still feels immediate.
public sealed class TrackerService : ITrackerService
{
    private static readonly TimeSpan Latency = TimeSpan.FromMilliseconds(300);

    public async ValueTask<IImmutableList<SessionEntry>> GetRecentSessionsAsync(int count, CancellationToken ct = default)
    {
        await Task.Delay(Latency, ct);
        return SampleData.Sessions.Take(count).ToImmutableList();
    }

    public async ValueTask<IImmutableList<SessionEntry>> SearchSessionsAsync(
        string modelFilter,
        string? query,
        CancellationToken ct = default)
    {
        await Task.Delay(Latency, ct);

        var search = (query ?? string.Empty).Trim();
        var allModels = string.IsNullOrEmpty(modelFilter)
            || string.Equals(modelFilter, SampleData.AllModels, StringComparison.OrdinalIgnoreCase);

        return SampleData.Sessions
            .Where(s => (allModels || s.ModelDisplayName.Contains(modelFilter, StringComparison.OrdinalIgnoreCase))
                        && (search.Length == 0
                            || s.ProjectName.Contains(search, StringComparison.OrdinalIgnoreCase)))
            .ToImmutableList();
    }

    public async ValueTask<IImmutableList<RateLimitInfo>> GetRateLimitsAsync(CancellationToken ct = default)
    {
        await Task.Delay(Latency, ct);
        return SampleData.RateLimits.ToImmutableList();
    }

    public async ValueTask<IImmutableList<ModelUsageBreakdown>> GetModelBreakdownAsync(CancellationToken ct = default)
    {
        await Task.Delay(Latency, ct);
        return SampleData.ModelBreakdown.ToImmutableList();
    }

    public async ValueTask<IImmutableList<ModelInfo>> GetModelPricingAsync(CancellationToken ct = default)
    {
        await Task.Delay(Latency, ct);
        return ModelCatalog.All.ToImmutableList();
    }
}
