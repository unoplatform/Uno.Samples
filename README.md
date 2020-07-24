# Uno Platform Samples - The UWP Bridge for iOS, Android and WebAssembly

[![Gitter](https://badges.gitter.im/uno-platform/Lobby.svg)](https://gitter.im/uno-platform/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

This repository provides simple to-the-point samples for features of the Uno platform.

# What is the Uno Platform

The Uno Platform is a Universal Windows Platform Bridge to allow UWP based code to run on iOS, Android, and WebAssembly. It provides the full definitions of the UWP Spring Creators Update (17134), and the implementation of growing number parts of the UWP API, such as **Windows.UI.Xaml**, to enable applications to run on these platforms.

Use the UWP tooling from Windows in [Visual Studio](https://www.visualstudio.com/), such as [Xaml Edit and Continue](https://blogs.msdn.microsoft.com/visualstudio/2016/04/06/ui-development-made-easier-with-xaml-edit-continue/) and [C# Edit and Continue](https://docs.microsoft.com/en-us/visualstudio/debugger/how-to-use-edit-and-continue-csharp), build your application as much as possible on Windows, then validate that your application runs on iOS, Android and WebAssembly.

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

### Map Control  
An implementation of the UWP `Maps` control with a custom slider that binds the value of the slider to the `ZoomLevel` property of the control. 

### Native Style Switch  
An example of a toggle that allows you to switch between Native UI Controls and UWP UI Controls. The sample includes a checkbox, slider, button, and toggle. 

### SkiaSharp Test  
An example of the Uno implementation of SkiaSharp creating a basic canvas with text.

### SQLite  
This is a simple standalone app demonstrating the use of SQLite in an Uno application, including WebAssembly. It uses Erik Sink's [SQLitePCLRaw](https://github.com/ericsink/SQLitePCL.raw), and Frank Krueger's [sqlite-net](https://github.com/praeclarum/sqlite-net) libraries.

### WCT DataGrid  
A dynamic grid view ported from the Windows Community Toolkit that allows for x:Bind. 

### WCT TabView  
Ported from the Windows Community Toolkit, this sample shows an implementation of a `TabViewItem` in a shared container.