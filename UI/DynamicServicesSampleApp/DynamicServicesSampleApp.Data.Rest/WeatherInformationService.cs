namespace DynamicServicesSampleApp.Data.Rest;

internal record WeatherInformationService(IApiClient weatherClient) : IWeatherInformation
{
    public async Task<IImmutableList<WeatherForecast>?> GetWeather(CancellationToken ct)
        => (await weatherClient.GetWeather(ct)).Content;
}
