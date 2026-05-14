using Uno.Extensions.Reactive;

namespace GridWatch.Presentation;

public partial record GridMapModel(IGridDataService DataService)
{
	public IListFeed<FacilityRow> FacilityRows => ListFeed.Async(DataService.GetFacilityRowsAsync);

	public IFeed<StatusCounts> Counts => FacilityRows.AsFeed().Select(list =>
	{
		int online = list.Count(f => f.Status == FacilityStatus.Online);
		int warning = list.Count(f => f.Status == FacilityStatus.Warning);
		int critical = list.Count(f => f.Status == FacilityStatus.Critical);
		return new StatusCounts(online, warning, critical, list.Count);
	});
}
