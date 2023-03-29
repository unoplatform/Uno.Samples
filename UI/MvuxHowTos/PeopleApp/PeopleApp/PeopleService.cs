namespace PeopleApp;

public partial record Person(string FirstName, string LastName);

public class PeopleService
{
    public async ValueTask<IImmutableList<Person>> GetPeopleAsync(CancellationToken ct)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), ct);

        var people = new Person[]
        {
            new Person(FirstName: "Master", LastName: "Yoda"),
            new Person(FirstName: "Darth", LastName: "Vader")
        };

        return people.ToImmutableList();
    }
}
