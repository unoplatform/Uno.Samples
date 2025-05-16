using DynamicServicesSampleApp.DataContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DynamicServicesSampleApp.Data.LocalData;
public static class HostBuilderExtensions
{
    public static IHostBuilder UseLocalWeatherService(this IHostBuilder hostBuilder)
        => hostBuilder
            .ConfigureServices(
                services => services.AddSingleton<IWeatherInformation, WeatherInformationService>());
}
