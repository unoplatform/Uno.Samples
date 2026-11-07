using Microsoft.AspNetCore.Mvc.Testing;
using Uno.Extensions;

namespace Http.KiotaExtensionsDemo.Tests;

/// <summary>
/// The server running in-process, and a host registered the way the app registers its API:
/// <c>UseHttp</c> plus the app's own <see cref="TasksApiSetup.AddTasksApi"/>. The only swap is the
/// primary handler (the "native" one <c>UseHttp</c> registers), which here sends requests into
/// the in-process server instead of the network. Every test gets a fresh server, so a fresh store
/// and a fresh summary warm-up.
/// </summary>
public sealed class TasksApiHost : IDisposable
{
    private readonly WebApplicationFactory<global::Program> _server = new();
    private readonly IHost _host;

    public TasksApiHost(Action<HostBuilderContext, IServiceCollection>? register = null)
    {
        register ??= (context, services) => services.AddTasksApi(context, "http://localhost");
        _host = new Microsoft.Extensions.Hosting.HostBuilder()
            .UseHttp(register)
            .ConfigureServices(services => services.AddTransient<HttpMessageHandler>(_ => _server.Server.CreateHandler()))
            .Build();
    }

    public IServiceProvider Services => _host.Services;

    public TasksService Tasks => Services.GetRequiredService<TasksService>();

    public void Dispose()
    {
        _host.Dispose();
        _server.Dispose();
    }
}
