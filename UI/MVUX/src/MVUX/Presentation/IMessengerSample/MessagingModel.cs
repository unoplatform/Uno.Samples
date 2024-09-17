using CommunityToolkit.Mvvm.Messaging;

namespace MVUX.Presentation.IMessengerSample;

public partial record MessagingModel
{
    protected IPeopleService PeopleService { get; }

    public MessagingModel(IPeopleService peopleService, IMessenger messenger)
    {
        PeopleService = peopleService;

        messenger.Observe(People, person => person.Id);
    }

    public IListState<Person> People =>
        ListState
        .Async(this, PeopleService.GetPeople)
        .Selection(SelectedPerson);

    public IState<Person> NewPerson => State<Person>.Value(this, Person.EmptyPerson);

    public IState<Person> SelectedPerson => State<Person>.Empty(this);


    public async ValueTask AddPerson(CancellationToken ct = default)
    {
        var newPerson = (await NewPerson)!;

        await PeopleService.AddPerson(newPerson, ct);

        await NewPerson.Update(old => Person.EmptyPerson(), ct);
    }

    public async ValueTask RemovePerson(Person person, CancellationToken ct = default)
    {
        var personId = person.Id;

        await PeopleService.RemovePerson(personId, ct);
    }
}
