using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Authentication.Cookies;
using Uno.Wasm.Bootstrap.Server;
using Authentication.WebExtensionsDemo.DataContracts.Serialization;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.ConfigureHttpJsonOptions(options =>
        options.SerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine(AuthJsonContext.Default));

    builder.Services.Configure<DemoAuthOptions>(builder.Configuration.GetSection("DemoAuth"));
    builder.Services.AddSingleton(TimeProvider.System);
    builder.Services.AddSingleton<DemoTokenStore>();

    // The browser session behind the login page (see AuthApi).
    builder.Services
        .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options => options.Cookie.Name = "webdemo-session");

    var app = builder.Build();

    // Also hosts the WebAssembly head, so on that head the app, the login page and the API share
    // one origin: no CORS, and the sign-in callback lands on a page this server serves.
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
