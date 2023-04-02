namespace EditablePeopleApp;

public partial record Person(int Id, string FirstName, string LastName);

public interface IPeopleService
{
    ValueTask<IImmutableList<Person>> GetPeople(CancellationToken ct = default);

    /// <summary>
    /// Adds a new <see cref="Person"/> to the store and returns its new ID.
    /// </summary>
    /// <param name="person">The <see cref="Person"/> to add.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> which is disregarded in this sample.</param>
    /// <returns>The store-assigned ID of the newly created <see cref="Person"/>.</returns>
    ValueTask<int> AddPerson(Person person, CancellationToken ct = default);

    ValueTask RemovePerson(int personId, CancellationToken ct = default);
}

public class PeopleService : IPeopleService
{
    public async ValueTask<IImmutableList<Person>> GetPeople(CancellationToken ct = default)
    {
        // this is what it takes for the 'server' to respond
        await Task.Delay(1000);

        // in real-life example this would be a remote request
        return _people.ToImmutableList();
    }

    /// <inheritdoc/>
    public async ValueTask<int> AddPerson(Person person, CancellationToken ct = default)
    {
        await Task.Delay(500);

        var newId = GenerateNewId();

        // in real-life example this would be a remote request
        _people.Add(person with { Id = newId });

        return newId;
    }

    public async ValueTask RemovePerson(int personId, CancellationToken ct = default)
    {
        await Task.Delay(500);

        _people.RemoveAll(person => person.Id == personId);
    }

    /// <summary>
    /// Get the following Person ID to be assigned.
    /// </summary>
    private int GenerateNewId() => _people.Max(person => person.Id) + 1;

    // this is just for demonstration purposes,
    // ideally a service just passes on information
    // it does not keep any state of it
    private List<Person> _people = new()
    {
        new (1, "Wolfgang", "Mozart"),
        new (2, "Ludwig", "Beethoven"),
    };
}