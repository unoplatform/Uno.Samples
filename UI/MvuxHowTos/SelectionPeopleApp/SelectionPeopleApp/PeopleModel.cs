using Uno.Extensions.Reactive;

namespace SelectionPeopleApp;

public partial record PeopleModel(IPeopleService PeopleService)
{
    public IListFeed<Person> People => ListFeed.Async(PeopleService.GetPeopleAsync);

    /*
    public IListFeed<Person> People =>
        ListFeed
        .Async(PeopleService.GetPeopleAsync)
        .Selection(SelectedPeople);
    */

    //public IState<string> SelectedFirstName => State<string>.Empty(this);

    public IState<Person> SelectedPerson => State<Person>.Empty(this);

    public IState<IImmutableList<Person>> SelectedPeople => State<IImmutableList<Person>>.Empty(this);

    public IState<int> DesiredSize => State<int>.Value(this, () => 5);

    public IListFeed<Person> PaginatedPeople =
        ListFeed.AsyncPaginated(async (pageRequest, ct) =>
            await PeopleService.GetPeopleAsync(pageRequest.DesiredSize ?? 0, pageRequest.Index, ct));
}