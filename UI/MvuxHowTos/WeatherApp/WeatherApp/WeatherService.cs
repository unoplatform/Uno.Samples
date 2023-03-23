namespace WeatherApp;

public partial record WeatherInfo(int Temperature);

public class WeatherService
{
	public async ValueTask<WeatherInfo> GetCurrentWeatherAsync(CancellationToken ct)
	{
		// fake delay to simulate requesting data from a remote server
		await Task.Delay(TimeSpan.FromSeconds(2), ct);

		// assign a random number ranged -40 to 40.
		var temperature = new Random().Next(-40, 40);

		return new WeatherInfo(temperature);
	}
}
