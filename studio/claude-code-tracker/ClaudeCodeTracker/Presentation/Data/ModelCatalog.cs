namespace ClaudeCodeTracker.Presentation.Data;

/// <summary>
/// The Claude model price list (USD per 1M tokens). Single source of truth for the Usage
/// page pricing table and for deriving each session's per-token-type cost on the detail page.
/// </summary>
public static class ModelCatalog
{
    public static IReadOnlyList<ModelInfo> All { get; } = new[]
    {
        new ModelInfo("claude-opus-4-5", "Claude Opus 4.5", "Opus", 15.00m, 75.00m, 1.50m, 3.75m),
        new ModelInfo("claude-sonnet-4-5", "Claude Sonnet 4.5", "Sonnet", 3.00m, 15.00m, 0.30m, 0.75m),
        new ModelInfo("claude-haiku-3-5", "Claude Haiku 3.5", "Haiku", 0.80m, 4.00m, 0.08m, 0.20m),
    };

    /// <summary>Pricing for a model id, falling back to the cheapest tier for unknown ids.</summary>
    public static ModelInfo Pricing(string modelId) =>
        All.FirstOrDefault(m => m.ModelId == modelId) ?? All[^1];
}
