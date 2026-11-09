using UnoCRM.Presentation.Data;

namespace UnoCRM.Presentation.Services;

// The CRM's "backend": everything the app reads rather than owns. Every member is asynchronous and
// cancellable, because that is the shape a real endpoint has — which is what lets the Models expose
// IListFeed<T>/IFeed<T> and the pages render results, empty, loading and failure states through a
// FeedView instead of hand-rolling them.
//
// Contact search is a SERVICE call taking the three filter criteria, not a client-side filter over a
// local copy. That is what turns ContactsPage's two FeedViews from two-state controls (Value/None
// only, because a synchronous filter can never be loading or failing) into the real thing.
public interface ICrmService
{
    ValueTask<DashboardData> GetDashboardAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<ActivityItem>> GetActivitiesAsync(CancellationToken ct = default);

    ValueTask<LeadsAnalytics> GetLeadsAnalyticsAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<TopLead>> GetTopOpenLeadsAsync(CancellationToken ct = default);

    ValueTask<IImmutableList<PipelineStage>> GetPipelineStagesAsync(
        string? source,
        string? period,
        string? rep,
        CancellationToken ct = default);

    ValueTask<IImmutableList<ContactLocation>> SearchContactsAsync(
        string? search,
        string? region,
        string? segment,
        CancellationToken ct = default);
}

// The in-memory implementation, standing in for an HTTP endpoint. Replacing this with a real client
// is the only change a live backend needs: the interface, the Models and every page stay as they are.
//
// The short delay is deliberate and load-bearing, not padding — without any latency a feed resolves
// on the first frame and a FeedView's ProgressTemplate would never be seen.
public sealed class CrmService : ICrmService
{
    private static readonly TimeSpan Latency = TimeSpan.FromMilliseconds(300);

    private static async ValueTask<T> Fetch<T>(T value, CancellationToken ct)
    {
        await Task.Delay(Latency, ct);
        return value;
    }

    public ValueTask<DashboardData> GetDashboardAsync(CancellationToken ct = default)
        => Fetch(CrmData.Dashboard, ct);


    public ValueTask<IImmutableList<ActivityItem>> GetActivitiesAsync(CancellationToken ct = default)
        => Fetch(CrmData.Dashboard.Activities.ToImmutableList() as IImmutableList<ActivityItem>, ct);

    public ValueTask<LeadsAnalytics> GetLeadsAnalyticsAsync(CancellationToken ct = default)
        => Fetch(CrmData.Leads, ct);

    public ValueTask<IImmutableList<TopLead>> GetTopOpenLeadsAsync(CancellationToken ct = default)
        => Fetch(CrmData.Leads.TopOpenLeads.ToImmutableList() as IImmutableList<TopLead>, ct);

    // Criteria in, board out. The five columns always come back — a stage the filter empties is a
    // column with a zero count, not a missing column.
    public async ValueTask<IImmutableList<PipelineStage>> GetPipelineStagesAsync(
        string? source,
        string? period,
        string? rep,
        CancellationToken ct = default)
    {
        await Task.Delay(Latency, ct);

        var src = source ?? PipelineModel.AllSources;
        var owner = rep ?? PipelineModel.AllReps;
        var maxAgeDays = period switch
        {
            PipelineModel.ThisWeek => 7,
            PipelineModel.ThisMonth => 30,
            _ => 90,
        };

        var matching = CrmData.Deals
            .Where(d => src == PipelineModel.AllSources
                        || d.Source.Equals(src, StringComparison.OrdinalIgnoreCase))
            .Where(d => owner == PipelineModel.AllReps
                        || d.Owner.Equals(owner, StringComparison.OrdinalIgnoreCase))
            .Where(d => d.AgeDays <= maxAgeDays);

        return CrmData.StagesFrom(matching).ToImmutableList();
    }

    // Criteria in, matching set out — the shape a real contacts endpoint has.
    public async ValueTask<IImmutableList<ContactLocation>> SearchContactsAsync(
        string? search,
        string? region,
        string? segment,
        CancellationToken ct = default)
    {
        await Task.Delay(Latency, ct);

        var query = (search ?? string.Empty).Trim();
        var reg = region ?? ContactsModel.AllRegions;
        var seg = segment ?? ContactsModel.AllSegments;

        return CrmData.Contacts
            .Where(x => reg == ContactsModel.AllRegions
                        || x.Region.Equals(reg, StringComparison.OrdinalIgnoreCase))
            .Where(x => seg == ContactsModel.AllSegments
                        || x.Segment.Equals(seg, StringComparison.OrdinalIgnoreCase))
            .Where(x => query.Length == 0
                        || x.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
                        || x.Company.Contains(query, StringComparison.OrdinalIgnoreCase)
                        || x.City.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToImmutableList();
    }
}
