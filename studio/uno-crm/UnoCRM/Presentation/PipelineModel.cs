using UnoCRM.Presentation.Services;

namespace UnoCRM.Presentation;

/// <summary>
/// Backs <see cref="PipelinePage"/>. The five stage columns come from ONE request to
/// <see cref="ICrmService"/>, cached so the desktop board and the mobile list share it and the page
/// never fetches the board twice.
///
/// Two shapes of the same request, because the two arrangements need different things. The desktop
/// board is a fixed five-column grid, so it takes the SCALAR feed and indexes into the loaded list.
/// The mobile arrangement is a stacked list whose "no stages" state is a designed card, so it takes
/// the list form — where an empty result surfaces as None and selects that template.
/// </summary>
public partial record PipelineModel(ICrmService Crm)
{
    private IFeed<IImmutableList<PipelineStage>>? _board;

    /// <summary>The whole board, for the desktop arrangement's five indexed columns.</summary>
    public IFeed<IImmutableList<PipelineStage>> Board =>
        _board ??= Feed.Async(Crm.GetPipelineStagesAsync);

    /// <summary>The same stages as a list, for the mobile arrangement's FeedView.</summary>
    public IListFeed<PipelineStage> Stages => Board.AsListFeed();
}
