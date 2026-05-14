namespace BrewHouse.Models;

public partial record AppConfig
{
    public string? Environment { get; init; }
}
