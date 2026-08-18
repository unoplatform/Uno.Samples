namespace UnoCRM.Presentation.MockData;

/// <summary>
/// Design-time DataContext for <see cref="DealDetailPage"/> in Hot Design / Studio. DealDetailModel
/// exposes a single <c>Deal</c> that every binding is a path off, so this mock mirrors it with one
/// materialized <see cref="Data.Deal"/>, with variants for the distinct health / won states. The
/// generated DealDetailModel VM overrides this at runtime.
/// </summary>
public partial record DealDetailPageMockData
{
    public required Deal Deal { get; init; }

    // Open deal, at risk — red health dot, "At risk", age meta.
    public static DealDetailPageMockData AtRisk { get; } = new()
    {
        Deal = CrmData.Deals.First(d => !d.IsWon && d.Health == DealHealth.AtRisk),
    };

    // Open deal, healthy — green health dot, "Healthy", age meta.
    public static DealDetailPageMockData Healthy { get; } = new()
    {
        Deal = CrmData.Deals.First(d => !d.IsWon && d.Health == DealHealth.Healthy),
    };

    // Closed-won deal — green accent, "Closed Won", "Won" meta (no day count).
    public static DealDetailPageMockData Won { get; } = new()
    {
        Deal = CrmData.Deals.First(d => d.IsWon),
    };
}
