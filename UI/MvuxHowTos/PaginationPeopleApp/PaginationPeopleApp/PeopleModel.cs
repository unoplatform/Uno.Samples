using Uno.Extensions.Reactive;

namespace PaginationPeopleApp;

public partial record PeopleModel(IPeopleService PeopleService)
{
    public IListFeed<Person> People =>
        ListFeed.AsyncPaginated(async (pageRequest, ct) =>
            await PeopleService.GetPeopleAsync(pageSize: 5, pageIndex: pageRequest.Index, ct));    

    public ValueTask MovePage(int direction, CancellationToken ct)
    {
        return ValueTask.CompletedTask;
    }
}