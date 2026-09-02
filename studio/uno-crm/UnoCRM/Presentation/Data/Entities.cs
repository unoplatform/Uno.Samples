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
/// How long a deal has sat in its current stage, banded for display: inside its first week,
/// past it, or past three weeks. A closed deal is <see cref="NotTracked"/> — it is not sitting in
/// a stage at all any more, so a day count would be meaningless.
/// </summary>
public enum DealAgeBand
{
    Fresh,
    Stalling,
    Stale,
    NotTracked,
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

    // The pipeline is a fixed sequence, so a stage is a POSITION in it. Reading the length off the
    // enum keeps the "STEP n OF m" read-out in step with the board if a stage is ever added.
    private static readonly int StageCount = Enum.GetValues<DealStage>().Length;

    // Dwell-time bands, in days: a deal is Fresh through its first week, Stalling after it, and
    // Stale once it has sat for three weeks. AgeScaleDays is the full span of the dwell gauge, so
    // the two thresholds land at 8/30 and 21/30 of its width.
    private const int StallingFromDays = 8;
    private const int StaleFromDays = 21;
    private const int AgeScaleDays = 30;

    // A deal on its first day is 1/30th of the gauge — a sliver that reads as an empty track rather
    // than as a small amount of time. The fill starts at a visible minimum instead, still well short
    // of the first threshold notch at 8/30, so a fresh deal cannot be mistaken for a stalling one.
    private const double MinGaugeFill = 0.07;

    public bool IsWon => Stage == DealStage.ClosedWon;

    /// <summary>e.g. <c>$45,000</c>.</summary>
    public string AmountDisplay => Amount.ToString("C0", Usd);

    /// <summary>
    /// Right-aligned meta on a pipeline card: the age in days, or "Won" for closed deals. It reads
    /// as a card badge, not as a value for an "age" field — a detail view wants
    /// <see cref="AgeInStageDisplay"/> and <see cref="AgeBand"/> instead.
    /// </summary>
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

    /// <summary>Human-readable health label for a detail view.</summary>
    public string HealthDisplay => Health switch
    {
        DealHealth.AtRisk => "At risk",
        DealHealth.Watch => "Watch",
        _ => "Healthy",
    };

    /// <summary>1-based position of <see cref="Stage"/> in the pipeline, e.g. 4 for Negotiation.</summary>
    public int StageNumber => (int)Stage + 1;

    /// <summary>
    /// The stage stated as a position rather than a color, e.g. <c>STEP 4 OF 5</c> — so the stage
    /// still reads when the hue does not.
    /// </summary>
    public string StagePositionDisplay => $"STEP {StageNumber} OF {StageCount}";

    /// <summary>Dwell time in the current stage, e.g. <c>47 days</c>. Not used for closed deals.</summary>
    public string AgeInStageDisplay => AgeDays == 1 ? "1 day" : $"{AgeDays} days";

    /// <summary>Which staleness band <see cref="AgeDays"/> falls in; a closed deal is not tracked.</summary>
    public DealAgeBand AgeBand => IsWon
        ? DealAgeBand.NotTracked
        : AgeDays >= StaleFromDays
            ? DealAgeBand.Stale
            : AgeDays >= StallingFromDays
                ? DealAgeBand.Stalling
                : DealAgeBand.Fresh;

    /// <summary>Human-readable staleness band, e.g. "Stalling"; empty for a closed deal.</summary>
    public string AgeBandDisplay => AgeBand switch
    {
        DealAgeBand.Fresh => "Fresh",
        DealAgeBand.Stalling => "Stalling",
        DealAgeBand.Stale => "Stale",
        _ => string.Empty,
    };

    /// <summary>
    /// How full the dwell gauge is (0..1) across its <see cref="AgeScaleDays"/>-day span. A deal
    /// older than the span pins the gauge full — the day count next to it carries the real number.
    /// </summary>
    public double AgeProgress => Math.Clamp(AgeDays / (double)AgeScaleDays, MinGaugeFill, 1d);
}

/// <summary>A pipeline column: a stage plus the deals currently in it. Carries no presentation — each
/// column's dot and count-badge palette lives in that stage's own keyed header template, so the
/// brushes resolve against the element's theme rather than travelling as strings.</summary>
// NOTE — this record is a FEED VALUE type (see the Model that exposes it), and the MVUX generator
// emits a bindable proxy for every feed value type which it constructs with an object initializer.
// `required` members reject that, so the build fails inside the generated code. The members therefore
// carry defaults instead of `required`: the record stays default-constructible for the generator and
// non-null for the compiler, and every call site in CrmData still sets all of them explicitly.
public partial record PipelineStage
{
    public string Name { get; init; } = string.Empty;
    public DealStage Stage { get; init; }
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
    public required int Count { get; init; }
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
