namespace WeatherApp;

public partial record WeatherInfo(int Temperature);

public interface IWeatherService
{
    ValueTask<WeatherInfo> GetCurrentWeather(CancellationToken ct);
}

public class WeatherService : IWeatherService
{
    public async ValueTask<WeatherInfo> GetCurrentWeather(CancellationToken ct)
    {
        await Task.Delay(TimeSpan.FromSeconds(1), ct);
        return new WeatherInfo(new Random().Next(-40, 40));
    }
}