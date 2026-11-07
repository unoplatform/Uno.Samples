using System.Diagnostics.CodeAnalysis;
using Http.KiotaExtensionsDemo.Client;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace Http.KiotaExtensionsDemo.Services;

public static class TasksApiSetup
{
    /// <summary>
    /// Registers the generated <see cref="TasksApiClient"/> and the <see cref="TasksService"/> that
    /// wraps it. Call it inside <c>UseHttp</c>. The app and the tests share this method.
    /// </summary>
    /// <remarks>
    /// This is <c>AddKiotaClient&lt;TasksApiClient&gt;</c> with one difference. <c>AddKiotaClient</c>
    /// gives the request adapter Kiota's JSON factories only (unoplatform/uno.extensions#2799), so
    /// the contract's <c>text/plain</c> status, its <c>multipart/form-data</c> upload and its
    /// <c>application/problem+json</c> validation errors would all throw
    /// <c>ArgumentOutOfRangeException: expected a application/json content type</c>. Here the
    /// adapter gets no factories, so it uses Kiota's default registries, which the generated
    /// client's constructor fills with JSON, text, form and multipart. Everything else is what
    /// <c>AddKiotaClient</c> does: Uno's <c>AddClient</c> supplies the base address, the native
    /// handler and every registered <c>DelegatingHandler</c>, and Kiota's own middleware (retry,
    /// redirect, user agent...) is attached the same way, so a 503 with <c>Retry-After</c> is retried.
    /// Go back to <c>AddKiotaClient</c> once that issue is fixed.
    /// </remarks>
    [RequiresDynamicCode("Binds EndpointOptions from configuration when options is null.")]
    [RequiresUnreferencedCode("Binds EndpointOptions from configuration when options is null.")]
    public static IServiceCollection AddTasksApi(this IServiceCollection services, HostBuilderContext context, string? url)
    {
        const string name = nameof(TasksApiClient);
        var kiotaHandlers = KiotaClientFactory.GetDefaultHandlerActivatableTypes();
        foreach (var handler in kiotaHandlers)
        {
            services.AddTransient(handler);
        }

        services.AddClient<TasksApiClient>(
            context,
            // Passed as options, not bound from a section: a bound Url is lost in trimmed builds (unoplatform/uno.extensions#3266).
            options: new EndpointOptions { Url = url },
            name: name,
            httpClientFactory: (s, _) =>
            {
                var builder = s
                    .AddHttpClient<TasksApiClient>(name)
                    .AddTypedClient((httpClient, _) =>
                        new TasksApiClient(new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), httpClient: httpClient)));
                foreach (var handler in kiotaHandlers)
                {
                    builder.AddHttpMessageHandler(sp => (DelegatingHandler)sp.GetRequiredService(handler));
                }
                return builder;
            });

        return services.AddSingleton<TasksService>();
    }
}
