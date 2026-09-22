namespace UnoCRM.Presentation.MockData;

/// <summary>
/// Design-time DataContext for <see cref="PipelinePage"/> in Hot Design / Studio, built to the
/// recipe documented on <see cref="ContactsPageMockData"/>. The page draws the same stages two ways —
/// a desktop board that indexes a scalar feed and a mobile list that takes the list form — so this
/// mirrors the Model and supplies both. The generated PipelineModel VM overrides this at runtime.
/// </summary>
[ReactiveBindable]
public partial record PipelinePageMockData
{
    /// <summary>The full board.</summary>
    public static PipelinePageMockDataViewModel Data => new();

    /// <summary>An empty board — the mobile list's NoneTemplate.</summary>
    public static PipelinePageMockDataViewModel EmptyBoard =>
        PipelinePageMockDataViewModel.ForModel(new()
        {
            Board = Feed.Async(_ => ValueTask.FromResult<IImmutableList<PipelineStage>>(ImmutableList<PipelineStage>.Empty)),
            Stages = ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<PipelineStage>>(ImmutableList<PipelineStage>.Empty)),
        });

    // Both arrangements' sources, mirroring the Model: the desktop board indexes the scalar feed,
    // the mobile list takes the list form. Each is an inline lambda that captures nothing, so all
    // instances share one feed — rule 4 of the recipe.
    public IFeed<IImmutableList<PipelineStage>> Board { get; init; } =
        Feed.Async(_ => ValueTask.FromResult<IImmutableList<PipelineStage>>(CrmData.Stages.ToImmutableList()));

    public IListFeed<PipelineStage> Stages { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<PipelineStage>>(CrmData.Stages.ToImmutableList()));

    // The filter bar. Settable strings rather than states: a preview only has to RENDER a selection,
    // and the vocabularies must be materialized or the ComboBoxes would drop it.
    public string SourceFilter { get; set; } = PipelineModel.AllSources;
    public string PeriodFilter { get; set; } = PipelineModel.ThisQuarter;
    public string RepFilter { get; set; } = PipelineModel.AllReps;

    public IReadOnlyList<string> Sources { get; } =
        new[] { PipelineModel.AllSources }
            .Concat(CrmData.Deals.Select(d => d.Source).Distinct(StringComparer.OrdinalIgnoreCase))
            .ToArray();

    public IReadOnlyList<string> Periods { get; } =
        [PipelineModel.ThisWeek, PipelineModel.ThisMonth, PipelineModel.ThisQuarter];

    public IReadOnlyList<string> Reps { get; } =
        new[] { PipelineModel.AllReps }
            .Concat(CrmData.Deals.Select(d => d.Owner).Distinct(StringComparer.OrdinalIgnoreCase))
            .ToArray();
}

// Reaches the generated ViewModel's protected model-taking constructor, so EmptyBoard can wrap a
// customized model. See ContactsPageMockDataViewModel for the full explanation.
public partial class PipelinePageMockDataViewModel
{
    internal static PipelinePageMockDataViewModel ForModel(PipelinePageMockData model) => new(model);
}
