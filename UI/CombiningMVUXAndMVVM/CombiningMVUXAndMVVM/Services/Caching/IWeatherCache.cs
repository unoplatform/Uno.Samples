namespace CombiningMVUXAndMVVM.Services.Caching;
using WeatherForecast = CombiningMVUXAndMVVM.Client.Models.WeatherForecast;
public interface IWeatherCache
{
    ValueTask<IImmutableList<WeatherForecast>> GetForecast(CancellationToken token);
}
