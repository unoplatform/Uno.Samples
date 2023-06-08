using System.Collections.Immutable;

using UnoChatGPT.DataContracts;

namespace UnoChatGPT.Services.Caching
{
	public interface IWeatherCache
	{
		ValueTask<IImmutableList<WeatherForecast>> GetForecast(CancellationToken token);
	}
}