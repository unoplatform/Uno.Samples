using System.Collections.Immutable;

namespace DynamicServicesSampleApp.DataContracts;

public interface IWeatherInformation
{
    Task<IImmutableList<WeatherForecast>?> GetWeather(CancellationToken cancellationToken = default);
}
