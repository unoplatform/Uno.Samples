using System.Text.Json;
using System.Text.Json.Serialization;
using DestinationRolodex.Models;

namespace DestinationRolodex.Services;

/// <summary>
/// Loads the packaged catalog (Assets/Data/destinations.json, the handoff's original six entries) and maps each
/// entry's remote photo URL to its packaged ms-appx file by id, as assets/manifest.json declares.
/// Credits, source pages, focal points and alt text are preserved on the model.
/// </summary>
public static class DestinationRepository
{
    public const string CatalogUri = "ms-appx:///Assets/Data/destinations.json";

    public static async Task<IReadOnlyList<Destination>> LoadPackagedAsync(CancellationToken ct = default)
    {
        var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri(CatalogUri)).AsTask(ct);
        var json = await Windows.Storage.FileIO.ReadTextAsync(file).AsTask(ct);
        return Parse(json);
    }

    public static IReadOnlyList<Destination> Parse(string json)
    {
        var dtos = JsonSerializer.Deserialize(json, DestinationJsonContext.Default.ListDestinationDto)
            ?? throw new InvalidDataException("destinations.json is empty.");
        var result = new List<Destination>(dtos.Count);
        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (var d in dtos)
        {
            if (string.IsNullOrWhiteSpace(d.Id) || string.IsNullOrWhiteSpace(d.Country))
            {
                throw new InvalidDataException("Every destination needs an id and a country.");
            }

            if (!ids.Add(d.Id))
            {
                throw new InvalidDataException($"Duplicate destination id: {d.Id}");
            }

            var media = d.Media ?? new MediaDto();
            result.Add(new Destination(
                d.Id,
                d.Country.Trim(),
                string.IsNullOrWhiteSpace(d.Location) ? d.Country.Trim() : d.Location.Trim(),
                d.Region?.Trim() ?? string.Empty,
                d.Description?.Trim() ?? string.Empty,
                new Uri($"ms-appx:///Assets/Destinations/{d.Id}.jpg"),
                FocalPoint.Parse(media.Position),
                string.IsNullOrWhiteSpace(media.Alt) ? $"{d.Location ?? d.Country}, {d.Country}" : media.Alt,
                media.Credit ?? string.Empty,
                Uri.TryCreate(media.Source, UriKind.Absolute, out var source) ? source : null,
                IsHexColor(d.Color) ? d.Color! : "#244e51",
                d.Geo is { Lat: { } lat, Lon: { } lon } && new GeoPoint(lat, lon).IsValid ? new GeoPoint(lat, lon) : null));
        }

        return result;
    }

    private static bool IsHexColor(string? value) =>
        value is { Length: 4 or 5 or 7 or 9 } && value[0] == '#' && value.Skip(1).All(Uri.IsHexDigit);
}

internal sealed class DestinationDto
{
    public string? Id { get; set; }
    public string? Country { get; set; }
    public string? Location { get; set; }
    public string? Region { get; set; }
    public string? Color { get; set; }
    public string? Description { get; set; }
    public MediaDto? Media { get; set; }
    public GeoDto? Geo { get; set; }
}

internal sealed class GeoDto
{
    public double? Lat { get; set; }
    public double? Lon { get; set; }
}

internal sealed class MediaDto
{
    public string? Type { get; set; }
    public string? Src { get; set; }
    public string? Preview { get; set; }
    public string? Position { get; set; }
    public string? Alt { get; set; }
    public string? Credit { get; set; }
    public string? Source { get; set; }
}

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(List<DestinationDto>))]
internal sealed partial class DestinationJsonContext : JsonSerializerContext
{
}
