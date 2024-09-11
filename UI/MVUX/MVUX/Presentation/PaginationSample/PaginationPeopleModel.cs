using Uno.Extensions.Reactive;
using Uno.Extensions.Reactive.Sources;

namespace MVUX.Presentation.PaginationSample;

public partial record PaginationPeopleModel(IPaginationPeopleService PeopleService)
{
	const uint DefaultPageSize = 20;

	public IListFeed<Person> PeopleAuto =>
		ListFeed.AsyncPaginated(async (PageRequest pageRequest, CancellationToken ct) =>
			await PeopleService.GetPeopleAsync(pageSize: pageRequest.DesiredSize ?? DefaultPageSize, firstItemIndex: pageRequest.CurrentCount, ct));

	public IFeed<uint> PageCount =>
		Feed.Async(async (ct) => await PeopleService.GetPageCount(DefaultPageSize, ct));

	public IState<uint> CurrentPage => State.Value(this, () => 1u);

	public IListFeed<Person> PeopleManual =>
		CurrentPage.SelectAsync(async (currentPage, ct) =>
			await PeopleService.GetPeopleAsync(
				pageSize: DefaultPageSize,
				// currentPage argument as index based - subtracting 1
				firstItemIndex: (currentPage - 1) * DefaultPageSize, ct))
		.AsListFeed();

	public IListFeed<Person> PeopleCursor =>
		ListFeed<Person>.AsyncPaginatedByCursor(
			// starting off with a blank Person, since the person list is to be ordered by name, any valid name will follow.
			firstPage: default(int?),
			// this will be automatically invoked by the ISupportIncrementalLoading the ListView supports
			getPage: async (cursor, desiredPageSize, ct) =>
			{
				var result = await PeopleService.GetPeopleAsync(cursor, desiredPageSize ?? DefaultPageSize, ct);
				return new PageResult<int?, Person>(result.CurrentPage, result.NextPersonIdCursor);
			});
}
