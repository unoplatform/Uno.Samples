namespace UnoCRM.Presentation.MockData;

/// <summary>
/// Design-time DataContext for <see cref="DealDetailPage"/> in Hot Design / Studio. DealDetailModel
/// exposes a single <c>Deal</c> that every binding is a path off, so this mock mirrors it with one
/// materialized <see cref="Data.Deal"/>. Navigation replaces it with the real model at runtime.
///
/// Each variant selects on the states the page actually draws — health AND dwell band — so laying
/// the previews side by side exercises the whole range: four of the five stages, all three health
/// values, and each dwell band including "no longer tracked".
/// </summary>
public partial record DealDetailPageMockData
{
    public required Deal Deal { get; init; }

    // Late-stage, at risk, and long past the stale threshold: step 4 of 5, the filled alarm mark,
    // and the dwell gauge pinned over its last notch. The loudest the page gets.
    public static DealDetailPageMockData AtRisk { get; } = new()
    {
        Deal = CrmData.Deals
            .Where(d => d.Health == DealHealth.AtRisk)
            .OrderByDescending(d => d.AgeDays)
            .First(),
    };

    // Mid-pipeline and drifting: step 3 of 5, the outlined caution mark, and dwell just past the
    // first threshold — the state that sits between the other two on every axis.
    public static DealDetailPageMockData Watch { get; } = new()
    {
        Deal = CrmData.Deals.First(d => d.Health == DealHealth.Watch && d.AgeBand == DealAgeBand.Stalling),
    };

    // Brand new and fine: step 1 of 5, the calm round mark, one day on the gauge.
    public static DealDetailPageMockData Healthy { get; } = new()
    {
        Deal = CrmData.Deals.First(d => d.Health == DealHealth.Healthy && d.AgeBand == DealAgeBand.Fresh),
    };

    // Closed: step 5 of 5 with the whole track cleared, and no dwell time to track.
    public static DealDetailPageMockData Won { get; } = new()
    {
        Deal = CrmData.Deals.First(d => d.AgeBand == DealAgeBand.NotTracked),
    };
}
