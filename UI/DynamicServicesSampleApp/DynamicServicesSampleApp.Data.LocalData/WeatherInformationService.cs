using System.Collections.Immutable;
using DynamicServicesSampleApp.DataContracts;

namespace DynamicServicesSampleApp.Data.LocalData;

internal record WeatherInformationService() : IWeatherInformation
{
    public Task<IImmutableList<WeatherForecast>?> GetWeather(CancellationToken ct)
        => Task.FromResult<IImmutableList<WeatherForecast>?>(
            [
                new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), 32, "Sunny")
            ]);
}
