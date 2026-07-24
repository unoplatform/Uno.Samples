namespace UnoCRM.Presentation;

/// <summary>
/// Backs <see cref="DashboardPage"/>. A pure projection of the shared, read-only CRM data — no
/// reactive members — so it opts out of the MVUX bindable generator with
/// <c>[ReactiveBindable(false)]</c> and is used directly as the page DataContext. It exposes the
/// same member names the XAML binds, reading from the seeded <see cref="CrmData"/> dataset.
/// </summary>
[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record DashboardModel
{
    private static DashboardData Data => CrmData.Dashboard;

    public string TotalLeadsText => Data.TotalLeadsText;
    public string TotalLeadsDelta => Data.TotalLeadsDelta;
    public string ActiveDealsText => Data.ActiveDealsText;
    public string ActiveDealsDelta => Data.ActiveDealsDelta;
    public string RevenueText => Data.RevenueText;
    public string RevenueDelta => Data.RevenueDelta;
    public string ConversionRateText => Data.ConversionRateText;
    public string ConversionRateDelta => Data.ConversionRateDelta;

    public IReadOnlyList<FunnelStage> Funnel => Data.Funnel;
    public IReadOnlyList<ActivityItem> Activities => Data.Activities;
}
