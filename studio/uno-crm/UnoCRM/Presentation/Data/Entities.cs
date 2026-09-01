using System.Globalization;

namespace UnoCRM.Presentation.Data;

/// <summary>
/// The stages a deal moves through in the sales pipeline. The order here is the order the
/// stages render across the Pipeline board and the Dashboard funnel.
/// </summary>
public enum DealStage
{
    NewLead,
    Qualified,
    Proposal,
    Negotiation,
    ClosedWon,
}

/// <summary>How healthy/at-risk a deal is — drives the colored status dot on a pipeline card.</summary>
public enum DealHealth
{
    Healthy,
    Watch,
    AtRisk,
}

/// <summary>
/// A single sales deal — the atomic record the whole sample is built from. A stable <see cref="Id"/>
/// carries key equality so MVUX list feeds and navigation match the right deal.
/// </summary>
public partial record Deal(
    [property: global::Uno.Extensions.Equality.Key] string Id,
    string Company,
    decimal Amount,
    string Owner,
    DealStage Stage,
    DealHealth Health,
    int AgeDays,
    string Source)
{
    private static readonly CultureInfo Usd = CultureInfo.GetCultureInfo("en-US");

    public bool IsWon => Stage == DealStage.ClosedWon;

    /// <summary>e.g. <c>$45,000</c>.</summary>
    public string AmountDisplay => Amount.ToString("C0", Usd);

    /// <summary>Right-aligned meta on a card: the age in days, or "Won" for closed deals.</summary>
    public string MetaDisplay => IsWon ? "Won" : $"{AgeDays}d";

    /// <summary>Human-readable stage label, e.g. "Closed Won".</summary>
    public string StageDisplay => Stage switch
    {
        DealStage.NewLead => "New Lead",
        DealStage.Qualified => "Qualified",
        DealStage.Proposal => "Proposal",
        DealStage.Negotiation => "Negotiation",
        DealStage.ClosedWon => "Closed Won",
        _ => Stage.ToString(),
    };

    /// <summary>Resource key for the card's accent dot / check — green when won, else by health.</summary>
    public string AccentKey => IsWon
        ? "DashboardGreenBrush"
        : Health switch
        {
            DealHealth.AtRisk => "DashboardRedBrush",
            DealHealth.Watch => "DashboardAmberBrush",
            _ => "DashboardGreenBrush",
        };

    /// <summary>Human-readable health label for a detail view.</summary>
    public string HealthDisplay => Health switch
    {
        DealHealth.AtRisk => "At risk",
        DealHealth.Watch => "Watch",
        _ => "Healthy",
    };
}

/// <summary>A pipeline column: a stage plus the deals currently in it, with its palette keys.
/// <see cref="AccentKey"/> fills the stage dot; <see cref="DeepKey"/> is the darker (light theme)
/// / lighter (dark theme) text variant for the small count set on the <see cref="SoftKey"/> tint.</summary>
// NOTE — this record is a FEED VALUE type (see the Model that exposes it), and the MVUX generator
// emits a bindable proxy for every feed value type which it constructs with an object initializer.
// `required` members reject that, so the build fails inside the generated code. The members therefore
// carry defaults instead of `required`: the record stays default-constructible for the generator and
// non-null for the compiler, and every call site in CrmData still sets all of them explicitly.
public partial record PipelineStage
{
    public string Name { get; init; } = string.Empty;
    public DealStage Stage { get; init; }
    public string AccentKey { get; init; } = string.Empty;
    public string DeepKey { get; init; } = string.Empty;
    public string SoftKey { get; init; } = string.Empty;
    public IReadOnlyList<Deal> Deals { get; init; } = [];

    public int Count => Deals.Count;
}

/// <summary>
/// One bar in the Dashboard "Pipeline Summary": a stage label, its count, its fill brush key and the
/// filled fraction (0..1). The bar's two columns are computed from <see cref="FillFraction"/> in XAML
/// (via DoubleToGridLengthConverter) so no UI type leaks into the data layer.
/// </summary>
public partial record FunnelStage
{
    public required string Name { get; init; }
    public required int Count { get; init; }
    public required string FillKey { get; init; }
    public required double FillFraction { get; init; }
}

/// <summary>A row in the Dashboard "Recent Activity" feed.</summary>
public partial record ActivityItem(
    string Title,
    string Detail,
    string TimeAgo);

/// <summary>A row in the Leads "Top Open Leads" list.</summary>
public partial record TopLead(string Company, string AmountDisplay);

/// <summary>A contact plotted on the Contacts map and listed alongside it.</summary>
public partial record ContactLocation(
    [property: global::Uno.Extensions.Equality.Key] string Id,
    string Name,
    string Company,
    string City,
    string Region,
    string Segment,
    double Latitude,
    double Longitude);

/// <summary>Everything the Dashboard page shows, derived once from <see cref="CrmData.Deals"/>.</summary>
// NOTE — this record is a FEED VALUE type (see the Model that exposes it), and the MVUX generator
// emits a bindable proxy for every feed value type which it constructs with an object initializer.
// `required` members reject that, so the build fails inside the generated code. The members therefore
// carry defaults instead of `required`: the record stays default-constructible for the generator and
// non-null for the compiler, and every call site in CrmData still sets all of them explicitly.
public partial record DashboardData
{
    public string TotalLeadsText { get; init; } = string.Empty;
    public string TotalLeadsDelta { get; init; } = string.Empty;
    public string ActiveDealsText { get; init; } = string.Empty;
    public string ActiveDealsDelta { get; init; } = string.Empty;
    public string RevenueText { get; init; } = string.Empty;
    public string RevenueDelta { get; init; } = string.Empty;
    public string ConversionRateText { get; init; } = string.Empty;
    public string ConversionRateDelta { get; init; } = string.Empty;
    public IReadOnlyList<FunnelStage> Funnel { get; init; } = [];
    public IReadOnlyList<ActivityItem> Activities { get; init; } = [];
}

/// <summary>
/// The numbers behind the Leads charts and KPIs. Chart <c>ISeries</c>/<c>Axis</c> objects are
/// assembled in <see cref="LeadsModel"/> (they need SkiaSharp paints), but every value they plot
/// comes from here so the page is stable and consistent with the rest of the app.
/// </summary>
public partial record LeadsAnalytics
{
    public required string NewLeadsText { get; init; }
    public required string QualificationRateText { get; init; }
    public required string PipelineValueText { get; init; }
    public required string AverageDealSizeText { get; init; }

    public required string[] MonthLabels { get; init; }
    public required int[] MonthlyLeads { get; init; }
    public required string[] SourceLabels { get; init; }
    public required int[] SourceCounts { get; init; }
    public required string[] StageLabels { get; init; }
    public required int[] StageCounts { get; init; }

    public required IReadOnlyList<TopLead> TopOpenLeads { get; init; }
}
