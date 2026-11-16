using System.Text.Json;
using System.Text.Json.Serialization;
using Uno.Wasm.Bootstrap.Server;

var builder = WebApplication.CreateBuilder(args);

// TasksApi.yaml spells Priority as "low" / "normal" / "high".
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<TaskStore>();

var app = builder.Build();

// Also hosts the WebAssembly head, so on that head the app and the API share one origin (no CORS).
app.UseUnoFrameworkFiles();
app.UseStaticFiles();

app.MapTasksApi();

app.MapFallbackToFile("index.html");

app.Run();

/// <summary>Public so the tests can host the server in-process with <c>WebApplicationFactory&lt;Program&gt;</c>.</summary>
public partial class Program;
