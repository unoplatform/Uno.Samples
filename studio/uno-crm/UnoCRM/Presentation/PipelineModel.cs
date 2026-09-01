using UnoCRM.Presentation.Services;

namespace UnoCRM.Presentation;

/// <summary>
/// Backs <see cref="PipelinePage"/>. The five stage columns come from ONE request to
/// <see cref="ICrmService"/>, cached so the desktop board and the mobile list share it.
///
/// The named accessors are scalar projections rather than indexes into a list feed, which keeps the
/// desktop board's <c>{Binding NewLead.Count}</c> / <c>{Binding NewLead.Deals}</c> bindings working
/// unchanged: the generated ViewModel materializes each one to a <see cref="PipelineStage"/>. They
/// use a plain <c>Select</c> safely because <see cref="AllStages"/> is a SCALAR feed — an empty list
/// inside it is still Data, not None (lesson 94 applies to the list-feed form, not this one) — and
/// <c>ElementAtOrDefault</c> guards the index regardless.
/// </summary>
public partial record PipelineModel(ICrmService Crm)
{
    private IFeed<IImmutableList<PipelineStage>>? _allStages;
    private IFeed<IImmutableList<PipelineStage>> AllStages =>
        _allStages ??= Feed.Async(Crm.GetPipelineStagesAsync);

    /// <summary>The whole board as a list, for the mobile arrangement's FeedView.</summary>
    public IListFeed<PipelineStage> Stages => AllStages.AsListFeed();

    public IFeed<PipelineStage> NewLead => Column(0);
    public IFeed<PipelineStage> Qualified => Column(1);
    public IFeed<PipelineStage> Proposal => Column(2);
    public IFeed<PipelineStage> Negotiation => Column(3);
    public IFeed<PipelineStage> ClosedWon => Column(4);

    // Feed.Select constrains TResult to notnull, and its nullable overload takes a Nullable<T> —
    // value types only — so a reference-typed column cannot be projected as null. A defensive empty
    // stage stands in instead: it renders a blank column rather than throwing, and the real service
    // always returns all five.
    private static readonly PipelineStage MissingColumn = new();

    private IFeed<PipelineStage> Column(int index) =>
        AllStages.Select(stages => stages.Count > index ? stages[index] : MissingColumn);
}
