using System.Collections.Immutable;
using CommunityToolkit.Mvvm.Messaging;

namespace MessagingPeopleApp;

public partial record PeopleModel
{
    protected IPeopleService PeopleService { get; }
    protected IPhoneService PhoneService { get; }

    public PeopleModel(IPeopleService peopleService, IPhoneService phoneService, IMessenger messenger)
    {
        PeopleService = peopleService;
        PhoneService = phoneService;

        messenger.Observe(People, person => person.Id);
        messenger.Observe(SelectedPersonPhones, SelectedPerson, (person, phones) => true, person => person.Id);
    }

    public IListState<Person> People =>
        ListState
        .Async(this, PeopleService.GetPeople)
        .Selection(SelectedPerson);

    public IState<Person> NewPerson => State<Person>.Value(this, Person.EmptyPerson);

    public IState<Person> SelectedPerson => State<Person>.Empty(this);

    public IListState<Phone> SelectedPersonPhones => ListState.FromFeed(this, SelectedPerson.SelectAsync(GetAllPhonesSafe).AsListFeed());

    private async ValueTask<IImmutableList<Phone>> GetAllPhonesSafe(Person selectedPerson, CancellationToken ct)
    {
        if (selectedPerson == null)
            return ImmutableList<Phone>.Empty;

        return await PhoneService.GetAllPhones(selectedPerson, ct);
    }

    public async ValueTask AddPerson(CancellationToken ct = default)
    {
        var newPerson = (await NewPerson)!;

        await PeopleService.AddPerson(newPerson, ct);

        await NewPerson.Update(old => Person.EmptyPerson(), ct);
    }

    public async ValueTask RemovePhone(int phoneId, CancellationToken ct)
    {
        await PhoneService.DeletePhoneAsync(phoneId, ct);
    }

    public async ValueTask RemovePerson(Person person, CancellationToken ct = default)
    {
        var personId = person.Id;

        await PeopleService.RemovePerson(personId, ct);
    }
}