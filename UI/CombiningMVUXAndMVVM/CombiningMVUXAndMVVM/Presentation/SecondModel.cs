namespace CombiningMVUXAndMVVM.Presentation;

public partial record SecondModel(Entity Entity)
{
    public IState<string> LastName => State<string>.Value(this, () => string.Empty);

    public IFeed<string> FullName => LastName.Where(lastName => !string.IsNullOrWhiteSpace(lastName))
                                             .Select(lastName => $"🚀 {Entity.Name} {lastName}");

    public IListFeed<City> Cities => ListFeed.Async(GetCities);

    public async Task LoadData(CancellationToken ct)
    {
        await Task.Delay(500, ct);
        await LastName.Update(x => "👟 A task was executed...", ct);
    }

    private async ValueTask<IImmutableList<City>> GetCities(CancellationToken ct)
    {
        await Task.Delay(500, ct);

        var cities = new List<City>
        {
            new("New York", "USA"),
            new("London", "UK"),
            new("Tokyo", "Japan"),
            new("Berlin", "Germany"),
            new("Paris", "France"),
            new("Sydney", "Australia"),
            new("Moscow", "Russia"),
            new("Beijing", "China"),
            new("Rio de Janeiro", "Brazil"),
            new("Cape Town", "South Africa"),
            new("Toronto", "Canada"),
            new("Mumbai", "India"),
            new("Seoul", "South Korea"),
            new("Mexico City", "Mexico"),
            new("Montreal", "Canada"),
        };

        return ImmutableList.CreateRange(cities);
    }
}


public record City(string Name, string Country);
