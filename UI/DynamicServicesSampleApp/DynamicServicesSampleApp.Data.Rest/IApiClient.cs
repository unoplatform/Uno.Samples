using Refit;

namespace DynamicServicesSampleApp.Data.Rest;

[Headers("Content-Type: application/json")]
internal interface IApiClient
{
    [Get("/api/weatherforecast")]
    Task<ApiResponse<IImmutableList<WeatherForecast>>> GetWeather(CancellationToken cancellationToken = default);
}
