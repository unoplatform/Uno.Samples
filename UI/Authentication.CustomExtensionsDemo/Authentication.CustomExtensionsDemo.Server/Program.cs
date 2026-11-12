using System.Text.Json.Serialization.Metadata;
using Uno.Wasm.Bootstrap.Server;
using Authentication.CustomExtensionsDemo.DataContracts.Serialization;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.ConfigureHttpJsonOptions(options =>
        options.SerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine(AuthJsonContext.Default));

    builder.Services.Configure<DemoAuthOptions>(builder.Configuration.GetSection("DemoAuth"));
    builder.Services.AddSingleton(TimeProvider.System);
    builder.Services.AddSingleton<DemoTokenStore>();

    // The OpenAPI document the app's Kiota client is generated from
    // (Specs/DemoServer.swagger.json in the app project; see the README).
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Also hosts the WebAssembly head, so on that head the app and the API share one origin and
    // no CORS is needed.
    app.UseUnoFrameworkFiles();
    app.UseStaticFiles();

    app.MapAuthApi();
    app.MapProfileApi();

    app.MapFallbackToFile("index.html");

    await app.RunAsync();
}
catch (Exception ex)
{
    Console.Error.WriteLine("Application terminated unexpectedly");
    Console.Error.WriteLine(ex);
#if DEBUG
    if (System.Diagnostics.Debugger.IsAttached)
    {
        System.Diagnostics.Debugger.Break();
    }
#endif
}
