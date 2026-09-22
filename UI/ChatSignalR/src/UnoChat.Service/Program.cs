using UnoChat.Service.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("CorsPolicy");

app.MapGet("/", () => "UnoChat SignalR Service is running.");
app.MapHub<ChatHub>("/chatHub");

app.Run();

public partial class Program { }