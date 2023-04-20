using Uno.Extensions.Reactive;

namespace SelectionPeopleApp;

public partial record PeopleModel(IPeopleService PeopleService)
{
    public IListFeed<Person> People =>
        ListFeed
        .Async(PeopleService.GetPeopleAsync)
        .Selection(SelectedPerson);
        //.Selection(SelectedPeople);

    public IState<Person> SelectedPerson => State<Person>.Empty(this);

    public IState<IImmutableList<Person>> SelectedPeople => State<IImmutableList<Person>>.Empty(this);
}