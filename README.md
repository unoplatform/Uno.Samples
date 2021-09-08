# Uno Platform Samples

This repository provides simple, to-the-point code samples for the [Uno Platform](https://platform.uno/).

See a complete list of code samples [below](#samples). Some of the samples have accompanying [step-by-step tutorials](https://platform.uno/docs/articles/tutorials-intro.html) in the official Uno Platform documentation.

<h1 align=center>
 <img align=center width="25%" src="https://raw.githubusercontent.com/unoplatform/styleguide/master/logo/uno-platform-logo-with-text.png" />
</h1>

## Pixel-Perfect. Multi-Platform. C# & Windows XAML. Today.

[![Twitter Followers](https://img.shields.io/twitter/follow/unoplatform?label=follow%20%40unoplatform&style=flat)](https://twitter.com/unoplatform)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](https://github.com/unoplatform/uno/blob/master/CONTRIBUTING.md)

## What is the Uno Platform?

The Uno Platform is a UI Platform for building single-codebase applications for Windows, Web/WebAssembly, iOS, macOS, Android and Linux. 

It allows C# and Windows XAML code to run on all target platforms, while allowing you control of every pixel. It comes with support for Fluent, Material and Cupertino design systems out of the box. Uno Platform implements a growing number of the UWP APIs, such as **Windows.UI.Xaml**, to enable UWP and WinUI applications to run on on all platforms with native performance. 

Use the UWP/WinUI tooling from Windows in [Visual Studio](https://www.visualstudio.com/), such as [XAML Hot Reload](https://docs.microsoft.com/en-us/visualstudio/xaml-tools/xaml-hot-reload?view=vs-2019) and [C# Edit and Continue](https://docs.microsoft.com/en-us/visualstudio/debugger/how-to-use-edit-and-continue-csharp), build your application as much as possible on Windows, then validate that your application runs on iOS, Android, macOS and WebAssembly.

Visit [our documentation](https://platform.uno/docs/articles/intro.html) for more details.

## Samples

### Android Custom Camera  
An Android-specific sample that shows how to start a camera capture intent, and display the result in an `Image` control.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/AndroidCustomCamera)

### Authentication with OpenID Connect (OIDC)

This sample application demonstrates the usage  of the `WebAuthenticationBroker` in Uno with an OpenID Connect endpoint.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/Authentication.OidcDemo)

[Follow the tutorial](https://platform.uno/docs/articles/guides/open-id-connect.html)

### Auto Suggest  
An implementation of the XAML `AutoSuggest` control, showing how to autofill suggestions for user input. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/AutoSuggestSample)

### Benchmark
An implementation of the .NET Benchmark Control, a performance comparison tool.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/Benchmark)

### Camera Capture UI  
A cross-platform implementation of the UWP `CameraCaptureUI` class that allows the user to capture audio, video, and photos from the device camera. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CameraCaptureUI)

### Chat SignalR

Demonstrates the use of [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-3.1) in an Uno Platform application. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ChatSignalR)

### Control Library  
An example of creating a custom control library and calling a control from your shared project.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ControlLibrary)

### Dual-Screen
A simple example using the `TwoPaneView` control spanned across dual screens (such as Neo or Duo dual-screen devices for example). 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/DualScreenSample)

### EmbeddedResources
An example that demonstrates the use of embedded resources and how to read them from your app.
Note that the [`Default namespace`](https://stackoverflow.com/questions/2871314/change-project-namespace-in-visual-studio) property of all projects is the same in order for the embedded resource names to be the same on all platforms.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/EmbeddedResources)

### FileSavePicker iOS

A working implementation of a folder-based save file picker for iOS. See [the 'iOS' section in the Windows.Storage.Pickers Uno documentation](https://platform.uno/docs/articles/features/windows-storage-pickers.html#ios) for more information.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/FileSavePickeriOS)

### HtmlControls

This is a WASM-only sample. It is creating _native_ HTML elements that can be used directly in XAML.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/HtmlControls)

### Localization Samples
A pair of samples related to localization:
- Localization: A sample showcasing the basics of localization.  
  [Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/LocalizationSamples/Localization)
  [Follow the tutorial](https://platform.uno/docs/articles/guides/localization.html)
- RuntimeCultureSwitching: An example of changing app language while it is running.  
  [Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/LocalizationSamples/RuntimeCultureSwitching)
  [Follow the tutorial](https://platform.uno/docs/articles/guides/hotswap-app-language.html)

### Map Control  
An implementation of the UWP `Maps` control with a custom slider that binds the value of the slider to the `ZoomLevel` property of the control. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MapControlSample)

### Native Frame Navigation
An example showcasing how to set up the native frame navigation for iOS and Android, and frame navigation in general for Uno.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/NativeFrameNav)

[Follow the tutorial](https://platform.uno/docs/articles/guides/native-frame-nav-tutorial.html)

### Native Style Switch  
An example of a toggle that allows you to switch between Native UI Controls and UWP UI Controls. The sample includes a checkbox, slider, button, and toggle. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/NativeStylesSwitch)

### Package Resources
An example that demonstrates the use of package assets and how to read them from your app. 

Note that for WebAssembly assets are downloaded on demand, as can be seen in the brower's network tab.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/PackageResources)

### SQLite  
This is a simple standalone app demonstrating the use of SQLite in an Uno application, including WebAssembly. It uses Erik Sink's [SQLitePCLRaw](https://github.com/ericsink/SQLitePCL.raw), and Frank Krueger's [sqlite-net](https://github.com/praeclarum/sqlite-net) libraries.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SQLiteSample)

### SkiaSharp Test  
An example of the Uno implementation of SkiaSharp creating a basic canvas with text.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SkiaSharpTest)

### Splash Screen Sample
An example showing how to set the splash/launch screen in Uno.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SplashScreenSample)

[Follow the tutorial](https://platform.uno/docs/articles/splash-screen.html)

### StatusBar Theme Color
An example showing how to adjust the `StatusBar` and `CommandBar` dynamically based on the current light/dark theme.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/StatusBarThemeColor)

[Follow the tutorial](https://platform.uno/docs/articles/guides/status-bar-theme-color.html)

### TheCatApiClient
An example demonstrating an approach to consuming REST web services in Uno using HttpClient.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/TheCatApiClient)

[Follow the tutorial](https://platform.uno/docs/articles/howto-consume-webservices.html)

### TimeEntry

Code for the Silverlight migration tutorial.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/TimeEntry)

[Follow the tutorial](https://platform.uno/docs/articles/silverlight-migration-landing.html)

### ToyCar
A proof of concept of a car animation using the `TwoPaneView` control spanned across dual screens (such as Neo or Duo dual-screen devices for example).
Inspiration from Justin Liu's [demo app](https://twitter.com/justinxinliu/status/1281123335410049027).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ToyCar)

### UnoContoso
A port of Microsoft's Contoso Enterprise UWP app to Uno Platform, using Prism.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoContoso)

### Uno.Cupertino Sample

An example showing how to set up the [`Uno.Cupertino`](https://github.com/unoplatform/Uno.Themes) library.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoCupertinoSample)

[Follow the tutorial](https://platform.uno/docs/articles/guides/uno-cupertino-walkthrough.html)

### Uno+Ethereum+Blockchain sample

A sample showing how to integrate smart contracts on the Ethereum blockchain with a multi-targeted Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoEthereumBlockChain)

### Uno.Material Sample
An example showing how to set up the [`Uno.Material`](https://github.com/unoplatform/Uno.Themes) library.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoMaterialSample)

[Follow the tutorial](https://platform.uno/docs/articles/guides/uno-material-walkthrough.html)

### WCT DataGrid  
A dynamic grid view ported from the Windows Community Toolkit that allows for x:Bind. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoWCTDataGridSample)

[Follow the tutorial](https://platform.uno/docs/articles/uno-community-toolkit.html)

### WCT DataGrid, TreeView, TabView

A combined Windows Community Toolkit sample showing the DataGrid, TreeView, and TabView controls in action.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/WCTDataTreeTabSample)

### WCT TabView
Ported from the Windows Community Toolkit, this sample shows an implementation of a `TabViewItem` in a shared container.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/WCTTabView)

### WebRTC

Demo of the usage of WebRTC in Uno WebAssembly. This sample establishes a direct WebRTC connection between 2 browsers and uses it to send messages between peers.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/WebRTC)

=======

Visit [our documentation](https://platform.uno/docs/articles/intro.html) for more details.
