namespace DynamicServicesSampleApp.Presentation;

public partial record SecondModel(Entity Entity, IWeatherInformation WeatherService)
{
    public IListState<WeatherForecast> Forecasts => ListState<WeatherForecast>.Empty(this);
    public async Task FetchWeather(CancellationToken ct)
    {
        var newForecasts = await WeatherService.GetWeather(ct);
        await Forecasts.UpdateAsync(data => newForecasts ?? [], ct);
    }
}
