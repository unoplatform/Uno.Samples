# .NET MAUI Embedding Sample App - DevExpress .NET MAUI Controls

This sample app embeds the [DevExpress .NET MAUI Controls](https://www.devexpress.com/maui/) in an Uno Platform application.

For more information on how to use the controls from DevExpress in an Uno Platform application via .NET MAUI Embedding, please visit [our documentation here](https://aka.platform.uno/maui-embedding-sample-app-devexpress).

<img src="doc/assets/third-party-sample-devexpress.gif" alt="DevExpress .NET MAUI Controls Demo sample" />

> [!NOTE]
> The DevExpress .NET MAUI Controls are currently available [free of charge](https://www.devexpress.com/maui/). However, in order to access the NuGet packages you do need to create an account at [DevExpress website](https://www.devexpress.com/MyAccount/Register/?returnUrl=https%3a%2f%2fnuget.devexpress.com%2f%23feed-url).

> Once you have an account with DevExpress, you need to visit the [Your DevExpress NuGet Feed URL](https://nuget.devexpress.com/#feed-url) page to retrieve a NuGet feed that's associated with your account. You can either add this as a NuGet feed in Visual Studio or use a nuget.config file.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="DevExpress Private Feed" value="[your NuGet feed goes here]" />
  </packageSources>
</configuration>
```

## List of controls used in this sample
- DataGrid
- Chart

## Sample App - Examples

### Android

 ![DevExpress Sample App running on Android](doc/assets/DevExpress_Android.png)

### iOS

 ![DevExpress Sample App running on iOS](doc/assets/DevExpress_iOS.png)

## Sample App - Issues
If you encounter any issues with this sample, please open an issue [here](https://github.com/unoplatform/uno/issues).