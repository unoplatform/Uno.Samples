namespace ClaudeCodeTracker.Presentation.MockData;

// Design-time data for the Hot Design *component* previews — the individual entities each keyed
// DataTemplate renders. Referenced from the preview XAML via {x:Bind}, so no preview needs a
// code-behind override.
//
// Separate from the *PageMockData records next door, and deliberately simpler: a card template binds
// one entity, so there is no feed here and nothing for a SourceContext to pump. The page mocks are
// the ones that have to be [ReactiveBindable] and hand out a generated ViewModel.
//
// Where a card has more than one meaningful look, there is one entity per look rather than one
// entity and a note — a preview can only show what it is given.
public static class PreviewData
{
    /// <summary>The busiest session in the seed: an Opus run, the widest cost and duration.</summary>
    public static SessionEntry Session { get; } = SampleData.Sessions[0];

    /// <summary>A cheap, short Haiku session — the other end of the range the row has to lay out.</summary>
    public static SessionEntry BriefSession { get; } =
        SampleData.Sessions.First(s => s.ModelDisplayName.Contains("Haiku"));

    /// <summary>The top model by spend, so its share bar is near full.</summary>
    public static ModelUsageBreakdown ModelUsage { get; } = SampleData.ModelBreakdown[0];

    /// <summary>The smallest share, where the bar is a sliver and the label still has to read.</summary>
    public static ModelUsageBreakdown MinorModelUsage { get; } =
        SampleData.ModelBreakdown[SampleData.ModelBreakdown.Count - 1];

    /// <summary>A part-consumed rate limit.</summary>
    public static RateLimitInfo RateLimit { get; } = SampleData.RateLimits[0];

    /// <summary>A limit at the top of its window — the state the card exists to warn about.</summary>
    public static RateLimitInfo NearLimit { get; } =
        new("Tokens / min", 396_000, 400_000, 99.0, "Resets in 41s", "Token throughput in the last 60 seconds");

    /// <summary>A row of the model price list.</summary>
    public static ModelInfo ModelPricing { get; } = ModelCatalog.All[0];

    /// <summary>A chart legend row: swatch, label and value.</summary>
    public static LegendEntry ChartLegend { get; } =
        new("Input", "2,318,400 · 60.3%", Windows.UI.Color.FromArgb(255, 0xE8, 0x8A, 0x3C));

    /// <summary>A topic tag chip.</summary>
    public static string TopicTag { get; } = "refactoring";
}
