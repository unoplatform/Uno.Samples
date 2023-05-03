namespace PaginationPeopleApp;

public partial record Person(string FirstName, string LastName)
{
    public override string ToString() => $"{FirstName} {LastName}";
}

public interface IPeopleService
{
    ValueTask<IImmutableList<Person>> GetPeopleAsync(uint pageSize, uint pageIndex, CancellationToken ct);

    ValueTask<uint> GetPageCount(uint pageSize, CancellationToken ct);

    ValueTask<(IImmutableList<Person> CurrentPage, Person NextCursor)> GetPeopleAsync(Person? cursor, uint pageSize, CancellationToken ct);
}

public class PeopleService : IPeopleService
{
    public async ValueTask<IImmutableList<Person>> GetPeopleAsync(uint pageSize, uint pageIndex, CancellationToken ct)
    {
        // convert to int for use with LINQ
        var (size, index) = ((int)pageSize, (int)pageIndex);

        // fake delay to simulate loading data
        await Task.Delay(TimeSpan.FromSeconds(1), ct);

        // this is where we would asynchronously load actual data from a remote data store
        var people = GetPeople();

        return people
            .Skip(size * index)
            .Take(size)
            .ToImmutableList();
    }

    // Determines how many pages we'll need to display all the data.
    public async ValueTask<uint> GetPageCount(uint pageSize, CancellationToken ct) =>
        (uint)Math.Ceiling(GetPeople().Length / (double)pageSize);

    public async ValueTask<(IImmutableList<Person> CurrentPage, Person NextCursor)> GetPeopleAsync(Person? cursor, uint pageSize, CancellationToken ct)
    {
        // fake delay to simulate loading data
        await Task.Delay(TimeSpan.FromSeconds(1), ct);

        var people = GetPeople();

        var collection = people
            // order by full name
            .OrderBy(person => person.ToString())
            // select only subsequent items
            .Where(person => StringComparer.CurrentCultureIgnoreCase.Compare(cursor?.ToString(), person.ToString()) <= 0)
            // take only n number of rows, plus the first entity of the next page
            .Take((int)pageSize + 1)
            // using array to enable range access
            .ToArray();

        // this returns a tuple of two elements
        // first element is the current page's entities except the last
        // the second contains the last item in the collection, which is a cursor for next page
        return (CurrentPage: collection[..^1].ToImmutableList(), NextCursor: collection[^1]);
    }

    private Person[] GetPeople() =>
        new Person[]
        {
            new("Liam", "Wilson"),
            new("Emma", "Murphy"),
            new("Noah", "Jones"),
            new("Olivia", "Harris"),
            new("William", "Jackson"),
            new("Ava", "Martin"),
            new("James", "Lee"),
            new("Sophia", "Garcia"),
            new("Logan", "Rodriguez"),
            new("Isabella", "Martinez"),
            new("Benjamin", "Davis"),
            new("Mia", "Anderson"),
            new("Mason", "Thomas"),
            new("Charlotte", "Moore"),
            new("Elijah", "Jackson"),
            new("Amelia", "Johnson"),
            new("Ethan", "White"),
            new("Harper", "Clark"),
            new("Michael", "Lewis"),
            new("Evelyn", "Robinson"),
            new("Daniel", "Walker"),
            new("Abigail", "Perez"),
            new("Alexander", "Hall"),
            new("Emily", "Young"),
            new("Matthew", "Allen"),
            new("Madison", "King"),
            new("Aiden", "Wright"),
            new("Victoria", "Scott"),
            new("Samuel", "Green"),
            new("Chloe", "Baker"),
            new("Christopher", "Adams"),
            new("Sofia", "Nelson"),
            new("Andrew", "Carter"),
            new("Ella", "Mitchell"),
            new("Joshua", "Parker"),
            new("Addison", "Turner"),
            new("Avery", "Phillips"),
            new("David", "Campbell"),
            new("Scarlett", "Parker"),
            new("Joseph", "Evans"),
            new("Lily", "Edwards"),
            new("Nathan", "Collins"),
            new("Aubrey", "Stewart"),
            new("Brandon", "Sanchez"),
            new("Hannah", "Morris"),
            new("Justin", "Nguyen"),
            new("Isabelle", "Rivera"),
            new("Caleb", "Coleman"),
            new("Samantha", "Gray"),
            new("Mason", "Bryant"),
            new("Zoe", "Cruz"),
            new("Jacob", "Reed"),
            new("Layla", "Henderson"),
            new("Logan", "Gonzales"),
            new("Gabriel", "Roberts"),
            new("Audrey", "Turner"),
            new("Lucas", "Phillips"),
            new("Skylar", "Wilson"),
            new("Ethan", "Gonzalez"),
            new("Natalie", "Barnes"),
            new("Kaylee", "Cox"),
            new("William", "Ross"),
            new("Aaliyah", "Cooper"),
            new("Aiden", "Hayes"),
            new("Brooklyn", "Green"),
            new("Samuel", "Hill"),
            new("Avery", "Baker"),
            new("Benjamin", "Cruz"),
            new("Leah", "Ortiz"),
            new("David", "Garcia"),
            new("Aubrey", "Barnes"),
            new("Elijah", "Diaz"),
            new("Emma", "Torres"),
            new("Connor", "Rogers"),
            new("Addison", "Peterson"),
            new("Carter", "Coleman"),
            new("Abigail", "West"),
            new("Noah", "Foster"),
            new("Lila", "Sanders"),
            new("Christopher", "Powell"),
            new("Caroline", "Sullivan"),
            new("Mason", "Johnson"),
            new("Grace", "Adams"),
            new("Jackson", "Flores"),
            new("Madelyn", "Mitchell"),
            new("Aiden", "Butler"),
            new("Eva", "Frazier"),
            new("Lincoln", "Bishop"),
            new("Emerson", "Walsh"),
            new("Lydia", "Holt"),
            new("Colin", "Morrison"),
            new("Vivian", "Sharp"),
            new("Finn", "Black"),
            new("Cassidy", "Conner"),
            new("Gabriella", "Higgins"),
            new("Wyatt", "Barton"),
            new("Makayla", "Lambert"),
            new("Hudson", "Pierce"),
            new("Jocelyn", "Harrington"),
            new("Nathaniel", "Gibson"),
            new("Aurora", "Fuller"),
            new("Seth", "Fields"),
            new("Kinsley", "Greer"),
            new("Damian", "Reyes"),
            new("Kendall", "Hodges"),
            new("Landon", "Castillo"),
            new("Malia", "Shaw"),
            new("Maximus", "Fleming"),
            new("Lyla", "Riley"),
            new("Colton", "Navarro"),
            new("Alexa", "Meadows"),
            new("Emmett", "McGuire"),
            new("Kiara", "Griffin"),
            new("Brantley", "Summers"),
            new("Delilah", "Wilkerson"),
            new("Miles", "Hubbard"),
            new("Elise", "Conrad"),
            new("Dominic", "Barrera"),
            new("Gianna", "Huang"),
            new("Jaxson", "Vaughn"),
            new("Aaliyah", "Nash"),
            new("Joel", "Roman"),
            new("Adalynn", "Dickson"),
            new("Beau", "Owens"),
            new("Brynn", "Huff"),
            new("Dante", "Richmond"),
            new("Celeste", "Pham"),
            new("Ryker", "Keller"),
            new("Eloise", "Sheppard"),
            new("Israel", "Briggs"),
            new("Ivy", "Velasquez"),
            new("Maximilian", "Hoover"),
            new("Alexandra", "Chan"),
            new("Greyson", "Short"),
            new("Iris", "Blevins"),
            new("Jace", "Langley"),
            new("Kira", "Benton"),
            new("Ryland", "Orozco"),
            new("Nora", "Wilcox"),
            new("Tobias", "Knox"),
            new("Amina", "Zhang"),
            new("Griffin", "Wilkinson"),
            new("Astrid", "Ritter"),
            new("River", "Herman"),
            new("Emersyn", "Hatfield"),
            new("Rhys", "Sexton"),
            new("Saylor", "Soria"),
            new("Gideon", "Roach"),
            new("Marley", "Lutz"),
            new("Jagger", "Dougherty"),
            new("Maren", "Frost"),
            new("Emiliano", "McClain"),
            new("Mikayla", "Cohen"),
            new("Phoenix", "Acevedo"),
            new("Averie", "Fulton"),
            new("Ronan", "Whitaker"),
            new("Elaina", "Werner"),
            new("Kian", "Blackwell"),
            new("Kaydence", "Hanna"),
            new("Apollo", "Chase"),
            new("Nina", "Lara"),
            new("Zaiden", "Randall"),
            new("Paloma", "Hester"),
            new("Mauricio", "Morse")
        };
}