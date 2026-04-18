using Grpc.Core;
using System.Text.Json;

namespace WeatherGrpcService.Services;

public class WeatherService(ILogger<WeatherService> logger) : Weather.WeatherBase
{
    private static readonly string[] _descriptions =
    [
        "Sunny",
        "Partly cloudy",
        "Cloudy",
        "Overcast",
        "Light rain",
        "Rainy",
        "Stormy",
        "Windy",
        "Foggy",
        "Snowy"
    ];

    private static readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://goweather.xyz/weather/")
    };

    public override async Task<WeatherReply> GetWeather(WeatherRequest request, ServerCallContext context)
    {
        logger.LogInformation("Requested temperature for {Name}", request.City);

        var encodedCity = Uri.EscapeDataString(request.City);
        var url = $"{encodedCity}";

        if (request.Mock)
        {
            return await MockedWeatherAsync(request);
        }

        try
        {
            using var response = await _httpClient.GetAsync(url, context.CancellationToken);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync(context.CancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: context.CancellationToken);

            var weather = document.RootElement;

            var reply = new WeatherReply
            {
                City = request.City,
                Temperature = weather.GetProperty("temperature").GetString() ?? string.Empty,
                Description = weather.GetProperty("description").GetString() ?? string.Empty,
                Wind = weather.GetProperty("wind").GetString() ?? string.Empty,
                Success = true
            };

            logger.LogInformation("Success: Temperature={temp} Wind={wind} Description={desc}", reply.Temperature, reply.Wind, reply.Description);

            return reply;
        }
        catch (Exception ex)
        {
            logger.LogError("Exception: {msg}", ex.Message);

            return new WeatherReply
            {
                City = request.City,
                Temperature = "",
                Description = ex.Message,
                Wind = string.Empty,
                Success = false
            };
        }

    }

    private async Task<WeatherReply> MockedWeatherAsync(WeatherRequest request)
    {
        logger.LogInformation("Mock weather requested for {City}", request.City);

        var temperature = Random.Shared.Next(-5, 41); // Celsius
        var wind = Random.Shared.Next(0, 81);         // km/h
        var description = _descriptions[Random.Shared.Next(_descriptions.Length)];

        var reply = new WeatherReply
        {
            City = request.City,
            Temperature = $"{temperature} °C",
            Wind = $"{wind} km/h",
            Description = description,
            Success = true
        };

        logger.LogInformation(
            "Mock response: Temperature={Temperature}, Wind={Wind}, Description={Description}",
            reply.Temperature, reply.Wind, reply.Description);

        return reply;
    }
}
