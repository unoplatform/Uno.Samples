using System.Collections.Immutable;

using UnoChatGPT.DataContracts;

namespace UnoChatGPT.Services.Endpoints
{
	[Headers("Content-Type: application/json")]
	public interface IApiClient
	{
		[Get("/api/weatherforecast")]
		Task<ApiResponse<IImmutableList<WeatherForecast>>> GetWeather(CancellationToken cancellationToken = default);
	}
}