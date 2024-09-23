using CommunityToolkit.Mvvm.Messaging;

namespace MVUX.Presentation.IMessengerSample;

public partial record MessagingModel(IPeopleService PeopleService, IMessenger Messenger)
{
	public IListState<Person> People => ListState
		.Async(this, PeopleService.GetPeople)
		.Selection(SelectedPerson)
		.Observe(Messenger, person => person.Id);

	public IState<Person> NewPerson => State<Person>.Empty(this);

    public IState<Person> SelectedPerson => State<Person>.Empty(this);


    public async ValueTask AddPerson(CancellationToken ct = default)
    {
        var newPerson = (await NewPerson)!;

        await PeopleService.AddPerson(newPerson, ct);

        await NewPerson.Update(old => Person.Empty, ct);
    }

    public async ValueTask RemovePerson(Person person, CancellationToken ct = default)
    {
        var personId = person.Id;

        await PeopleService.RemovePerson(personId, ct);
    }
}
