using CommunityToolkit.Mvvm.Messaging;

namespace MessagingPeopleApp;

public partial record PeopleModel
{
    private readonly IPeopleService _peopleService;

    public PeopleModel(IPeopleService peopleService, IMessenger messenger)
    {
        _peopleService = peopleService;

        messenger.Observe(People, person => person.Id);        
    }

    public IListState<Person> People => ListState.Async(this, _peopleService.GetPeople);

    public IState<Person> NewPerson => State<Person>.Value(this, Person.EmptyPerson);

    public async ValueTask AddPerson(CancellationToken ct = default)
    {
        var newPerson = (await NewPerson)!;

        await _peopleService.AddPerson(newPerson, ct);

        await NewPerson.Update(old => Person.EmptyPerson(), ct);
    }

    public async ValueTask RemovePerson(Person person, CancellationToken ct = default)
    {
        var personId = person.Id;

        await _peopleService.RemovePerson(personId, ct);
    }
}