using System.Globalization;
using Microsoft.UI.Xaml;

namespace UnoCRM;

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

/// <summary>A single sales deal. This is the atomic record the whole sample is built from.</summary>
public sealed record Deal(
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

    /// <summary>Resource key for the card's accent dot / check — green when won, else by health.</summary>
    public string AccentKey => IsWon
        ? "DashboardGreenBrush"
        : Health switch
        {
            DealHealth.AtRisk => "DashboardRedBrush",
            DealHealth.Watch => "DashboardAmberBrush",
            _ => "DashboardGreenBrush",
        };

    /// <summary>Resource key for the meta text — green/bold for won deals, muted otherwise.</summary>
    public string MetaKey => IsWon ? "DashboardGreenBrush" : "DashboardMutedTextBrush";
}

/// <summary>A pipeline column: a stage plus the deals currently in it, with its palette keys.</summary>
public sealed class PipelineStage
{
    public required string Name { get; init; }
    public required DealStage Stage { get; init; }
    public required string AccentKey { get; init; }
    public required string SoftKey { get; init; }
    public required IReadOnlyList<Deal> Deals { get; init; }

    public int Count => Deals.Count;
}

/// <summary>One bar in the Dashboard "Pipeline Summary": a stage label, its count and bar fill.</summary>
public sealed class FunnelStage
{
    public required string Name { get; init; }
    public required int Count { get; init; }
    public required string FillKey { get; init; }

    /// <summary>Star width of the filled portion of the track.</summary>
    public required GridLength FillColumn { get; init; }

    /// <summary>Star width of the remaining (empty) portion of the track.</summary>
    public required GridLength RemainderColumn { get; init; }
}

/// <summary>A row in the Dashboard "Recent Activity" feed.</summary>
public sealed record ActivityItem(
    string Title,
    string Detail,
    string TimeAgo,
    string AccentKey,
    string SoftKey,
    string Glyph);

/// <summary>A row in the Leads "Top Open Leads" list.</summary>
public sealed record TopLead(string Company, string AmountDisplay);

/// <summary>Everything the Dashboard page binds to, derived once from <see cref="CrmData.Deals"/>.</summary>
public sealed class DashboardData
{
    public required string TotalLeadsText { get; init; }
    public required string TotalLeadsDelta { get; init; }
    public required string ActiveDealsText { get; init; }
    public required string ActiveDealsDelta { get; init; }
    public required string RevenueText { get; init; }
    public required string RevenueDelta { get; init; }
    public required string ConversionRateText { get; init; }
    public required string ConversionRateDelta { get; init; }
    public required IReadOnlyList<FunnelStage> Funnel { get; init; }
    public required IReadOnlyList<ActivityItem> Activities { get; init; }
}

/// <summary>The Pipeline page's binding context: the five stage columns.</summary>
public sealed class PipelineData
{
    public required IReadOnlyList<PipelineStage> Stages { get; init; }

    public PipelineStage NewLead => Stages[0];
    public PipelineStage Qualified => Stages[1];
    public PipelineStage Proposal => Stages[2];
    public PipelineStage Negotiation => Stages[3];
    public PipelineStage ClosedWon => Stages[4];
}

/// <summary>
/// The numbers behind the Leads charts and KPIs. Chart <c>ISeries</c>/<c>Axis</c> objects are
/// still assembled in <see cref="LeadsPage"/> (they need SkiaSharp paints), but every value
/// they plot comes from here so the page is stable and consistent with the rest of the app.
/// </summary>
public sealed class LeadsAnalytics
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

/// <summary>
/// The single in-memory dataset shared by every page. Everything is deterministic and computed
/// once, so the Dashboard, Pipeline and Leads pages all tell a consistent story (the same deals,
/// totals and stage counts) instead of each inventing its own hardcoded numbers.
/// </summary>
public static class CrmData
{
    private static readonly CultureInfo Usd = CultureInfo.GetCultureInfo("en-US");

    public static IReadOnlyList<Deal> Deals { get; } = BuildDeals();

    public static IReadOnlyList<PipelineStage> Stages { get; } = BuildStages();

    public static PipelineData Pipeline { get; } = new() { Stages = Stages };

    public static DashboardData Dashboard { get; } = BuildDashboard();

    public static LeadsAnalytics Leads { get; } = BuildLeads();

    private static IReadOnlyList<Deal> BuildDeals() =>
    [
        // New Lead
        new("Acme Corp", 45_000m, "Sarah Chen", DealStage.NewLead, DealHealth.AtRisk, 2, "Web"),
        new("TechVision Inc", 28_500m, "Mike Johnson", DealStage.NewLead, DealHealth.Watch, 5, "Email"),
        new("Bright Solutions", 12_000m, "Lisa Park", DealStage.NewLead, DealHealth.Healthy, 1, "Referral"),
        new("DataFlow Ltd", 67_200m, "James Wright", DealStage.NewLead, DealHealth.Watch, 3, "Ads"),

        // Qualified
        new("NovaTech", 89_000m, "David Kim", DealStage.Qualified, DealHealth.AtRisk, 8, "Referral"),
        new("CloudSync", 34_750m, "Anna Lopez", DealStage.Qualified, DealHealth.Healthy, 12, "Web"),
        new("Pinnacle Group", 52_300m, "Tom Rivera", DealStage.Qualified, DealHealth.Watch, 6, "Event"),

        // Proposal
        new("Meridian Health", 124_000m, "Rachel Adams", DealStage.Proposal, DealHealth.AtRisk, 15, "Email"),
        new("UrbanEdge", 56_800m, "Chris Taylor", DealStage.Proposal, DealHealth.Watch, 9, "Web"),
        new("BluePeak Inc", 41_500m, "Maya Patel", DealStage.Proposal, DealHealth.Healthy, 4, "Ads"),

        // Negotiation
        new("Vertex Labs", 210_000m, "Kevin Zhang", DealStage.Negotiation, DealHealth.AtRisk, 22, "Referral"),
        new("Orion Systems", 73_400m, "Elena Ross", DealStage.Negotiation, DealHealth.Watch, 18, "Event"),
        new("Atlas Financial", 95_600m, "Nina Brooks", DealStage.Negotiation, DealHealth.Healthy, 11, "Web"),

        // Closed Won
        new("Summit Retail", 156_000m, "Paul Martinez", DealStage.ClosedWon, DealHealth.Healthy, 0, "Email"),
        new("Crest Dynamics", 82_500m, "Jenna Cole", DealStage.ClosedWon, DealHealth.Healthy, 0, "Web"),
        new("Forge Media", 47_200m, "Leo Harris", DealStage.ClosedWon, DealHealth.Healthy, 0, "Referral"),
    ];

    private static IReadOnlyList<PipelineStage> BuildStages()
    {
        (DealStage Stage, string Name, string Accent, string Soft)[] defs =
        [
            (DealStage.NewLead, "NEW LEAD", "DashboardBlueBrush", "DashboardBlueSoftBrush"),
            (DealStage.Qualified, "QUALIFIED", "DashboardPurpleBrush", "DashboardPurpleSoftBrush"),
            (DealStage.Proposal, "PROPOSAL", "DashboardAmberBrush", "DashboardAmberSoftBrush"),
            (DealStage.Negotiation, "NEGOTIATION", "DashboardRedBrush", "DashboardRedSoftBrush"),
            (DealStage.ClosedWon, "CLOSED WON", "DashboardGreenBrush", "DashboardGreenSoftBrush"),
        ];

        return defs
            .Select(d => new PipelineStage
            {
                Name = d.Name,
                Stage = d.Stage,
                AccentKey = d.Accent,
                SoftKey = d.Soft,
                Deals = Deals.Where(deal => deal.Stage == d.Stage).ToList(),
            })
            .ToList();
    }

    private static DashboardData BuildDashboard()
    {
        var openDeals = Deals.Where(d => !d.IsWon).ToList();
        var wonDeals = Deals.Where(d => d.IsWon).ToList();
        var monthlyTotal = MonthlyLeadSeed.Sum();
        var conversion = (double)wonDeals.Count / Deals.Count;
        var maxStageCount = Stages.Max(s => s.Count);

        string[] funnelNames = ["New Lead", "Qualified", "Proposal", "Negotiation", "Closed Won"];

        var funnel = Stages
            .Select((stage, i) =>
            {
                var fraction = maxStageCount == 0 ? 0d : (double)stage.Count / maxStageCount;
                return new FunnelStage
                {
                    Name = funnelNames[i],
                    Count = stage.Count,
                    FillKey = stage.AccentKey,
                    FillColumn = new GridLength(fraction, GridUnitType.Star),
                    RemainderColumn = new GridLength(1 - fraction, GridUnitType.Star),
                };
            })
            .ToList();

        return new DashboardData
        {
            TotalLeadsText = monthlyTotal.ToString("N0", Usd),
            TotalLeadsDelta = "+12.5%",
            ActiveDealsText = openDeals.Count.ToString("N0", Usd),
            ActiveDealsDelta = "+8.3%",
            RevenueText = ToShortMoney(wonDeals.Sum(d => d.Amount)),
            RevenueDelta = "+18.2%",
            ConversionRateText = conversion.ToString("P1", Usd),
            ConversionRateDelta = "+3.1%",
            Funnel = funnel,
            Activities = BuildActivities(),
        };
    }

    private static IReadOnlyList<ActivityItem> BuildActivities() =>
    [
        new("Deal won — Summit Retail", "$156,000  •  Closed Won", "2h ago",
            "DashboardGreenBrush", "DashboardGreenSoftBrush", ""),
        new("New lead assigned — Acme Corp", "Sarah Chen  •  Web", "15m ago",
            "DashboardBlueBrush", "DashboardBlueSoftBrush", ""),
        new("Proposal sent — Meridian Health", "$124,000  •  Proposal", "1h ago",
            "DashboardAmberBrush", "DashboardAmberSoftBrush", ""),
        new("Lead qualified — NovaTech", "David Kim  •  Referral", "2h ago",
            "DashboardPurpleBrush", "DashboardPurpleSoftBrush", ""),
        new("Negotiation started — Vertex Labs", "$210,000  •  Negotiation", "3h ago",
            "DashboardRedBrush", "DashboardRedSoftBrush", ""),
    ];

    // Stable monthly lead volume (Jan–Dec). Seeded once so the Leads charts no longer reshuffle
    // on every visit the way the previous Random-based data did.
    private static readonly int[] MonthlyLeadSeed =
        [118, 132, 121, 145, 162, 158, 174, 169, 188, 196, 207, 172];

    private static readonly string[] SourceOrder = ["Web", "Email", "Referral", "Ads", "Event"];

    private static LeadsAnalytics BuildLeads()
    {
        var sourceCounts = SourceOrder
            .Select(source => Deals.Count(d => d.Source == source))
            .ToArray();

        var stageCounts = Stages.Select(s => s.Count).ToArray();
        string[] stageLabels = ["New Lead", "Qualified", "Proposal", "Negotiation", "Closed Won"];

        var qualifiedOrBeyond = Deals.Count(d => d.Stage != DealStage.NewLead);
        var qualificationRate = (double)qualifiedOrBeyond / Deals.Count;
        var openValue = Deals.Where(d => !d.IsWon).Sum(d => d.Amount);
        var avgDeal = Deals.Average(d => d.Amount);

        var topOpen = Deals
            .Where(d => !d.IsWon)
            .OrderByDescending(d => d.Amount)
            .Take(4)
            .Select(d => new TopLead(d.Company, d.AmountDisplay))
            .ToList();

        return new LeadsAnalytics
        {
            NewLeadsText = MonthlyLeadSeed.Sum().ToString("N0", Usd),
            QualificationRateText = qualificationRate.ToString("P0", Usd),
            PipelineValueText = ToShortMoney(openValue),
            AverageDealSizeText = ToShortMoney(avgDeal),
            MonthLabels = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
            MonthlyLeads = MonthlyLeadSeed,
            SourceLabels = SourceOrder,
            SourceCounts = sourceCounts,
            StageLabels = stageLabels,
            StageCounts = stageCounts,
            TopOpenLeads = topOpen,
        };
    }

    /// <summary>Formats a money value compactly, e.g. <c>$930K</c> or <c>$1.2M</c>.</summary>
    private static string ToShortMoney(decimal amount) => amount switch
    {
        >= 1_000_000m => $"${amount / 1_000_000m:0.#}M",
        >= 1_000m => $"${amount / 1_000m:N0}K",
        _ => amount.ToString("C0", Usd),
    };
}
