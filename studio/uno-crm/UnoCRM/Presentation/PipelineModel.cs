namespace UnoCRM.Presentation;

/// <summary>
/// Backs <see cref="PipelinePage"/>. A read-only projection of the five pipeline stage columns from
/// the shared dataset, so the board always agrees with the Dashboard and Leads pages. The named
/// stage accessors let the desktop board bind each column without index magic. No reactive members,
/// so it opts out of the bindable generator.
/// </summary>
[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record PipelineModel
{
    public IReadOnlyList<PipelineStage> Stages => CrmData.Stages;

    public PipelineStage NewLead => Stages[0];
    public PipelineStage Qualified => Stages[1];
    public PipelineStage Proposal => Stages[2];
    public PipelineStage Negotiation => Stages[3];
    public PipelineStage ClosedWon => Stages[4];
}
