using System.Diagnostics;
using System.Net.Sockets;
using Grpc.Net.Client;

namespace TestWeatherService;

[FixtureLifeCycle(LifeCycle.SingleInstance)]
public class WeatherServiceTests
{
    private Process? _serverProcess;
    private GrpcChannel? _channel;

    [OneTimeSetUp]
    public async Task StartServer()
    {
        int httpsPort = GetFreePort();
        int httpPort = GetFreePort();

        var projectPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory,
                         "..", "..", "..", "..",
                         "WeatherGrpcService", "WeatherGrpcService.csproj"));

        _serverProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{projectPath}\" --urls \"https://localhost:{httpsPort};http://localhost:{httpPort}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            }
        };

        _serverProcess.Start();

        // Poll until the server is ready (up to 30 seconds)
        using var httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });

        var deadline = DateTime.UtcNow.AddSeconds(30);
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                await httpClient.GetAsync($"https://localhost:{httpsPort}/");
                break;
            }
            catch
            {
                await Task.Delay(500);
            }
        }

        _channel = GrpcChannel.ForAddress($"https://localhost:{httpsPort}",
            new GrpcChannelOptions
            {
                HttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                }
            });
    }

    [OneTimeTearDown]
    public void StopServer()
    {
        _channel?.Dispose();
        if (_serverProcess is { HasExited: false })
        {
            _serverProcess.Kill(entireProcessTree: true);
            _serverProcess.WaitForExit();
        }
        _serverProcess?.Dispose();
    }

    private static int GetFreePort()
    {
        var listener = new TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        int port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    [Test]
    public async Task GetWeather_WhenCityExistsSuccess()
    {
        var weatherClient = new Weather.WeatherClient(_channel);
        var weatherReply = await weatherClient.GetWeatherAsync(
            new WeatherRequest { City = "Barcelona", Mock = false });

        Assert.That(weatherReply.Success, Is.True);
    }

    [Test]
    public async Task GetWeather_WhenCityDoesNotExistFails()
    {
        var weatherClient = new Weather.WeatherClient(_channel);
        var weatherReply = await weatherClient.GetWeatherAsync(
            new WeatherRequest { City = "NonExistentCity", Mock = false });

        Assert.That(weatherReply.Success, Is.False);
    }

    [Test]
    public async Task GetWeather_WhenMockDataSuccess()
    {
        var weatherClient = new Weather.WeatherClient(_channel);
        var weatherReply = await weatherClient.GetWeatherAsync(
            new WeatherRequest { City = "Barcelona", Mock = true });

        Assert.That(weatherReply.Success, Is.True);
    }
}
