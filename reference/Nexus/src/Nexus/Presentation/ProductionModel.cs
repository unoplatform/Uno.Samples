using Nexus.Services;
using Uno.Extensions.Reactive;

namespace Nexus.Presentation;

/// <summary>MVUX model for the Production page; all data sourced from <see cref="INexusService"/>.</summary>
public partial record ProductionModel(INexusService Nexus)
{
    public IListFeed<Batch> BatchQueue => ListFeed.Async(Nexus.GetBatchQueueAsync);

    public IFeed<ShiftProgress> Shift => Feed.Async(Nexus.GetShiftProgressAsync);

    public IListFeed<Material> Materials => ListFeed.Async(Nexus.GetMaterialsAsync);

    public IListFeed<ProductionLine> Lines => ListFeed.Async(Nexus.GetProductionLinesAsync);
}
