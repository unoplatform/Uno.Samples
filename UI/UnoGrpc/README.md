# UnoGrpc

This project shows how to use gRPC in an Uno Platform application.

Tested on:

[x] Desktop  
[x] Wasm  
[x] Android  

## How to run the sample:

1. Start the WeatherGrpcService using the localSettings.json HTTPS profile or using:

```ps
dotnet run --urls "https://localhost:7014"
```

2. Run the UnoGrpc application using Desktop, WebAssembly, or Android target.

## NuGet packages

Both client packages must be included because WebAssembly uses the Web version.

```xml
<PackageVersion Include="Grpc.Net.Client" Version="2.76.0" />
<PackageVersion Include="Grpc.Net.Client.Web" Version="2.76.0" />
```

## gRPC server configuration

By default the gRPC server must accept HTTP1 and HTTP2, since WebAssembly only uses HTTP1.

Edit **appsettings.json** to change 'Protocols':

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Kestrel": {
    "EndpointDefaults": {
      "Protocols": "Http1AndHttp2"
    }
  }
}
```
CORS must be configured on the gRPC server for wasm, it must allow the origins and add `app.UseGrpcWeb()`.

```csharp
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
```

## Client configuration

- To use Android, add `UseNativeHttpHandler` to the project.

```xml
    <PropertyGroup Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">
        <UseNativeHttpHandler>false</UseNativeHttpHandler>
    </PropertyGroup>
```

and configure the service depending on whether it is Android, Desktop, or WebAssembly:

NOTE: For Android it is using 10.0.2.2 that should point to localhost.

```csharp
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
```
