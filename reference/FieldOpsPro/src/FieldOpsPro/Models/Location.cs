namespace FieldOpsPro.Models;

public record Location(
    double Latitude,
    double Longitude,
    string Address,
    string? Name = null,
    string? Floor = null,
    string? Distance = null
);
