namespace SelectionPeopleApp;

public partial record Person(string FirstName, string LastName);

public interface IPeopleService
{
    ValueTask<IImmutableList<Person>> GetPeopleAsync(CancellationToken ct);
}

public class PeopleService : IPeopleService
{
    public async ValueTask<IImmutableList<Person>> GetPeopleAsync(CancellationToken ct)
    {
        await Task.Delay(TimeSpan.FromSeconds(1), ct);

        var people = new Person[]
        {
            new("Master", "Yoda"),
            new("Darth", "Vader"),
            new("Luke", "Skywalker"),
            new("Han", "Solo"),
            new("Leia", "Organa")
        };

        return people.ToImmutableList();
    }
}