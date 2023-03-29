using Uno.Extensions.Reactive;

namespace PeopleApp;

public partial record PeopleModel(PeopleService PeopleService)
{
    public IListFeed<Person> People => ListFeed.Async(PeopleService.GetPeopleAsync);
}