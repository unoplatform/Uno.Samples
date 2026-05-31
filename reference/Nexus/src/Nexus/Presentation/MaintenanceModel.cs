using Nexus.Services;
using Uno.Extensions.Reactive;

namespace Nexus.Presentation;

/// <summary>MVUX model for the Maintenance page; all data sourced from <see cref="INexusService"/>.</summary>
public partial record MaintenanceModel(INexusService Nexus)
{
    public IListFeed<WorkOrder> WorkOrders => ListFeed.Async(Nexus.GetWorkOrdersAsync);

    public IListFeed<SparePart> SpareParts => ListFeed.Async(Nexus.GetSparePartsAsync);

    public IListFeed<Equipment> Equipment => ListFeed.Async(Nexus.GetEquipmentAsync);
}
