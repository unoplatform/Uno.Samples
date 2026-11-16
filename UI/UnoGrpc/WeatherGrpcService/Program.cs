using WeatherGrpcService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

//var app = builder.Build();

// Configure the HTTP request pipeline.
//app.MapGrpcService<GreeterService>();
//app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

//app.Run();
builder.Services.AddGrpc();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Wasm", p => p
        .WithOrigins("http://localhost:5000", "https://localhost:5001")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

var app = builder.Build();

app.UseRouting();
app.UseGrpcWeb();
app.UseCors();

app.MapGrpcService<WeatherService>().EnableGrpcWeb().RequireCors("Wasm");
app.MapGet("/", () => "gRPC service");

app.Run();
