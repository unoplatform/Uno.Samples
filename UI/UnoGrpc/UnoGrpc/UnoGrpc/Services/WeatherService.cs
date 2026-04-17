using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
namespace UnoGrpc.Services;

public class WeatherService
{
    private readonly string _channelUrl =
#if __ANDROID__
        "https://10.0.2.2:7014";
#else
        "https://localhost:7014";
#endif

    private GrpcChannel CreateChannel()
    {
#if __WASM__
        var wasmHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
        return GrpcChannel.ForAddress(_channelUrl, new GrpcChannelOptions
        {
            HttpHandler = wasmHandler
        });
#elif __ANDROID__
        var androidHandler = new SocketsHttpHandler();
#if DEBUG
        androidHandler.SslOptions.RemoteCertificateValidationCallback = (_, _, _, _) => true;
#endif
        return GrpcChannel.ForAddress(_channelUrl, new GrpcChannelOptions
        {
            HttpHandler = androidHandler
        });
#else
        return GrpcChannel.ForAddress(_channelUrl);
#endif
    }

    public async Task<WeatherReply> GetWeatherAsync(string city, bool mockData)
    {
        try
        {
            using var channel = CreateChannel();

            var client = new Weather.WeatherClient(channel);
            var weatherReply = await client.GetWeatherAsync(
                new WeatherRequest { City = (city ?? string.Empty).Replace(" ", ""), Mock = mockData });
            return weatherReply;
        }
        catch (Exception ex)
        {
            return new WeatherReply
            {
                City = string.Empty,
                Temperature = string.Empty,
                Wind = string.Empty,
                Description = ex.Message,
                Success = false
            };
        }
    }
}
