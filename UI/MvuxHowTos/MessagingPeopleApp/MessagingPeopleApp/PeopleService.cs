using System.Collections.Immutable;
using CommunityToolkit.Mvvm.Messaging;

namespace MessagingPeopleApp;

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
        // this is what it takes for the 'server' to respond
        await Task.Delay(1000);

        // in real-life example this would be a remote request
        return _people.ToImmutableList();
    }

    /// <inheritdoc/>
    public async ValueTask AddPerson(Person person, CancellationToken ct = default)
    {
        await Task.Delay(500);

        var newId = GenerateNewId();
        var newPerson = person with { Id = newId };

        // in real-life example this would be a remote request
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

    /// <summary>
    /// Get the following Person ID to be assigned.
    /// </summary>
    private int GenerateNewId() => _people.DefaultIfEmpty().Max(person => person?.Id ?? default) + 1;

    // this is just for demonstration purposes,
    // ideally a service just passes on information
    // it does not keep any state of it
    private HashSet<Person> _people = new()
    {
        new (1, "Wolfgang", "Mozart"),
        new (2, "Ludwig", "Beethoven"),
    };
}