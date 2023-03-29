namespace WeatherApp;

using Uno.Extensions.Reactive;

public partial record WeatherModel(IWeatherService WeatherService)
{
	public IFeed<WeatherInfo> CurrentWeather => Feed.Async(WeatherService.GetCurrentWeather);
}
