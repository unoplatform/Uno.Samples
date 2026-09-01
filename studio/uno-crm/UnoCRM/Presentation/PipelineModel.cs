using UnoCRM.Presentation.Services;

namespace UnoCRM.Presentation;

/// <summary>
/// Backs <see cref="PipelinePage"/>. The board comes from ONE request to <see cref="ICrmService"/>,
/// re-issued whenever a filter changes and cached so the desktop columns and the mobile list share it.
///
/// Two shapes of the same request, because the two arrangements need different things. The desktop
/// board is a fixed five-column grid, so it takes the SCALAR feed and indexes into the loaded list.
/// The mobile arrangement is a stacked list whose "no stages" state is a designed card, so it takes
/// the list form — where an empty result surfaces as None and selects that template.
/// </summary>
public partial record PipelineModel(ICrmService Crm)
{
    public const string AllSources = "All Sources";
    public const string AllReps = "All Reps";
    public const string ThisWeek = "This Week";
    public const string ThisMonth = "This Month";
    public const string ThisQuarter = "This Quarter";

    // Two-way mutable user input. One state each, bound by BOTH arrangements' controls, so the
    // desktop row and the mobile scroller stay in lockstep with no manual mirroring.
    public IState<string> SourceFilter => State.Value(this, () => AllSources);
    public IState<string> PeriodFilter => State.Value(this, () => ThisQuarter);
    public IState<string> RepFilter => State.Value(this, () => AllReps);

    // The filter VOCABULARIES, and the one part of this page that is deliberately NOT a feed.
    //
    // Each ComboBox binds SelectedItem TwoWay to a state above, whose default is the "All …"/period
    // sentinel. A ComboBox will not hold a SelectedItem that is absent from its ItemsSource, so if the
    // items arrive later — as they would from an async feed — the selection is dropped and the TwoWay
    // binding writes that empty selection back over the state. These lists are the affordance for
    // asking a question, not an answer that can be loading or failed, so they are materialized on the
    // first frame.
    public IReadOnlyList<string> Sources { get; } =
        new[] { AllSources }
            .Concat(CrmData.Deals.Select(d => d.Source).Distinct(StringComparer.OrdinalIgnoreCase))
            .ToArray();

    public IReadOnlyList<string> Periods { get; } = [ThisWeek, ThisMonth, ThisQuarter];

    public IReadOnlyList<string> Reps { get; } =
        new[] { AllReps }
            .Concat(CrmData.Deals.Select(d => d.Owner).Distinct(StringComparer.OrdinalIgnoreCase))
            .ToArray();

    // One board feed that both arrangements derive from, so the service is asked once per filter
    // change rather than once per consumer. Cached on first access so they share the same instance.
    private IFeed<IImmutableList<PipelineStage>>? _board;

    /// <summary>The whole board, for the desktop arrangement's five indexed columns.</summary>
    public IFeed<IImmutableList<PipelineStage>> Board =>
        _board ??= Feed
            .Combine(SourceFilter, PeriodFilter, RepFilter)
            .SelectAsync(async (criteria, ct) =>
                await Crm.GetPipelineStagesAsync(criteria.Item1, criteria.Item2, criteria.Item3, ct));

    /// <summary>The same stages as a list, for the mobile arrangement's FeedView.</summary>
    public IListFeed<PipelineStage> Stages => Board.AsListFeed();
}
