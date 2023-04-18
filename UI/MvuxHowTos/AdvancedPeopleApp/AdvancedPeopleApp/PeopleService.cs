namespace AdvancedPeopleApp;

public partial record Person(string FirstName, string LastName);

public interface IPeopleService
{
    ValueTask<IImmutableList<Person>> GetPeopleAsync(CancellationToken ct);
    ValueTask<IImmutableList<Person>> GetPeopleAsync(uint pageSize, uint currentPage, CancellationToken ct);
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
            new("Leia", "Organa"),
            new("Lando", "Calrissian"),
            new("Obi-Wan", "Kenobi"),
            new("Anakin", "Skywalker"),
            new("Qui-Gon", "Jinn"),
            new("Mace", "Windu"),
            new("Boba", "Fett"),
            new("Jango", "Fett"),
            new("Mon", "Mothma"),
            new("Wilhuff", "Tarkin"),
            new("Sheev", "Palpatine"),
            new("Count", "Dooku"),
            new("General", "Grievous"),
            new("Ahsoka", "Tano"),
            new("Captain", "Rex"),
            new("Bo-Katan", "Kryze"),
            new("Hera", "Syndulla"),
            new("Kanan", "Jarrus"),
            new("Ezra", "Bridger"),
            new("Sabine", "Wren"),
            new("Garazeb", "Orrelios"),
            new("Cassian", "Andor"),
            new("Jyn", "Erso"),
            new("Bodhi", "Rook"),
            new("Galen", "Erso"),
            new("Orson", "Krennic"),
            new("Saw", "Gerrera"),
            new("Baze", "Malbus"),
            new("Wedge", "Antilles"),
            new("Biggs", "Darklighter"),
            new("Aayla", "Secura"),
            new("Shaak", "Ti"),
            new("Kit", "Fisto"),
            new("Plo", "Koon"),
            new("Bail", "Organa"),
            new("Admiral", "Ackbar"),
            new("Nien", "Nunb"),
            new("Poe", "Dameron"),
            new("Rey", "Skywalker"),
            new("Maz", "Kanata"),
            new("Captain", "Phasma"),
            new("Rose", "Tico"),
            new("Paige", "Tico"),
            new("Zorii", "Bliss"),
            new("Babu", "Frik"),
            new("Dryden", "Vos"),
            new("Enfys", "Nest"),
            new("Tobias", "Beckett"),
            new("Rio", "Durant"),
            new("Admiral", "Holdo"),
            new("Cad", "Bane"),
            new("Aurra", "Sing"),
            new("Asajj", "Ventress"),
            new("Savage", "Opress"),
            new("Fennec", "Shand"),
        };

        return people.ToImmutableList();
    }

    public async ValueTask<IImmutableList<Person>> GetPeopleAsync(uint pageSize, uint currentPageIndex, CancellationToken ct)
    {
        if (pageSize == 0) pageSize = 1;
        ++currentPageIndex;

        // convert to int
        var (size, number) = ((int)pageSize!, (int)currentPageIndex);

        var people = await GetPeopleAsync(ct);

        return people
            .Skip(size * number)
            .Take(size)
            .ToImmutableList();
    }
}