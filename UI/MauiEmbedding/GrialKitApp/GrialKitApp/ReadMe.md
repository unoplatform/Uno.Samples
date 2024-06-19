# .NET MAUI Embedding Sample App - Grial UI Kit for .NET MAUI

This sample app embeds the [Grial UI Kit for .NET MAUI](https://grialkit.com/) in an Uno Platform application.

For more information on how to use the controls from GrialKit in an Uno Platform application via .NET MAUI Embedding, please visit [our documentation here](https://aka.platform.uno/maui-embedding-sample-app-grialkit).

<img src="doc/assets/third-party-sample-grialkit.gif" alt="Grial UI Kit for .NET MAUI Demo sample" />

> [!NOTE]
> In order to use the Grial UI Kit controls, you first need to [sign up](https://admin.grialkit.com/secure/grial/front/signup). 

> After signing up, when you log in to the [admin portal](https://admin.grialkit.com), you'll be prompted to create a Grial app by entering the name of the application. Once you've created the Grial app, go to the Download tab and select `Download Kickoff Solution` - at this point, you'll be prompted to purchase a license, which you'll need in order to proceed with using Grial.

> Extract the downloaded zip file and locate the GrialLicense file. We'll add this file to the Uno application. Also in the zip file, open the csproj file and retrieve the ApplicationTitle and ApplicationId property values.

```xml
<!-- Display name -->
<ApplicationTitle>[ApplicationTitle]</ApplicationTitle>
<!-- App Identifier -->
<ApplicationId>[ApplicationId]</ApplicationId>
```
> The last thing we'll need from the zip file is the nuget.config file. This file will also be added to the Uno application.

## List of controls used in this sample
- Area Chart
- Bar Chart
- Bar Multi Series
- Line Chart
- Pie Chart

## Sample App - Examples

### Android

 ![GrialKit Sample App running on Android](doc/assets/GrialKit_Android.png)

### iOS

 ![GrialKit Sample App running on iOS](doc/assets/GrialKit_iOS.png)

## Sample App - Issues
If you encounter any issues with this sample, please open an issue [here](https://github.com/unoplatform/uno/issues).
