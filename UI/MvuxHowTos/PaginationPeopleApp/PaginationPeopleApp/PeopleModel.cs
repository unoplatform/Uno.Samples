using Uno.Extensions.Reactive;

namespace PaginationPeopleApp;

public partial record PeopleModel(IPeopleService PeopleService)
{
    const int PageSize = 20;

    public IListFeed<Person> PeopleAuto =>
        ListFeed.AsyncPaginated(async (pageRequest, ct) =>
            await PeopleService.GetPeopleAsync(pageSize: PageSize, pageIndex: pageRequest.Index, ct));

    public IFeed<int> PageCount =>
        Feed.Async(async (ct) => await PeopleService.GetPageCount(PageSize, ct));

    public IState<uint> CurrentPage => State.Value(this, () => 0u);

    public IListFeed<Person> PeopleManual =>
        CurrentPage.SelectAsync(async (pageIndex, ct) =>
            await PeopleService.GetPeopleAsync(pageSize: PageSize, pageIndex: pageIndex, ct))
        .AsListFeed();

    public async ValueTask Move(int value, CancellationToken ct)
    {
        var currentPage = await CurrentPage;
        var desiredPage = currentPage + value;

        if (desiredPage < 0 || desiredPage >= await PageCount)
            return;

        await CurrentPage.Set((uint)desiredPage, ct);
    }
}