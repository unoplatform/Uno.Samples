# The Uno Platform (Pronounced 'Oono' or 'Ouno') is a Universal Windows Platform Bridge that allows UWP-based code (C# and XAML) to run on iOS, Android, macOS, and WebAssembly

[![Twitter Followers](https://img.shields.io/twitter/follow/unoplatform?label=follow%20%40unoplatform&style=flat)](https://twitter.com/unoplatform)
[![GitHub Stars](https://img.shields.io/github/stars/unoplatform/uno?label=github%20stars)](https://github.com/unoplatform/uno/stargazers/)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](https://github.com/unoplatform/uno/blob/master/CONTRIBUTING.md)

This repository provides simple to-the-point samples for features of the Uno platform.

# What is the Uno Platform

The Uno Platform (Pronounced 'Oono' or 'Ouno') is a Universal Windows Platform Bridge that allows UWP-based code (C# and XAML) to run on iOS, Android, macOS, and WebAssembly. It provides the full definitions of the UWP [Windows 10 2004 (19041)](https://docs.microsoft.com/en-us/windows/uwp/whats-new/windows-10-build-19041), and the implementation of a growing number of parts of the UWP API, such as **Windows.UI.Xaml**, to enable UWP and WinUI applications to run on these platforms.

Use the UWP/WinUI tooling from Windows in [Visual Studio](https://www.visualstudio.com/), such as [XAML Edit and Continue](https://blogs.msdn.microsoft.com/visualstudio/2016/04/06/ui-development-made-easier-with-xaml-edit-continue/) and [C# Edit and Continue](https://docs.microsoft.com/en-us/visualstudio/debugger/how-to-use-edit-and-continue-csharp), build your application as much as possible on Windows, then validate that your application runs on iOS, Android, macOS and WebAssembly.

Visit [our documentation](https://platform.uno/docs/articles) for more details.

# Samples

### Android Custom Camera  
An android specific sample that shows how to start a camera capture intent, and display the result in an `Image` control.

### Auto Suggest  
An implementation of the XAML `AutoSuggest` control, showing how to autofill suggestions for user input. 

### Benchmark
An implementation of the .NET Benchmark Control, performance comparison tool

### Camera Capture UI  
A cross-platform implementation of the UWP `CameraCaptureUI` class that allows the user to capture audio, video, and photos from the device camera. 

### Control Library  
An example of creating a custom control library and calling a control from your shared project.

### Dual-Screen
A simple example using the `TwoPaneView` control spanned across dual screens (such as Neo or Duo dual-screen devices for example). 

### EmbeddedResources
An example that demonstrates the use of embedded resources and how to read them from your app.
Note that the [`Default namespace`](https://stackoverflow.com/questions/2871314/change-project-namespace-in-visual-studio) property of all projects is the same in order for the embedded resources names to be the same on all platforms.

## LocalizationSamples
A collection samples related to localization:
- Localization: A sample showcasing the basics of localization.  
  [Browse source](UI/Localization/Localization)
- RuntimeCultureSwitching: An example of changing app language while it is running.  
  [Browse source](UI/Localization/RuntimeCultureSwitching)

### Map Control  
An implementation of the UWP `Maps` control with a custom slider that binds the value of the slider to the `ZoomLevel` property of the control. 

### Native Frame Navigation
An example showcasing how to set up the native frame navigation for iOS and Android, and frame navigation in general for Uno.  
[Browse source](UI/NativeFrameNav)

### Native Style Switch  
An example of a toggle that allows you to switch between Native UI Controls and UWP UI Controls. The sample includes a checkbox, slider, button, and toggle. 

### Package Resources
An example that demonstrates the use of package assets and how to read them from your app. 

Note that for WebAssembly assets are downloaded on demand, as can be seen in the brower's network tab.

### StatusBar Theme Color
An example showing how to adjust the `StatusBar` and `CommandBar` dynamically based on the current light/dark theme.  
[Browse source](UI/StatusBarThemeColor)

### SkiaSharp Test  
An example of the Uno implementation of SkiaSharp creating a basic canvas with text.

### TheCatApiClient
An example demonstrating an approach to consuming REST web services in Uno using HttpClient.

### ToyCar
A proof of concept of a car animation using the `TwoPaneView` control spanned across dual screens (such as Neo or Duo dual-screen devices for example).
Inspiration from Justin Liu's [demo app](https://twitter.com/justinxinliu/status/1281123335410049027).

### SQLite  
This is a simple standalone app demonstrating the use of SQLite in an Uno application, including WebAssembly. It uses Erik Sink's [SQLitePCLRaw](https://github.com/ericsink/SQLitePCL.raw), and Frank Krueger's [sqlite-net](https://github.com/praeclarum/sqlite-net) libraries.

### UnoContoso
This is a project that ported Microsoft's Contoso Enterprise UWP app to Uno Platform Prism.

### WCT DataGrid  
A dynamic grid view ported from the Windows Community Toolkit that allows for x:Bind. 

### WCT TabView
Ported from the Windows Community Toolkit, this sample shows an implementation of a `TabViewItem` in a shared container.

=======
Visit [our documentation](https://github.com/unoplatform/uno/blob/master/doc/articles/intro.md) for more details.
