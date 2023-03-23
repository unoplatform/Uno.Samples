namespace WeatherApp;

using Uno.Extensions.Reactive;

public partial record WeatherModel(WeatherService WeatherService)
{
	public IFeed<WeatherInfo> CurrentWeather => Feed.Async(WeatherService.GetCurrentWeatherAsync);
}
