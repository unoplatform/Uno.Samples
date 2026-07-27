---
uid: Uno.Workshops.ToDo-App.DefiningConfigurationLoadedRecord
---

# Authentication Options via `IOptions` and `appsettings.json`

This sample demonstrates how to define immutable authentication options as `record`, load them from `appsettings.json` using [Uno.Extensions.Configuration](xref:Uno.Extensions.Configuration.Overview) `UseConfiguration(...).Section<T>()` and consume them inside of an `AuthenticationService` which will be used in this Sample app.

## 1. Defining the `Auth` record

Your JSON file(s) will consist of a serialized representation of multiple properties and their values. Hence, configuration sections allow you to programmatically read a specific subset of these properties from the instantiated class that represents them.

To ensure immutability while still allowing the configuration binder to populate values, the record must expose *init-only* properties.
A primary constructor **cannot** be used, because the configuration binder requires a parameterless constructor and settable properties.

Author a new class or record with related properties to be used for configuration:

```csharp
public partial record Auth
{
    public string? ApplicationId { get; init; }
    public string[]? Scopes { get; init; }
    public string? RedirectUri { get; init; }
    public string? KeychainSecurityGroup { get; init; }
}
```

The following form **does not work** with `IOptions`, even if all parameters are nullable:

```csharp
public partial record Auth(string? ApplicationId, string[]? Scopes, string? RedirectUri, string? KeychainSecurityGroup);
```

The binder cannot map configuration values into primary-constructor parameters.

> [!TIP]
> If you want to keep the Types non-nullable and depending on their implementation, you can apply default values like:
> ```csharp
> public partial record Auth
> {
>     public string ApplicationId { get; init; } = string.Empty;
>     public string[]? Scopes { get; init; } = [];
>     public string RedirectUri { get; init; } = string.Empty;
>     public string KeychainSecurityGroup { get; init; } = string.Empty;
> }
> ```

## 2. Add the configuration settings into your `appsettings.json` file

```json
{
  "Auth": {
    "ApplicationId": "my-app-id",
    "Scopes": [ "openid", "profile" ],
    "RedirectUri": "myapp://auth",
    "KeychainSecurityGroup": "com.company.myapp"
  }
}
```

> [!WARNING]
> Especially for Authentication related Data which includes sensitive credentials like Client ID or a Client Secret, make sure to use a Credential Handler like in Development Environment the `dotnet user-secrets` and **do not** check in any of them into Source Control!

## 3.  Registering the configuration section

You can now use the `.Section<T>()` extension method on `IConfigBuilder` to load configuration information for class or record of the type argument you specify:

```csharp
protected override void OnLaunched(LaunchActivatedEventArgs args)
{
    var appBuilder = this.CreateBuilder(args)
        .Configure(host => {
            host.UseConfiguration(configure: configBuilder =>
                configBuilder
                    .AddEmbeddedSource<App>()
                        .Section<Auth>("Auth")
                        ...
        });
}
```

This binds the `Auth` section to the `Auth` record and makes it available through `IOptions<Auth>` or `IOptionsSnapshot<Auth>`.

## 4. Consuming the options in a service

To access the instantiated representation of the configuration section you registered above, complete with values populated from the `appsettings.json` file, you'll need to add a new constructor parameter for it to one of your application's services.

The configuration section will be injected as an object of type `IOptions<T>`, so add a corresponding parameter for it to the constructor of the service:

```csharp
using Microsoft.Extensions.Options;

public class AuthenticationService : IAuthenticationService
{
    public AuthenticationService(IOptions<Auth> settings)
    {
        var authSettings = settings.Value;
        ...
    }
    ...
}
```

## Learn more about Configuration in Uno Apps

* [How To: Uno.Extensions.Configuration](xref:Uno.Extensions.Configuration.Overview)