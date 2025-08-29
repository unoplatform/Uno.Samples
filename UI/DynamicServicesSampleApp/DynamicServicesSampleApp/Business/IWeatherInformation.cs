using System.Collections.Immutable;

namespace DynamicServicesSampleApp.Business;
public interface WeatherInformation
{
    IImmutableList<WeatherForecast> GetWeather();
}
