using CommunityToolkit.Mvvm.Messaging;

namespace MVUX.Presentation.IMessengerSample;

public partial record Person(int Id, string FirstName, string LastName)
{
    public static Person EmptyPerson() =>
      new Person(Id: default, FirstName: string.Empty, LastName: string.Empty);
}

public interface IPeopleService
{
    ValueTask<IImmutableList<Person>> GetPeople(CancellationToken ct = default);

    ValueTask AddPerson(Person person, CancellationToken ct = default);

    ValueTask RemovePerson(int personId, CancellationToken ct = default);
}

public class PeopleService : IPeopleService
{
    protected IMessenger Messenger { get; }

    public PeopleService(IMessenger messenger)
    {
        Messenger = messenger;
    }

    public async ValueTask<IImmutableList<Person>> GetPeople(CancellationToken ct = default)
    {
        await Task.Delay(1000);

        return _people.ToImmutableList();
    }

    public async ValueTask AddPerson(Person person, CancellationToken ct = default)
    {
        await Task.Delay(500);

        var newId = GenerateNewId();
        var newPerson = person with { Id = newId };

        if (_people.Add(newPerson))
        {
            Messenger.Send(new EntityMessage<Person>(EntityChange.Created, newPerson));
		}
	}

    public async ValueTask RemovePerson(int personId, CancellationToken ct = default)
    {
        await Task.Delay(500);

        if (_people.RemoveWhere(person => person.Id == personId) > 0)
        {
            Messenger.Send(new EntityMessage<Person>(EntityChange.Deleted, Person.EmptyPerson() with { Id = personId }));
        }
    }

    private int GenerateNewId() => _people.DefaultIfEmpty().Max(person => person?.Id ?? default) + 1;

    private readonly HashSet<Person> _people =
    [
	    new(1, "Luke", "Skywalker"),
	    new(2, "Darth", "Vader")
    ];
}
