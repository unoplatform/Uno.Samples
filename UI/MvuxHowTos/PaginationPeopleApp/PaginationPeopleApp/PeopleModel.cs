using Uno.Extensions.Reactive;

namespace PaginationPeopleApp;

public partial record PeopleModel(IPeopleService PeopleService)
{
    const int PageSize = 20;

    public IListFeed<Person> PeopleAuto =>
        ListFeed.AsyncPaginated(async (PageRequest pageRequest, CancellationToken ct) =>
            await PeopleService.GetPeopleAsync(pageSize: PageSize, pageIndex: pageRequest.Index, ct));

    public IFeed<int> PageCount =>
        Feed.Async(async (ct) => await PeopleService.GetPageCount(PageSize, ct));

    public IState<uint> CurrentPage => State.Value(this, () => 1u);

    public IListFeed<Person> PeopleManual =>
        CurrentPage.SelectAsync(async (currentPage, ct) =>
            // currentPage argument as index based - subtracting 1
            await PeopleService.GetPeopleAsync(pageSize: PageSize, pageIndex: currentPage - 1, ct))
        .AsListFeed();

    public async ValueTask Move(int direction, CancellationToken ct)
    {
        var currentPage = await CurrentPage;
        var desiredPage = currentPage + direction;

        if (desiredPage <= 0 || desiredPage > await PageCount)
            return;

        await CurrentPage.Set((uint)desiredPage, ct);
    }
}