using DynamicServicesSampleApp.DataContracts.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DynamicServicesSampleApp.Data.Rest;
public static class HostBuilderExtensions
{
    public static IHostBuilder UseRestWeatherService(this IHostBuilder hostBuilder)
        => hostBuilder
                // Register Json serializers (ISerializer and ISerializer)
                .UseSerialization((context, services) => services
                    .AddContentSerializer(context)
                    .AddJsonTypeInfo(WeatherForecastContext.Default.IImmutableListWeatherForecast))
                .UseHttp((context, services) => services
                    // Register HttpClient
#if DEBUG
                    // DelegatingHandler will be automatically injected into Refit Client
                    .AddTransient<DelegatingHandler, DebugHttpHandler>()
#endif
                    .AddRefitClient<IApiClient>(context))
                .ConfigureServices(
                    services => services.AddSingleton<IWeatherInformation, WeatherInformationService>());
}
