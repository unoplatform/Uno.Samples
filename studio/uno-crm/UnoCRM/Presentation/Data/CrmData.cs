using System.Globalization;

namespace UnoCRM.Presentation.Data;

/// <summary>
/// The single in-memory dataset shared by every page. Everything is deterministic and computed
/// once, so the Dashboard, Pipeline, Leads and Contacts pages all tell a consistent story (the same
/// deals, totals and stage counts) across visits — swap these builders out for a real API/data
/// layer to go live.
/// </summary>
public static class CrmData
{
    private static readonly CultureInfo Usd = CultureInfo.GetCultureInfo("en-US");

    // Declared before the Dashboard/Leads properties below: static members initialize in
    // declaration order, and the builders read these during that init. If they sat lower they'd
    // still be null when the builders run (a TypeInitializer crash on first access).

    // Stable monthly lead volume (Jan–Dec). Seeded once so the Leads charts don't reshuffle.
    private static readonly int[] MonthlyLeadSeed =
        [118, 132, 121, 145, 162, 158, 174, 169, 188, 196, 207, 172];

    private static readonly string[] SourceOrder = ["Web", "Email", "Referral", "Ads", "Event"];

    // Contact-generation inputs — declared before the Contacts property below, because static
    // members initialize in declaration order and BuildContacts() reads these during that init.
    // If they sat lower they'd still be null when BuildContacts runs (a TypeInitializer crash on
    // first access of CrmData).

    // A regional office: a city center plus how many contacts to scatter around it.
    private static readonly (string Region, string Segment, string City, double Lat, double Lon, int Count)[] Offices =
    [
        ("North America", "Enterprise", "Seattle", 47.6062, -122.3321, 8),
        ("North America", "Mid-Market", "New York", 40.7128, -74.0060, 8),
        ("North America", "SMB", "Toronto", 43.6532, -79.3832, 7),
        ("Europe", "Enterprise", "London", 51.5074, -0.1278, 8),
        ("Europe", "Mid-Market", "Berlin", 52.5200, 13.4050, 7),
        ("Europe", "SMB", "Madrid", 40.4168, -3.7038, 6),
        ("APAC", "Enterprise", "Singapore", 1.3521, 103.8198, 8),
        ("APAC", "Mid-Market", "Tokyo", 35.6762, 139.6503, 7),
        ("APAC", "SMB", "Sydney", -33.8688, 151.2093, 6),
        ("LATAM", "Enterprise", "Sao Paulo", -23.5505, -46.6333, 7),
        ("LATAM", "Mid-Market", "Mexico City", 19.4326, -99.1332, 7),
        ("LATAM", "SMB", "Bogota", 4.7110, -74.0721, 6),
    ];

    private static readonly string[] FirstNames =
        ["Alex", "Jordan", "Casey", "Morgan", "Taylor", "Jamie", "Riley", "Avery", "Cameron", "Harper", "Dakota", "Quinn", "Reese", "Skyler", "Logan"];
    private static readonly string[] LastNames =
        ["Chen", "Patel", "Garcia", "Kim", "Ross", "Rivera", "Brooks", "Nguyen", "Singh", "Wright", "Davis", "Lopez", "Adams", "Walker", "Wilson"];
    private static readonly string[] CompanyStarts =
        ["North", "Blue", "Summit", "Vertex", "Clear", "Nova", "Prime", "Vector", "Cloud", "Bright", "Atlas", "Global"];
    private static readonly string[] CompanyEnds =
        ["Analytics", "Systems", "Dynamics", "Logistics", "Health", "Finance", "Retail", "Labs", "Energy", "Media", "Networks", "Advisors"];

    public static IReadOnlyList<Deal> Deals { get; } = BuildDeals();

    public static IReadOnlyList<PipelineStage> Stages { get; } = BuildStages();

    public static DashboardData Dashboard { get; } = BuildDashboard();

    public static LeadsAnalytics Leads { get; } = BuildLeads();

    public static IReadOnlyList<ContactLocation> Contacts { get; } = BuildContacts();

    private static IReadOnlyList<Deal> BuildDeals() =>
    [
        // New Lead
        new("d-01", "Acme Corp", 45_000m, "Sarah Chen", DealStage.NewLead, DealHealth.AtRisk, 2, "Web"),
        new("d-02", "TechVision Inc", 28_500m, "Mike Johnson", DealStage.NewLead, DealHealth.Watch, 5, "Email"),
        new("d-03", "Bright Solutions", 12_000m, "Lisa Park", DealStage.NewLead, DealHealth.Healthy, 1, "Referral"),
        new("d-04", "DataFlow Ltd", 67_200m, "James Wright", DealStage.NewLead, DealHealth.Watch, 3, "Ads"),

        // Qualified
        new("d-05", "NovaTech", 89_000m, "David Kim", DealStage.Qualified, DealHealth.AtRisk, 8, "Referral"),
        new("d-06", "CloudSync", 34_750m, "Anna Lopez", DealStage.Qualified, DealHealth.Healthy, 12, "Web"),
        new("d-07", "Pinnacle Group", 52_300m, "Tom Rivera", DealStage.Qualified, DealHealth.Watch, 6, "Event"),

        // Proposal
        new("d-08", "Meridian Health", 124_000m, "Rachel Adams", DealStage.Proposal, DealHealth.AtRisk, 15, "Email"),
        new("d-09", "UrbanEdge", 56_800m, "Chris Taylor", DealStage.Proposal, DealHealth.Watch, 9, "Web"),
        new("d-10", "BluePeak Inc", 41_500m, "Maya Patel", DealStage.Proposal, DealHealth.Healthy, 4, "Ads"),

        // Negotiation
        new("d-11", "Vertex Labs", 210_000m, "Kevin Zhang", DealStage.Negotiation, DealHealth.AtRisk, 22, "Referral"),
        new("d-12", "Orion Systems", 73_400m, "Elena Ross", DealStage.Negotiation, DealHealth.Watch, 18, "Event"),
        new("d-13", "Atlas Financial", 95_600m, "Nina Brooks", DealStage.Negotiation, DealHealth.Healthy, 11, "Web"),

        // Closed Won
        new("d-14", "Summit Retail", 156_000m, "Paul Martinez", DealStage.ClosedWon, DealHealth.Healthy, 0, "Email"),
        new("d-15", "Crest Dynamics", 82_500m, "Jenna Cole", DealStage.ClosedWon, DealHealth.Healthy, 0, "Web"),
        new("d-16", "Forge Media", 47_200m, "Leo Harris", DealStage.ClosedWon, DealHealth.Healthy, 0, "Referral"),
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
            .Select((stage, i) => new FunnelStage
            {
                Name = funnelNames[i],
                Count = stage.Count,
                FillKey = stage.AccentKey,
                FillFraction = maxStageCount == 0 ? 0d : (double)stage.Count / maxStageCount,
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
            "DashboardGreenBrush", "DashboardGreenSoftBrush", ""),
        new("New lead assigned — Acme Corp", "Sarah Chen  •  Web", "15m ago",
            "DashboardBlueBrush", "DashboardBlueSoftBrush", ""),
        new("Proposal sent — Meridian Health", "$124,000  •  Proposal", "1h ago",
            "DashboardAmberBrush", "DashboardAmberSoftBrush", ""),
        new("Lead qualified — NovaTech", "David Kim  •  Referral", "2h ago",
            "DashboardPurpleBrush", "DashboardPurpleSoftBrush", ""),
        new("Negotiation started — Vertex Labs", "$210,000  •  Negotiation", "3h ago",
            "DashboardRedBrush", "DashboardRedSoftBrush", ""),
    ];

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

    /// <summary>
    /// Builds a stable set of contacts scattered deterministically around each regional office —
    /// fixed per-index lat/lon offsets and index-selected names/companies, so the map and list are
    /// identical on every visit (no <c>Random</c>).
    /// </summary>
    private static IReadOnlyList<ContactLocation> BuildContacts()
    {
        var contacts = new List<ContactLocation>();
        var n = 0;

        for (var o = 0; o < Offices.Length; o++)
        {
            var office = Offices[o];
            for (var i = 0; i < office.Count; i++)
            {
                // Deterministic scatter: spread points on a small lat/lon lattice around the center.
                var latOffset = ((i % 4) - 1.5) * 0.42;
                var lonOffset = (((i * 3) % 5) - 2) * 0.42;

                var name = $"{FirstNames[(o * 3 + i * 2) % FirstNames.Length]} {LastNames[(o * 5 + i * 3) % LastNames.Length]}";
                var company = $"{CompanyStarts[(o + i) % CompanyStarts.Length]} {CompanyEnds[(o * 2 + i) % CompanyEnds.Length]}";

                contacts.Add(new ContactLocation(
                    Id: $"c-{++n:D3}",
                    Name: name,
                    Company: company,
                    City: office.City,
                    Region: office.Region,
                    Segment: office.Segment,
                    Latitude: office.Lat + latOffset,
                    Longitude: office.Lon + lonOffset));
            }
        }

        return contacts;
    }

    /// <summary>Formats a money value compactly, e.g. <c>$930K</c> or <c>$1.2M</c>.</summary>
    private static string ToShortMoney(decimal amount) => amount switch
    {
        >= 1_000_000m => $"${amount / 1_000_000m:0.#}M",
        >= 1_000m => $"${amount / 1_000m:N0}K",
        _ => amount.ToString("C0", Usd),
    };
}
