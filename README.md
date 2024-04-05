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

---
uid: Uno.Samples.List
---

## Samples

### Commerce App

The Commerce App is a sample application that demonstrates the use of ListFeed pagination, Feedviews, and other features provided by Uno.Extensions. It illustrates how these features can be applied to create an application complete with a shopping cart, products, and more.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/Commerce)

### SimpleCalc App

The SimpleCalc App is a sample application designed to perform basic arithmetic operations. App was built using the four variants of the [Simple Calc workshop](https://aka.platform.uno/simplecalc-workshop), combining different markup languages (XAML or C# Markup) and presentation frameworks (MVVM or MVUX).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/SimpleCalc)

### Counter App

Experience the simplicity and power of Uno.Extensions through the Counter App, a straightforward yet powerful demonstration of both basic and advanced features of the Uno Platform. This app provides a hands-on experience with fundamental concepts such as state management, user interaction, and real-time UI updates.
This sample app was built using the four variants of the [Counter workshop](https://aka.platform.uno/counter-tutorial), combining markup language (XAML or C# Markup) and presentation framework (MVVM or MVUX).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/Counter)

### ToDo App

Dive into the essentials of task management with the ToDo App, a meticulously crafted sample application that highlights the power and flexibility of Uno.Extensions. By emphasizing the creation and organization of to-do lists, this app showcases practical applications of essential Uno.Extensions features, offering a hands-on experience in crafting responsive and user-friendly interfaces across multiple platforms.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/ToDo)

### TubePlayer App

The TubePlayer App is a sample application that allows users to search for, and stream Youtube videos. This app was created using the tools, libraries, and patterns provided by the Uno Platform, designed to facilitate the rapid development of high-quality applications.
This sample app was built following the [Tube Player workshop](https://aka.platform.uno/tubeplayer-workshop).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/TubePlayer)

### Advanced XBind

The {x:Bind} markup extension�new for Windows 10�is an alternative to {Binding}. {x:Bind} 
runs in less time and less memory than {Binding} and supports better debugging.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/AdvancedXBind)

### Android Custom Camera  
An Android-specific sample that shows how to start a camera capture intent, and display the result in an `Image` control.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/AndroidCustomCamera)

### Authentication with OpenID Connect (OIDC)

This sample application demonstrates the usage  of the `WebAuthenticationBroker` in Uno with an OpenID Connect endpoint.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/Authentication.OidcDemo)

[Follow the tutorial](https://platform.uno/docs/articles/guides/open-id-connect.html)

### Auto-Suggest  
An implementation of the XAML `AutoSuggest` control, showing how to autofill suggestions for user input. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/AutoSuggestSample)

### Benchmark
An implementation of the .NET Benchmark Control, a performance comparison tool.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/Benchmark)

### BluetoothExplorer
A sample that allows the user to search for nearby Bluetooth connections and connect to a device of their choice. Uses [InTheHand.BluetoothLE](https://www.nuget.org/packages/InTheHand.BluetoothLE)

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/BluetoothExplorer) 

### Camera Capture UI  
A cross-platform implementation of the UWP `CameraCaptureUI` class that allows the user to capture audio, video, and photos from the device camera. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CameraCaptureUI)

### CardView Migration
An Uno conversion of the Xamarin `CardView` sample showing how to migrate a custom control. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CardViewMigration)

### ChatGPT

A ChatGPT sample using OpenAI SDK with C# Markup, MVUX and immutable records.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ChatGPT)

### Chat SignalR

Demonstrates the use of [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-3.1) in an Uno Platform application. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ChatSignalR)

### Control Library  
An example of creating a custom control library and calling a control from your shared project.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ControlLibrary)

### Country Data

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CountryDataSample)


### Custom Sorting
Sample app to accompany "Adding Custom Sorting Logic" blog post.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CustomSorting)

### Dual-Screen
A simple example using the `TwoPaneView` control spanned across dual screens (such as Neo or Duo dual-screen devices for example). 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/DualScreenSample)

### EmbeddedResources
An example that demonstrates the use of embedded resources and how to read them from your app.
Note that the [`Default namespace`](https://stackoverflow.com/questions/2871314/change-project-namespace-in-visual-studio) property of all projects is the same in order for the embedded resource names to be the same on all platforms.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/EmbeddedResources)

### Entity Framework Core Demo

An example of Entity Framework Core 7 with a SQLite storage for WebAssembly, WinAppSDK, iOS and Android.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/EFCoreSQLiteSample)

### FileSavePicker iOS

A working implementation of a folder-based save file picker for iOS. See [the 'iOS' section in the Windows.Storage.Pickers Uno documentation](https://platform.uno/docs/articles/features/windows-storage-pickers.html#ios) for more information.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/FileSavePickeriOS)

### Food Delivery

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/FoodDeliveryUI)

### HtmlControls

This is a WASM-only sample. It is creating _native_ HTML elements that can be used directly in XAML.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/HtmlControls)

### LiteDB

This is an example that utilizes the [LiteDB NuGet package](http://www.litedb.org/) to save data.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/LiteDB)

### Inserting Separators

This example demonstrates the dynamic creation of a menu incorporating nested items as well as items with separators. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/InsertingSeparators)

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

### .NET MAUI Embedding  
Multiple samples that demonstrate third-party control libraries embedded in Uno Platform applications using .NET MAUI Embedding.
Note that these controls work only for target platforms .NET MAUI reaches – iOS, Android, MacOS, and Windows.

#### ArcGIS Maps SDK for .NET

Embeds the [ArcGIS Maps SDK for .NET](https://developers.arcgis.com/net/) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/ArcGisApp)

#### Esri ArcGIS Maps SDK for .NET

Embeds the [Esri ArcGIS Maps SDK for .NET](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-EsriMaps.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/ArcGisApp)

#### DevExpress .NET MAUI Controls

Embeds the [DevExpress .NET MAUI Controls](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-DevExpres.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/DevExpressApp)

#### GrapeCity ComponentOne .NET MAUI Controls

Embeds the [GrapeCity ComponentOne .NET MAUI Controls](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-GrapeCity.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/GrapeCityApp)

#### Grial UI Kit

Embeds the [Grial UI Kit](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-GrialKit.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/GrialKitApp)

#### .NET MAUI Community Toolkit  

Embeds the [.NET MAUI Community Toolkit](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/MauiCommunityToolkitApp)

#### DevExpress .NET MAUI Controls

Embeds the [DevExpress .NET MAUI Controls](https://www.devexpress.com/maui/) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/DevExpressApp)

#### GrapeCity ComponentOne .NET MAUI Controls 

Embeds the [GrapeCity ComponentOne .NET MAUI Controls](https://www.grapecity.com/componentone/docs/maui/online-maui/overview.html) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/GrapeCityApp)

#### Grial UI Kit for .NET MAUI

Embeds the [Grial UI Kit for .NET MAUI](https://grialkit.com/) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/GrialKitApp)

#### Syncfusion .NET MAUI Controls

Embeds the [Syncfusion .NET MAUI Controls](https://www.syncfusion.com/maui-controls) in an Uno Platform application.
#### Syncfusion .NET MAUI Controls

Embeds the [Syncfusion .NET MAUI Controls](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-Synfusion.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/SyncfusionApp)

#### Telerik UI for .NET MAUI

Embeds the [Telerik UI for .NET MAUI](https://www.telerik.com/maui-ui) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/TelerikApp)
Embeds the [Telerik UI for .NET MAUI](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-Telerik.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/TelerikApp)

### Migrating Xamarin.Forms Animations
Code to accompany the [blog post](https://platform.uno/blog/migrating-animations-from-xamarin-forms-to-uno-platform/) on migrating animations from Xamarin Forms to Uno.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MigratingAnimations)

### Migrating Xamarin.Forms Effects
Code samples to accompany the [blog post](https://platform.uno/blog/xamarin-forms-migration-to-uno-platform-effects-and-alternative-approaches/) on Migrating from Xamarin.Forms Effects
- XamarinFormsEffect: A reference implementation of an effect with an Android implementation.
  [Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MigratingEffects/XamarinFormsEffect)
- UnoEffectSample: Showcasing how to replace Effects with either ControlTemplates or custom code accessing the native control.
  [Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MigratingEffects/UnoEffectsSample)
  

### Native Frame Navigation
An example showcasing how to set up the native frame navigation for iOS and Android, and frame navigation in general for Uno.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/NativeFrameNav)

[Follow the tutorial](https://platform.uno/docs/articles/guides/native-frame-nav-tutorial.html)

### Native Style Switch  
An example of a toggle that allows you to switch between Native UI Controls and UWP UI Controls. The sample includes a checkbox, slider, button, and toggle. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/NativeStylesSwitch)

### Neumorphism  
An example of an app containing an animated lock that you can unlock.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/Neumorphism)

### Package Resources
An example that demonstrates the use of package assets and how to read them from your app. 

Note that for WebAssembly assets are downloaded on demand, as can be seen in the browser's network tab.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/PackageResources)

### Pet Adopt
An example that demonstrates the use of pipspager with a flipview, in an app created with the help of the Figma-to-XAML plugin. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/PetAdoptUI)

### SkiaSharp Test  
An example of the Uno implementation of SkiaSharp creating a basic canvas with text.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SkiaSharpTest)

### SkiaSharp Skottie: Lottie file player

This samples shows the use of the SkiaSharp.Skottie component, which allows the playback of [lottie files](https://airbnb.design/lottie).

This component provides Lottie support for all available Uno Platform targets.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SkottieSample)

### Splash Screen Sample
An example showing how to set the splash/launch screen in Uno.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SplashScreenSample)

[Follow the tutorial](https://platform.uno/docs/articles/splash-screen.html)

### SQLite  
This is a simple standalone app demonstrating the use of SQLite in an Uno application, including WebAssembly. It uses Erik Sink's [SQLitePCLRaw](https://github.com/ericsink/SQLitePCL.raw), and Frank Krueger's [sqlite-net](https://github.com/praeclarum/sqlite-net) libraries.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SQLiteSample)

### StatusBar Theme Color
An example showing how to adjust the `StatusBar` and `CommandBar` dynamically based on the current light/dark theme.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/StatusBarThemeColor)

[Follow the tutorial](https://platform.uno/docs/articles/guides/status-bar-theme-color.html)

### The Cat Api Client
An example demonstrating an approach to consuming REST web services in Uno using HttpClient.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/TheCatApiClient)

[Follow the tutorial](https://platform.uno/docs/articles/howto-consume-webservices.html)

### Time Entry

Code for the Silverlight migration tutorial.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/TimeEntry)

[Follow the tutorial](https://platform.uno/docs/articles/silverlight-migration-landing.html)

### Toy Car
A proof of concept of a car animation using the `TwoPaneView` control spanned across dual screens (such as Neo or Duo dual-screen devices for example).
Inspiration from Justin Liu's [demo app](https://twitter.com/justinxinliu/status/1281123335410049027).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ToyCar)

### Travel UI
A sample travel app that shows how a user could 1) search for locations, 2) favorite locations and 3) view their profile as well as others' profiles. For the layout, showcases many ListViews coupled with Grids.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/TravelUI)

### Uno BackgroundWorker: Background Work

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoBackgroundWorker)

### Uno Cakes Mobile
A port of Shaw Yu's Cakes Mobile App from [XampleUI](https://github.com/shawyunz/XampleUI) to Uno Platform.
Used to demonstrate simple page navigation from View and ViewModel.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoCakesMobile)

### Uno Contoso
A port of Microsoft's Contoso Enterprise UWP app to Uno Platform, using Prism.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoContoso)

### Uno Cupertino Theme

An example showing how to set up the [`Uno.Cupertino`](https://github.com/unoplatform/Uno.Themes) library.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoCupertinoSample)

[Consult the documentation](https://platform.uno/docs/articles/external/uno.themes/doc/cupertino-getting-started.html)

### Uno Ethereum+Blockchain

A sample showing how to integrate smart contracts on the Ethereum blockchain with a multi-targeted Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoEthereumBlockChain)

### Uno GoodReads

A sample showing how to make an app containing several pages in a TabView, as well as fake data generation to populate those pages. The blog post series for this app includes parts on [creating the home page](https://platform.uno/blog/recreating-amazon-goodreads-app-home-page-using-material-ui-figma-and-uno-platform/), [creating the author page](https://platform.uno/blog/how-to-quickly-build-goodreads-author-page-with-figma-and-uno-platform/), [creating the books page](https://platform.uno/blog/replicating-goodreads-detail-page-in-figma-with-uno-platform/), and [code generation with Figma](https://platform.uno/blog/from-figma-to-visual-studio-adding-back-end-logic-to-goodreads-app/).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoGoodReads)

### Uno Islands

This sample shows how you can integrate Uno Platform XAML controls into existing WPF applications using Uno Islands. This feature allows you to enhance WPF apps with Uno Platform features by hosting Uno Platform XAML files in a Shared project and adding an Uno Island using the UnoXamlHost control.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoIslandsSampleApp)

[Follow the tutorial](https://platform.uno/docs/articles/guides/uno-islands.html)

### Uno Material Theme
An example showing how to set up the [`Uno.Material`](https://github.com/unoplatform/Uno.Themes) library.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoMaterialSample)

[Consult the documentation](https://platform.uno/docs/articles/external/uno.themes/doc/material-getting-started.html)

### Uno Onnx

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoOnnxSamples)

### Uno Scroll Reveal

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoScrollReveal)

### Uno SQlite One Drive Invoice

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoSQLiteOneDriveInvoiceSample)

### Uno Toolkit Material
An example showing how to set up the [`Uno.Toolkit.Material`](https://github.com/unoplatform/uno.toolkit.ui/tree/main/src/library/Uno.Toolkit.Material) library, 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoMaterialToolkitSample)

[Consult the documentation](https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/material-getting-started.html)

### WCT DataGrid  
A dynamic grid view ported from the Windows Community Toolkit that allows for x:Bind. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoWCTDataGridSample)

[Follow the tutorial](https://platform.uno/docs/articles/uno-community-toolkit.html)

### WCT DataGrid, TreeView, TabView

A combined Windows Community Toolkit sample showing the DataGrid, TreeView, and TabView controls in action.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/WCTDataTreeTabSample)

### WebRTC

Demo of the usage of WebRTC in Uno WebAssembly. This sample establishes a direct WebRTC connection between 2 browsers and uses it to send messages between peers.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/WebRTC)

### XAML Basics : ListView

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/XamlBasics/ListViewSample)


Visit [our documentation](https://platform.uno/docs/articles/intro.html) for more details.

### XamlBrewer SkiaSharp
Port of the XAML Brewer WinUI3 SkiaSharp Sample application ([blog post](https://xamlbrewer.wordpress.com/2023/09/25/getting-started-with-skiasharp-in-winui-3/) and [source code](https://github.com/XamlBrewer/XamlBrewer.WinUI3.SkiaSharp.Sample))

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/XamlBrewerUnoApp)
uid: Uno.Samples.List
---

## Samples

### Commerce App

The Commerce App is a sample application that demonstrates the use of ListFeed pagination, Feedviews, and other features provided by Uno.Extensions. It illustrates how these features can be applied to create an application complete with a shopping cart, products, and more.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/Commerce)

### SimpleCalc App

The SimpleCalc App is a sample application designed to perform basic arithmetic operations. App was built using the four variants of the [Simple Calc workshop](https://aka.platform.uno/simplecalc-workshop), combining different markup languages (XAML or C# Markup) and presentation frameworks (MVVM or MVUX).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/SimpleCalc)

### Counter App

Experience the simplicity and power of Uno.Extensions through the Counter App, a straightforward yet powerful demonstration of both basic and advanced features of the Uno Platform. This app provides a hands-on experience with fundamental concepts such as state management, user interaction, and real-time UI updates.
This sample app was built using the four variants of the [Counter workshop](https://aka.platform.uno/counter-tutorial), combining markup language (XAML or C# Markup) and presentation framework (MVVM or MVUX).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/Counter)

### ToDo App

Dive into the essentials of task management with the ToDo App, a meticulously crafted sample application that highlights the power and flexibility of Uno.Extensions. By emphasizing the creation and organization of to-do lists, this app showcases practical applications of essential Uno.Extensions features, offering a hands-on experience in crafting responsive and user-friendly interfaces across multiple platforms.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/ToDo)

### TubePlayer App

The TubePlayer App is a sample application that allows users to search for, and stream Youtube videos. This app was created using the tools, libraries, and patterns provided by the Uno Platform, designed to facilitate the rapid development of high-quality applications.
This sample app was built following the [Tube Player workshop](https://aka.platform.uno/tubeplayer-workshop).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/TubePlayer)

### Advanced XBind

The {x:Bind} markup extension�new for Windows 10�is an alternative to {Binding}. {x:Bind} 
runs in less time and less memory than {Binding} and supports better debugging.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/AdvancedXBind)

### Android Custom Camera  
An Android-specific sample that shows how to start a camera capture intent, and display the result in an `Image` control.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/AndroidCustomCamera)

### Authentication with OpenID Connect (OIDC)

This sample application demonstrates the usage  of the `WebAuthenticationBroker` in Uno with an OpenID Connect endpoint.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/Authentication.OidcDemo)

[Follow the tutorial](https://platform.uno/docs/articles/guides/open-id-connect.html)

### Auto-Suggest  
An implementation of the XAML `AutoSuggest` control, showing how to autofill suggestions for user input. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/AutoSuggestSample)

### Benchmark
An implementation of the .NET Benchmark Control, a performance comparison tool.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/Benchmark)

### BluetoothExplorer
A sample that allows the user to search for nearby Bluetooth connections and connect to a device of their choice. Uses [InTheHand.BluetoothLE](https://www.nuget.org/packages/InTheHand.BluetoothLE)

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/BluetoothExplorer) 

### Camera Capture UI  
A cross-platform implementation of the UWP `CameraCaptureUI` class that allows the user to capture audio, video, and photos from the device camera. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CameraCaptureUI)

### CardView Migration
An Uno conversion of the Xamarin `CardView` sample showing how to migrate a custom control. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CardViewMigration)

### ChatGPT

A ChatGPT sample using OpenAI SDK with C# Markup, MVUX and immutable records.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ChatGPT)

### Chat SignalR

Demonstrates the use of [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-3.1) in an Uno Platform application. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ChatSignalR)

### Control Library  
An example of creating a custom control library and calling a control from your shared project.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ControlLibrary)

### Country Data

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CountryDataSample)


### Custom Sorting
Sample app to accompany "Adding Custom Sorting Logic" blog post.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CustomSorting)

### Dual-Screen
A simple example using the `TwoPaneView` control spanned across dual screens (such as Neo or Duo dual-screen devices for example). 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/DualScreenSample)

### EmbeddedResources
An example that demonstrates the use of embedded resources and how to read them from your app.
Note that the [`Default namespace`](https://stackoverflow.com/questions/2871314/change-project-namespace-in-visual-studio) property of all projects is the same in order for the embedded resource names to be the same on all platforms.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/EmbeddedResources)

### Entity Framework Core Demo

An example of Entity Framework Core 7 with a SQLite storage for WebAssembly, WinAppSDK, iOS and Android.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/EFCoreSQLiteSample)

### FileSavePicker iOS

A working implementation of a folder-based save file picker for iOS. See [the 'iOS' section in the Windows.Storage.Pickers Uno documentation](https://platform.uno/docs/articles/features/windows-storage-pickers.html#ios) for more information.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/FileSavePickeriOS)

### Food Delivery

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/FoodDeliveryUI)

### HtmlControls

This is a WASM-only sample. It is creating _native_ HTML elements that can be used directly in XAML.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/HtmlControls)

### LiteDB

This is an example that utilizes the [LiteDB NuGet package](http://www.litedb.org/) to save data.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/LiteDB)

### Inserting Separators

This example demonstrates the dynamic creation of a menu incorporating nested items as well as items with separators. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/InsertingSeparators)

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

### .NET MAUI Embedding  
Multiple samples that demonstrate third-party control libraries embedded in Uno Platform applications using .NET MAUI Embedding.
Note that these controls work only for target platforms .NET MAUI reaches – iOS, Android, MacOS, and Windows.

#### ArcGIS Maps SDK for .NET

Embeds the [ArcGIS Maps SDK for .NET](https://developers.arcgis.com/net/) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/ArcGisApp)

#### Esri ArcGIS Maps SDK for .NET

Embeds the [Esri ArcGIS Maps SDK for .NET](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-EsriMaps.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/ArcGisApp)

#### DevExpress .NET MAUI Controls

Embeds the [DevExpress .NET MAUI Controls](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-DevExpres.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/DevExpressApp)

#### GrapeCity ComponentOne .NET MAUI Controls

Embeds the [GrapeCity ComponentOne .NET MAUI Controls](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-GrapeCity.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/GrapeCityApp)

#### Grial UI Kit

Embeds the [Grial UI Kit](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-GrialKit.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/GrialKitApp)

#### .NET MAUI Community Toolkit  

Embeds the [.NET MAUI Community Toolkit](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/MauiCommunityToolkitApp)

#### DevExpress .NET MAUI Controls

Embeds the [DevExpress .NET MAUI Controls](https://www.devexpress.com/maui/) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/DevExpressApp)

#### GrapeCity ComponentOne .NET MAUI Controls 

Embeds the [GrapeCity ComponentOne .NET MAUI Controls](https://www.grapecity.com/componentone/docs/maui/online-maui/overview.html) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/GrapeCityApp)

#### Grial UI Kit for .NET MAUI

Embeds the [Grial UI Kit for .NET MAUI](https://grialkit.com/) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/GrialKitApp)

#### Syncfusion .NET MAUI Controls

Embeds the [Syncfusion .NET MAUI Controls](https://www.syncfusion.com/maui-controls) in an Uno Platform application.
#### Syncfusion .NET MAUI Controls

Embeds the [Syncfusion .NET MAUI Controls](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-Synfusion.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/SyncfusionApp)

#### Telerik UI for .NET MAUI

Embeds the [Telerik UI for .NET MAUI](https://www.telerik.com/maui-ui) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/TelerikApp)
Embeds the [Telerik UI for .NET MAUI](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-Telerik.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/TelerikApp)

### Migrating Xamarin.Forms Animations
Code to accompany the [blog post](https://platform.uno/blog/migrating-animations-from-xamarin-forms-to-uno-platform/) on migrating animations from Xamarin Forms to Uno.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MigratingAnimations)

### Migrating Xamarin.Forms Effects
Code samples to accompany the [blog post](https://platform.uno/blog/xamarin-forms-migration-to-uno-platform-effects-and-alternative-approaches/) on Migrating from Xamarin.Forms Effects
- XamarinFormsEffect: A reference implementation of an effect with an Android implementation.
  [Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MigratingEffects/XamarinFormsEffect)
- UnoEffectSample: Showcasing how to replace Effects with either ControlTemplates or custom code accessing the native control.
  [Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MigratingEffects/UnoEffectsSample)
  

### Native Frame Navigation
An example showcasing how to set up the native frame navigation for iOS and Android, and frame navigation in general for Uno.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/NativeFrameNav)

[Follow the tutorial](https://platform.uno/docs/articles/guides/native-frame-nav-tutorial.html)

### Native Style Switch  
An example of a toggle that allows you to switch between Native UI Controls and UWP UI Controls. The sample includes a checkbox, slider, button, and toggle. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/NativeStylesSwitch)

### Neumorphism  
An example of an app containing an animated lock that you can unlock.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/Neumorphism)

### Package Resources
An example that demonstrates the use of package assets and how to read them from your app. 

Note that for WebAssembly assets are downloaded on demand, as can be seen in the browser's network tab.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/PackageResources)

### Pet Adopt
An example that demonstrates the use of pipspager with a flipview, in an app created with the help of the Figma-to-XAML plugin. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/PetAdoptUI)

### SkiaSharp Test  
An example of the Uno implementation of SkiaSharp creating a basic canvas with text.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SkiaSharpTest)

### SkiaSharp Skottie: Lottie file player

This samples shows the use of the SkiaSharp.Skottie component, which allows the playback of [lottie files](https://airbnb.design/lottie).

This component provides Lottie support for all available Uno Platform targets.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SkottieSample)

### Splash Screen Sample
An example showing how to set the splash/launch screen in Uno.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SplashScreenSample)

[Follow the tutorial](https://platform.uno/docs/articles/splash-screen.html)

### SQLite  
This is a simple standalone app demonstrating the use of SQLite in an Uno application, including WebAssembly. It uses Erik Sink's [SQLitePCLRaw](https://github.com/ericsink/SQLitePCL.raw), and Frank Krueger's [sqlite-net](https://github.com/praeclarum/sqlite-net) libraries.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SQLiteSample)

### StatusBar Theme Color
An example showing how to adjust the `StatusBar` and `CommandBar` dynamically based on the current light/dark theme.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/StatusBarThemeColor)

[Follow the tutorial](https://platform.uno/docs/articles/guides/status-bar-theme-color.html)

### The Cat Api Client
An example demonstrating an approach to consuming REST web services in Uno using HttpClient.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/TheCatApiClient)

[Follow the tutorial](https://platform.uno/docs/articles/howto-consume-webservices.html)

### Time Entry

Code for the Silverlight migration tutorial.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/TimeEntry)

[Follow the tutorial](https://platform.uno/docs/articles/silverlight-migration-landing.html)

### Toy Car
A proof of concept of a car animation using the `TwoPaneView` control spanned across dual screens (such as Neo or Duo dual-screen devices for example).
Inspiration from Justin Liu's [demo app](https://twitter.com/justinxinliu/status/1281123335410049027).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ToyCar)

### Travel UI
A sample travel app that shows how a user could 1) search for locations, 2) favorite locations and 3) view their profile as well as others' profiles. For the layout, showcases many ListViews coupled with Grids.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/TravelUI)

### Uno BackgroundWorker: Background Work

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoBackgroundWorker)

### Uno Cakes Mobile
A port of Shaw Yu's Cakes Mobile App from [XampleUI](https://github.com/shawyunz/XampleUI) to Uno Platform.
Used to demonstrate simple page navigation from View and ViewModel.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoCakesMobile)

### Uno Contoso
A port of Microsoft's Contoso Enterprise UWP app to Uno Platform, using Prism.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoContoso)

### Uno Cupertino Theme

An example showing how to set up the [`Uno.Cupertino`](https://github.com/unoplatform/Uno.Themes) library.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoCupertinoSample)

[Consult the documentation](https://platform.uno/docs/articles/external/uno.themes/doc/cupertino-getting-started.html)

### Uno Ethereum+Blockchain

A sample showing how to integrate smart contracts on the Ethereum blockchain with a multi-targeted Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoEthereumBlockChain)

### Uno GoodReads

A sample showing how to make an app containing several pages in a TabView, as well as fake data generation to populate those pages. The blog post series for this app includes parts on [creating the home page](https://platform.uno/blog/recreating-amazon-goodreads-app-home-page-using-material-ui-figma-and-uno-platform/), [creating the author page](https://platform.uno/blog/how-to-quickly-build-goodreads-author-page-with-figma-and-uno-platform/), [creating the books page](https://platform.uno/blog/replicating-goodreads-detail-page-in-figma-with-uno-platform/), and [code generation with Figma](https://platform.uno/blog/from-figma-to-visual-studio-adding-back-end-logic-to-goodreads-app/).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoGoodReads)

### Uno Islands

This sample shows how you can integrate Uno Platform XAML controls into existing WPF applications using Uno Islands. This feature allows you to enhance WPF apps with Uno Platform features by hosting Uno Platform XAML files in a Shared project and adding an Uno Island using the UnoXamlHost control.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoIslandsSampleApp)

[Follow the tutorial](https://platform.uno/docs/articles/guides/uno-islands.html)

### Uno Material Theme
An example showing how to set up the [`Uno.Material`](https://github.com/unoplatform/Uno.Themes) library.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoMaterialSample)

[Consult the documentation](https://platform.uno/docs/articles/external/uno.themes/doc/material-getting-started.html)

### Uno Onnx

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoOnnxSamples)

### Uno Scroll Reveal

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoScrollReveal)

### Uno SQlite One Drive Invoice

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoSQLiteOneDriveInvoiceSample)

### Uno Toolkit Material
An example showing how to set up the [`Uno.Toolkit.Material`](https://github.com/unoplatform/uno.toolkit.ui/tree/main/src/library/Uno.Toolkit.Material) library, 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoMaterialToolkitSample)

[Consult the documentation](https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/material-getting-started.html)

### WCT DataGrid  
A dynamic grid view ported from the Windows Community Toolkit that allows for x:Bind. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoWCTDataGridSample)

[Follow the tutorial](https://platform.uno/docs/articles/uno-community-toolkit.html)

### WCT DataGrid, TreeView, TabView

A combined Windows Community Toolkit sample showing the DataGrid, TreeView, and TabView controls in action.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/WCTDataTreeTabSample)

### WebRTC

Demo of the usage of WebRTC in Uno WebAssembly. This sample establishes a direct WebRTC connection between 2 browsers and uses it to send messages between peers.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/WebRTC)

### XAML Basics : ListView

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/XamlBasics/ListViewSample)


Visit [our documentation](https://platform.uno/docs/articles/intro.html) for more details.

### XamlBrewer SkiaSharp
Port of the XAML Brewer WinUI3 SkiaSharp Sample application ([blog post](https://xamlbrewer.wordpress.com/2023/09/25/getting-started-with-skiasharp-in-winui-3/) and [source code](https://github.com/XamlBrewer/XamlBrewer.WinUI3.SkiaSharp.Sample))

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/XamlBrewerUnoApp)
uid: Uno.Samples.List
---

## Samples

### Commerce App

The Commerce App is a sample application that demonstrates the use of ListFeed pagination, Feedviews, and other features provided by Uno.Extensions. It illustrates how these features can be applied to create an application complete with a shopping cart, products, and more.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/Commerce)

### SimpleCalc App

The SimpleCalc App is a sample application designed to perform basic arithmetic operations. App was built using the four variants of the [Simple Calc workshop](https://aka.platform.uno/simplecalc-workshop), combining different markup languages (XAML or C# Markup) and presentation frameworks (MVVM or MVUX).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/SimpleCalc)

### Counter App

Experience the simplicity and power of Uno.Extensions through the Counter App, a straightforward yet powerful demonstration of both basic and advanced features of the Uno Platform. This app provides a hands-on experience with fundamental concepts such as state management, user interaction, and real-time UI updates.
This sample app was built using the four variants of the [Counter workshop](https://aka.platform.uno/counter-tutorial), combining markup language (XAML or C# Markup) and presentation framework (MVVM or MVUX).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/Counter)

### ToDo App

Dive into the essentials of task management with the ToDo App, a meticulously crafted sample application that highlights the power and flexibility of Uno.Extensions. By emphasizing the creation and organization of to-do lists, this app showcases practical applications of essential Uno.Extensions features, offering a hands-on experience in crafting responsive and user-friendly interfaces across multiple platforms.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/ToDo)

### TubePlayer App

The TubePlayer App is a sample application that allows users to search for, and stream Youtube videos. This app was created using the tools, libraries, and patterns provided by the Uno Platform, designed to facilitate the rapid development of high-quality applications.
This sample app was built following the [Tube Player workshop](https://aka.platform.uno/tubeplayer-workshop).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/TubePlayer)

### Advanced XBind

The {x:Bind} markup extension�new for Windows 10�is an alternative to {Binding}. {x:Bind} 
runs in less time and less memory than {Binding} and supports better debugging.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/AdvancedXBind)

### Android Custom Camera  
An Android-specific sample that shows how to start a camera capture intent, and display the result in an `Image` control.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/AndroidCustomCamera)

### Authentication with OpenID Connect (OIDC)

This sample application demonstrates the usage  of the `WebAuthenticationBroker` in Uno with an OpenID Connect endpoint.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/Authentication.OidcDemo)

[Follow the tutorial](https://platform.uno/docs/articles/guides/open-id-connect.html)

### Auto-Suggest  
An implementation of the XAML `AutoSuggest` control, showing how to autofill suggestions for user input. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/AutoSuggestSample)

### Benchmark
An implementation of the .NET Benchmark Control, a performance comparison tool.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/Benchmark)

### BluetoothExplorer
A sample that allows the user to search for nearby Bluetooth connections and connect to a device of their choice. Uses [InTheHand.BluetoothLE](https://www.nuget.org/packages/InTheHand.BluetoothLE)

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/BluetoothExplorer) 

### Camera Capture UI  
A cross-platform implementation of the UWP `CameraCaptureUI` class that allows the user to capture audio, video, and photos from the device camera. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CameraCaptureUI)

### CardView Migration
An Uno conversion of the Xamarin `CardView` sample showing how to migrate a custom control. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CardViewMigration)

### ChatGPT

A ChatGPT sample using OpenAI SDK with C# Markup, MVUX and immutable records.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ChatGPT)

### Chat SignalR

Demonstrates the use of [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-3.1) in an Uno Platform application. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ChatSignalR)

### Control Library  
An example of creating a custom control library and calling a control from your shared project.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ControlLibrary)

### Country Data

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CountryDataSample)


### Custom Sorting
Sample app to accompany "Adding Custom Sorting Logic" blog post.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CustomSorting)

### Dual-Screen
A simple example using the `TwoPaneView` control spanned across dual screens (such as Neo or Duo dual-screen devices for example). 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/DualScreenSample)

### EmbeddedResources
An example that demonstrates the use of embedded resources and how to read them from your app.
Note that the [`Default namespace`](https://stackoverflow.com/questions/2871314/change-project-namespace-in-visual-studio) property of all projects is the same in order for the embedded resource names to be the same on all platforms.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/EmbeddedResources)

### Entity Framework Core Demo

An example of Entity Framework Core 7 with a SQLite storage for WebAssembly, WinAppSDK, iOS and Android.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/EFCoreSQLiteSample)

### FileSavePicker iOS

A working implementation of a folder-based save file picker for iOS. See [the 'iOS' section in the Windows.Storage.Pickers Uno documentation](https://platform.uno/docs/articles/features/windows-storage-pickers.html#ios) for more information.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/FileSavePickeriOS)

### Food Delivery

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/FoodDeliveryUI)

### HtmlControls

This is a WASM-only sample. It is creating _native_ HTML elements that can be used directly in XAML.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/HtmlControls)

### LiteDB

This is an example that utilizes the [LiteDB NuGet package](http://www.litedb.org/) to save data.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/LiteDB)

### Inserting Separators

This example demonstrates the dynamic creation of a menu incorporating nested items as well as items with separators. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/InsertingSeparators)

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

### .NET MAUI Embedding  
Multiple samples that demonstrate third-party control libraries embedded in Uno Platform applications using .NET MAUI Embedding.
Note that these controls work only for target platforms .NET MAUI reaches – iOS, Android, MacOS, and Windows.

#### ArcGIS Maps SDK for .NET

Embeds the [ArcGIS Maps SDK for .NET](https://developers.arcgis.com/net/) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/ArcGisApp)

#### Esri ArcGIS Maps SDK for .NET

Embeds the [Esri ArcGIS Maps SDK for .NET](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-EsriMaps.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/ArcGisApp)

#### DevExpress .NET MAUI Controls

Embeds the [DevExpress .NET MAUI Controls](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-DevExpres.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/DevExpressApp)

#### GrapeCity ComponentOne .NET MAUI Controls

Embeds the [GrapeCity ComponentOne .NET MAUI Controls](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-GrapeCity.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/GrapeCityApp)

#### Grial UI Kit

Embeds the [Grial UI Kit](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-GrialKit.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/GrialKitApp)

#### .NET MAUI Community Toolkit  

Embeds the [.NET MAUI Community Toolkit](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/MauiCommunityToolkitApp)

#### DevExpress .NET MAUI Controls

Embeds the [DevExpress .NET MAUI Controls](https://www.devexpress.com/maui/) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/DevExpressApp)

#### GrapeCity ComponentOne .NET MAUI Controls 

Embeds the [GrapeCity ComponentOne .NET MAUI Controls](https://www.grapecity.com/componentone/docs/maui/online-maui/overview.html) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/GrapeCityApp)

#### Grial UI Kit for .NET MAUI

Embeds the [Grial UI Kit for .NET MAUI](https://grialkit.com/) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/GrialKitApp)

#### Syncfusion .NET MAUI Controls

Embeds the [Syncfusion .NET MAUI Controls](https://www.syncfusion.com/maui-controls) in an Uno Platform application.
#### Syncfusion .NET MAUI Controls

Embeds the [Syncfusion .NET MAUI Controls](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-Synfusion.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/SyncfusionApp)

#### Telerik UI for .NET MAUI

Embeds the [Telerik UI for .NET MAUI](https://www.telerik.com/maui-ui) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/TelerikApp)
Embeds the [Telerik UI for .NET MAUI](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-Telerik.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/TelerikApp)

### Migrating Xamarin.Forms Animations
Code to accompany the [blog post](https://platform.uno/blog/migrating-animations-from-xamarin-forms-to-uno-platform/) on migrating animations from Xamarin Forms to Uno.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MigratingAnimations)

### Migrating Xamarin.Forms Effects
Code samples to accompany the [blog post](https://platform.uno/blog/xamarin-forms-migration-to-uno-platform-effects-and-alternative-approaches/) on Migrating from Xamarin.Forms Effects
- XamarinFormsEffect: A reference implementation of an effect with an Android implementation.
  [Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MigratingEffects/XamarinFormsEffect)
- UnoEffectSample: Showcasing how to replace Effects with either ControlTemplates or custom code accessing the native control.
  [Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MigratingEffects/UnoEffectsSample)
  

### Native Frame Navigation
An example showcasing how to set up the native frame navigation for iOS and Android, and frame navigation in general for Uno.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/NativeFrameNav)

[Follow the tutorial](https://platform.uno/docs/articles/guides/native-frame-nav-tutorial.html)

### Native Style Switch  
An example of a toggle that allows you to switch between Native UI Controls and UWP UI Controls. The sample includes a checkbox, slider, button, and toggle. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/NativeStylesSwitch)

### Neumorphism  
An example of an app containing an animated lock that you can unlock.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/Neumorphism)

### Package Resources
An example that demonstrates the use of package assets and how to read them from your app. 

Note that for WebAssembly assets are downloaded on demand, as can be seen in the browser's network tab.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/PackageResources)

### Pet Adopt
An example that demonstrates the use of pipspager with a flipview, in an app created with the help of the Figma-to-XAML plugin. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/PetAdoptUI)

### SkiaSharp Test  
An example of the Uno implementation of SkiaSharp creating a basic canvas with text.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SkiaSharpTest)

### SkiaSharp Skottie: Lottie file player

This samples shows the use of the SkiaSharp.Skottie component, which allows the playback of [lottie files](https://airbnb.design/lottie).

This component provides Lottie support for all available Uno Platform targets.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SkottieSample)

### Splash Screen Sample
An example showing how to set the splash/launch screen in Uno.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SplashScreenSample)

[Follow the tutorial](https://platform.uno/docs/articles/splash-screen.html)

### SQLite  
This is a simple standalone app demonstrating the use of SQLite in an Uno application, including WebAssembly. It uses Erik Sink's [SQLitePCLRaw](https://github.com/ericsink/SQLitePCL.raw), and Frank Krueger's [sqlite-net](https://github.com/praeclarum/sqlite-net) libraries.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SQLiteSample)

### StatusBar Theme Color
An example showing how to adjust the `StatusBar` and `CommandBar` dynamically based on the current light/dark theme.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/StatusBarThemeColor)

[Follow the tutorial](https://platform.uno/docs/articles/guides/status-bar-theme-color.html)

### The Cat Api Client
An example demonstrating an approach to consuming REST web services in Uno using HttpClient.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/TheCatApiClient)

[Follow the tutorial](https://platform.uno/docs/articles/howto-consume-webservices.html)

### Time Entry

Code for the Silverlight migration tutorial.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/TimeEntry)

[Follow the tutorial](https://platform.uno/docs/articles/silverlight-migration-landing.html)

### Toy Car
A proof of concept of a car animation using the `TwoPaneView` control spanned across dual screens (such as Neo or Duo dual-screen devices for example).
Inspiration from Justin Liu's [demo app](https://twitter.com/justinxinliu/status/1281123335410049027).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ToyCar)

### Travel UI
A sample travel app that shows how a user could 1) search for locations, 2) favorite locations and 3) view their profile as well as others' profiles. For the layout, showcases many ListViews coupled with Grids.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/TravelUI)

### Uno BackgroundWorker: Background Work

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoBackgroundWorker)

### Uno Cakes Mobile
A port of Shaw Yu's Cakes Mobile App from [XampleUI](https://github.com/shawyunz/XampleUI) to Uno Platform.
Used to demonstrate simple page navigation from View and ViewModel.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoCakesMobile)

### Uno Contoso
A port of Microsoft's Contoso Enterprise UWP app to Uno Platform, using Prism.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoContoso)

### Uno Cupertino Theme

An example showing how to set up the [`Uno.Cupertino`](https://github.com/unoplatform/Uno.Themes) library.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoCupertinoSample)

[Consult the documentation](https://platform.uno/docs/articles/external/uno.themes/doc/cupertino-getting-started.html)

### Uno Ethereum+Blockchain

A sample showing how to integrate smart contracts on the Ethereum blockchain with a multi-targeted Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoEthereumBlockChain)

### Uno GoodReads

A sample showing how to make an app containing several pages in a TabView, as well as fake data generation to populate those pages. The blog post series for this app includes parts on [creating the home page](https://platform.uno/blog/recreating-amazon-goodreads-app-home-page-using-material-ui-figma-and-uno-platform/), [creating the author page](https://platform.uno/blog/how-to-quickly-build-goodreads-author-page-with-figma-and-uno-platform/), [creating the books page](https://platform.uno/blog/replicating-goodreads-detail-page-in-figma-with-uno-platform/), and [code generation with Figma](https://platform.uno/blog/from-figma-to-visual-studio-adding-back-end-logic-to-goodreads-app/).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoGoodReads)

### Uno Islands

This sample shows how you can integrate Uno Platform XAML controls into existing WPF applications using Uno Islands. This feature allows you to enhance WPF apps with Uno Platform features by hosting Uno Platform XAML files in a Shared project and adding an Uno Island using the UnoXamlHost control.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoIslandsSampleApp)

[Follow the tutorial](https://platform.uno/docs/articles/guides/uno-islands.html)

### Uno Material Theme
An example showing how to set up the [`Uno.Material`](https://github.com/unoplatform/Uno.Themes) library.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoMaterialSample)

[Consult the documentation](https://platform.uno/docs/articles/external/uno.themes/doc/material-getting-started.html)

### Uno Onnx

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoOnnxSamples)

### Uno Scroll Reveal

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoScrollReveal)

### Uno SQlite One Drive Invoice

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoSQLiteOneDriveInvoiceSample)

### Uno Toolkit Material
An example showing how to set up the [`Uno.Toolkit.Material`](https://github.com/unoplatform/uno.toolkit.ui/tree/main/src/library/Uno.Toolkit.Material) library, 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoMaterialToolkitSample)

[Consult the documentation](https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/material-getting-started.html)

### WCT DataGrid  
A dynamic grid view ported from the Windows Community Toolkit that allows for x:Bind. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoWCTDataGridSample)

[Follow the tutorial](https://platform.uno/docs/articles/uno-community-toolkit.html)

### WCT DataGrid, TreeView, TabView

A combined Windows Community Toolkit sample showing the DataGrid, TreeView, and TabView controls in action.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/WCTDataTreeTabSample)

### WebRTC

Demo of the usage of WebRTC in Uno WebAssembly. This sample establishes a direct WebRTC connection between 2 browsers and uses it to send messages between peers.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/WebRTC)

### XAML Basics : ListView

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/XamlBasics/ListViewSample)


Visit [our documentation](https://platform.uno/docs/articles/intro.html) for more details.

### XamlBrewer SkiaSharp
Port of the XAML Brewer WinUI3 SkiaSharp Sample application ([blog post](https://xamlbrewer.wordpress.com/2023/09/25/getting-started-with-skiasharp-in-winui-3/) and [source code](https://github.com/XamlBrewer/XamlBrewer.WinUI3.SkiaSharp.Sample))

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/XamlBrewerUnoApp)
uid: Uno.Samples.List
---

## Samples

### Commerce App

The Commerce App is a sample application that demonstrates the use of ListFeed pagination, Feedviews, and other features provided by Uno.Extensions. It illustrates how these features can be applied to create an application complete with a shopping cart, products, and more.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/Commerce)

### SimpleCalc App

The SimpleCalc App is a sample application designed to perform basic arithmetic operations. App was built using the four variants of the [Simple Calc workshop](https://aka.platform.uno/simplecalc-workshop), combining different markup languages (XAML or C# Markup) and presentation frameworks (MVVM or MVUX).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/SimpleCalc)

### Counter App

Experience the simplicity and power of Uno.Extensions through the Counter App, a straightforward yet powerful demonstration of both basic and advanced features of the Uno Platform. This app provides a hands-on experience with fundamental concepts such as state management, user interaction, and real-time UI updates.
This sample app was built using the four variants of the [Counter workshop](https://aka.platform.uno/counter-tutorial), combining markup language (XAML or C# Markup) and presentation framework (MVVM or MVUX).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/Counter)

### ToDo App

Dive into the essentials of task management with the ToDo App, a meticulously crafted sample application that highlights the power and flexibility of Uno.Extensions. By emphasizing the creation and organization of to-do lists, this app showcases practical applications of essential Uno.Extensions features, offering a hands-on experience in crafting responsive and user-friendly interfaces across multiple platforms.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/ToDo)

### TubePlayer App

The TubePlayer App is a sample application that allows users to search for, and stream Youtube videos. This app was created using the tools, libraries, and patterns provided by the Uno Platform, designed to facilitate the rapid development of high-quality applications.
This sample app was built following the [Tube Player workshop](https://aka.platform.uno/tubeplayer-workshop).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/TubePlayer)

### Advanced XBind

The {x:Bind} markup extension�new for Windows 10�is an alternative to {Binding}. {x:Bind} 
runs in less time and less memory than {Binding} and supports better debugging.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/AdvancedXBind)

### Android Custom Camera  
An Android-specific sample that shows how to start a camera capture intent, and display the result in an `Image` control.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/AndroidCustomCamera)

### Authentication with OpenID Connect (OIDC)

This sample application demonstrates the usage  of the `WebAuthenticationBroker` in Uno with an OpenID Connect endpoint.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/Authentication.OidcDemo)

[Follow the tutorial](https://platform.uno/docs/articles/guides/open-id-connect.html)

### Auto-Suggest  
An implementation of the XAML `AutoSuggest` control, showing how to autofill suggestions for user input. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/AutoSuggestSample)

### Benchmark
An implementation of the .NET Benchmark Control, a performance comparison tool.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/Benchmark)

### BluetoothExplorer
A sample that allows the user to search for nearby Bluetooth connections and connect to a device of their choice. Uses [InTheHand.BluetoothLE](https://www.nuget.org/packages/InTheHand.BluetoothLE)

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/BluetoothExplorer) 

### Camera Capture UI  
A cross-platform implementation of the UWP `CameraCaptureUI` class that allows the user to capture audio, video, and photos from the device camera. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CameraCaptureUI)

### CardView Migration
An Uno conversion of the Xamarin `CardView` sample showing how to migrate a custom control. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CardViewMigration)

### ChatGPT

A ChatGPT sample using OpenAI SDK with C# Markup, MVUX and immutable records.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ChatGPT)

### Chat SignalR

Demonstrates the use of [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-3.1) in an Uno Platform application. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ChatSignalR)

### Control Library  
An example of creating a custom control library and calling a control from your shared project.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ControlLibrary)

### Country Data

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CountryDataSample)


### Custom Sorting
Sample app to accompany "Adding Custom Sorting Logic" blog post.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CustomSorting)

### Dual-Screen
A simple example using the `TwoPaneView` control spanned across dual screens (such as Neo or Duo dual-screen devices for example). 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/DualScreenSample)

### EmbeddedResources
An example that demonstrates the use of embedded resources and how to read them from your app.
Note that the [`Default namespace`](https://stackoverflow.com/questions/2871314/change-project-namespace-in-visual-studio) property of all projects is the same in order for the embedded resource names to be the same on all platforms.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/EmbeddedResources)

### Entity Framework Core Demo

An example of Entity Framework Core 7 with a SQLite storage for WebAssembly, WinAppSDK, iOS and Android.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/EFCoreSQLiteSample)

### FileSavePicker iOS

A working implementation of a folder-based save file picker for iOS. See [the 'iOS' section in the Windows.Storage.Pickers Uno documentation](https://platform.uno/docs/articles/features/windows-storage-pickers.html#ios) for more information.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/FileSavePickeriOS)

### Food Delivery

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/FoodDeliveryUI)

### HtmlControls

This is a WASM-only sample. It is creating _native_ HTML elements that can be used directly in XAML.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/HtmlControls)

### LiteDB

This is an example that utilizes the [LiteDB NuGet package](http://www.litedb.org/) to save data.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/LiteDB)

### Inserting Separators

This example demonstrates the dynamic creation of a menu incorporating nested items as well as items with separators. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/InsertingSeparators)

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

### .NET MAUI Embedding  
Multiple samples that demonstrate third-party control libraries embedded in Uno Platform applications using .NET MAUI Embedding.
Note that these controls work only for target platforms .NET MAUI reaches – iOS, Android, MacOS, and Windows.

#### ArcGIS Maps SDK for .NET

Embeds the [ArcGIS Maps SDK for .NET](https://developers.arcgis.com/net/) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/ArcGisApp)

#### Esri ArcGIS Maps SDK for .NET

Embeds the [Esri ArcGIS Maps SDK for .NET](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-EsriMaps.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/ArcGisApp)

#### DevExpress .NET MAUI Controls

Embeds the [DevExpress .NET MAUI Controls](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-DevExpres.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/DevExpressApp)

#### GrapeCity ComponentOne .NET MAUI Controls

Embeds the [GrapeCity ComponentOne .NET MAUI Controls](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-GrapeCity.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/GrapeCityApp)

#### Grial UI Kit

Embeds the [Grial UI Kit](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-GrialKit.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/GrialKitApp)

#### .NET MAUI Community Toolkit  

Embeds the [.NET MAUI Community Toolkit](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/MauiCommunityToolkitApp)

#### DevExpress .NET MAUI Controls

Embeds the [DevExpress .NET MAUI Controls](https://www.devexpress.com/maui/) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/DevExpressApp)

#### GrapeCity ComponentOne .NET MAUI Controls 

Embeds the [GrapeCity ComponentOne .NET MAUI Controls](https://www.grapecity.com/componentone/docs/maui/online-maui/overview.html) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/GrapeCityApp)

#### Grial UI Kit for .NET MAUI

Embeds the [Grial UI Kit for .NET MAUI](https://grialkit.com/) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/GrialKitApp)

#### Syncfusion .NET MAUI Controls

Embeds the [Syncfusion .NET MAUI Controls](https://www.syncfusion.com/maui-controls) in an Uno Platform application.
#### Syncfusion .NET MAUI Controls

Embeds the [Syncfusion .NET MAUI Controls](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-Synfusion.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/SyncfusionApp)

#### Telerik UI for .NET MAUI

Embeds the [Telerik UI for .NET MAUI](https://www.telerik.com/maui-ui) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/TelerikApp)
Embeds the [Telerik UI for .NET MAUI](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-Telerik.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/TelerikApp)

### Migrating Xamarin.Forms Animations
Code to accompany the [blog post](https://platform.uno/blog/migrating-animations-from-xamarin-forms-to-uno-platform/) on migrating animations from Xamarin Forms to Uno.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MigratingAnimations)

### Migrating Xamarin.Forms Effects
Code samples to accompany the [blog post](https://platform.uno/blog/xamarin-forms-migration-to-uno-platform-effects-and-alternative-approaches/) on Migrating from Xamarin.Forms Effects
- XamarinFormsEffect: A reference implementation of an effect with an Android implementation.
  [Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MigratingEffects/XamarinFormsEffect)
- UnoEffectSample: Showcasing how to replace Effects with either ControlTemplates or custom code accessing the native control.
  [Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MigratingEffects/UnoEffectsSample)
  

### Native Frame Navigation
An example showcasing how to set up the native frame navigation for iOS and Android, and frame navigation in general for Uno.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/NativeFrameNav)

[Follow the tutorial](https://platform.uno/docs/articles/guides/native-frame-nav-tutorial.html)

### Native Style Switch  
An example of a toggle that allows you to switch between Native UI Controls and UWP UI Controls. The sample includes a checkbox, slider, button, and toggle. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/NativeStylesSwitch)

### Neumorphism  
An example of an app containing an animated lock that you can unlock.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/Neumorphism)

### Package Resources
An example that demonstrates the use of package assets and how to read them from your app. 

Note that for WebAssembly assets are downloaded on demand, as can be seen in the browser's network tab.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/PackageResources)

### Pet Adopt
An example that demonstrates the use of pipspager with a flipview, in an app created with the help of the Figma-to-XAML plugin. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/PetAdoptUI)

### SkiaSharp Test  
An example of the Uno implementation of SkiaSharp creating a basic canvas with text.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SkiaSharpTest)

### SkiaSharp Skottie: Lottie file player

This samples shows the use of the SkiaSharp.Skottie component, which allows the playback of [lottie files](https://airbnb.design/lottie).

This component provides Lottie support for all available Uno Platform targets.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SkottieSample)

### Splash Screen Sample
An example showing how to set the splash/launch screen in Uno.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SplashScreenSample)

[Follow the tutorial](https://platform.uno/docs/articles/splash-screen.html)

### SQLite  
This is a simple standalone app demonstrating the use of SQLite in an Uno application, including WebAssembly. It uses Erik Sink's [SQLitePCLRaw](https://github.com/ericsink/SQLitePCL.raw), and Frank Krueger's [sqlite-net](https://github.com/praeclarum/sqlite-net) libraries.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SQLiteSample)

### StatusBar Theme Color
An example showing how to adjust the `StatusBar` and `CommandBar` dynamically based on the current light/dark theme.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/StatusBarThemeColor)

[Follow the tutorial](https://platform.uno/docs/articles/guides/status-bar-theme-color.html)

### The Cat Api Client
An example demonstrating an approach to consuming REST web services in Uno using HttpClient.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/TheCatApiClient)

[Follow the tutorial](https://platform.uno/docs/articles/howto-consume-webservices.html)

### Time Entry

Code for the Silverlight migration tutorial.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/TimeEntry)

[Follow the tutorial](https://platform.uno/docs/articles/silverlight-migration-landing.html)

### Toy Car
A proof of concept of a car animation using the `TwoPaneView` control spanned across dual screens (such as Neo or Duo dual-screen devices for example).
Inspiration from Justin Liu's [demo app](https://twitter.com/justinxinliu/status/1281123335410049027).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ToyCar)

### Travel UI
A sample travel app that shows how a user could 1) search for locations, 2) favorite locations and 3) view their profile as well as others' profiles. For the layout, showcases many ListViews coupled with Grids.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/TravelUI)

### Uno BackgroundWorker: Background Work

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoBackgroundWorker)

### Uno Cakes Mobile
A port of Shaw Yu's Cakes Mobile App from [XampleUI](https://github.com/shawyunz/XampleUI) to Uno Platform.
Used to demonstrate simple page navigation from View and ViewModel.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoCakesMobile)

### Uno Contoso
A port of Microsoft's Contoso Enterprise UWP app to Uno Platform, using Prism.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoContoso)

### Uno Cupertino Theme

An example showing how to set up the [`Uno.Cupertino`](https://github.com/unoplatform/Uno.Themes) library.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoCupertinoSample)

[Consult the documentation](https://platform.uno/docs/articles/external/uno.themes/doc/cupertino-getting-started.html)

### Uno Ethereum+Blockchain

A sample showing how to integrate smart contracts on the Ethereum blockchain with a multi-targeted Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoEthereumBlockChain)

### Uno GoodReads

A sample showing how to make an app containing several pages in a TabView, as well as fake data generation to populate those pages. The blog post series for this app includes parts on [creating the home page](https://platform.uno/blog/recreating-amazon-goodreads-app-home-page-using-material-ui-figma-and-uno-platform/), [creating the author page](https://platform.uno/blog/how-to-quickly-build-goodreads-author-page-with-figma-and-uno-platform/), [creating the books page](https://platform.uno/blog/replicating-goodreads-detail-page-in-figma-with-uno-platform/), and [code generation with Figma](https://platform.uno/blog/from-figma-to-visual-studio-adding-back-end-logic-to-goodreads-app/).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoGoodReads)

### Uno Islands

This sample shows how you can integrate Uno Platform XAML controls into existing WPF applications using Uno Islands. This feature allows you to enhance WPF apps with Uno Platform features by hosting Uno Platform XAML files in a Shared project and adding an Uno Island using the UnoXamlHost control.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoIslandsSampleApp)

[Follow the tutorial](https://platform.uno/docs/articles/guides/uno-islands.html)

### Uno Material Theme
An example showing how to set up the [`Uno.Material`](https://github.com/unoplatform/Uno.Themes) library.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoMaterialSample)

[Consult the documentation](https://platform.uno/docs/articles/external/uno.themes/doc/material-getting-started.html)

### Uno Onnx

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoOnnxSamples)

### Uno Scroll Reveal

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoScrollReveal)

### Uno SQlite One Drive Invoice

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoSQLiteOneDriveInvoiceSample)

### Uno Toolkit Material
An example showing how to set up the [`Uno.Toolkit.Material`](https://github.com/unoplatform/uno.toolkit.ui/tree/main/src/library/Uno.Toolkit.Material) library, 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoMaterialToolkitSample)

[Consult the documentation](https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/material-getting-started.html)

### WCT DataGrid  
A dynamic grid view ported from the Windows Community Toolkit that allows for x:Bind. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoWCTDataGridSample)

[Follow the tutorial](https://platform.uno/docs/articles/uno-community-toolkit.html)

### WCT DataGrid, TreeView, TabView

A combined Windows Community Toolkit sample showing the DataGrid, TreeView, and TabView controls in action.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/WCTDataTreeTabSample)

### WebRTC

Demo of the usage of WebRTC in Uno WebAssembly. This sample establishes a direct WebRTC connection between 2 browsers and uses it to send messages between peers.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/WebRTC)

### XAML Basics : ListView

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/XamlBasics/ListViewSample)


Visit [our documentation](https://platform.uno/docs/articles/intro.html) for more details.

### XamlBrewer SkiaSharp
Port of the XAML Brewer WinUI3 SkiaSharp Sample application ([blog post](https://xamlbrewer.wordpress.com/2023/09/25/getting-started-with-skiasharp-in-winui-3/) and [source code](https://github.com/XamlBrewer/XamlBrewer.WinUI3.SkiaSharp.Sample))

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/XamlBrewerUnoApp)
uid: Uno.Samples.List
---

## Samples

### Commerce App

The Commerce App is a sample application that demonstrates the use of ListFeed pagination, Feedviews, and other features provided by Uno.Extensions. It illustrates how these features can be applied to create an application complete with a shopping cart, products, and more.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/Commerce)

### SimpleCalc App

The SimpleCalc App is a sample application designed to perform basic arithmetic operations. App was built using the four variants of the [Simple Calc workshop](https://aka.platform.uno/simplecalc-workshop), combining different markup languages (XAML or C# Markup) and presentation frameworks (MVVM or MVUX).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/SimpleCalc)

### Counter App

Experience the simplicity and power of Uno.Extensions through the Counter App, a straightforward yet powerful demonstration of both basic and advanced features of the Uno Platform. This app provides a hands-on experience with fundamental concepts such as state management, user interaction, and real-time UI updates.
This sample app was built using the four variants of the [Counter workshop](https://aka.platform.uno/counter-tutorial), combining markup language (XAML or C# Markup) and presentation framework (MVVM or MVUX).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/Counter)

### ToDo App

Dive into the essentials of task management with the ToDo App, a meticulously crafted sample application that highlights the power and flexibility of Uno.Extensions. By emphasizing the creation and organization of to-do lists, this app showcases practical applications of essential Uno.Extensions features, offering a hands-on experience in crafting responsive and user-friendly interfaces across multiple platforms.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/ToDo)

### TubePlayer App

The TubePlayer App is a sample application that allows users to search for, and stream Youtube videos. This app was created using the tools, libraries, and patterns provided by the Uno Platform, designed to facilitate the rapid development of high-quality applications.
This sample app was built following the [Tube Player workshop](https://aka.platform.uno/tubeplayer-workshop).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/reference/TubePlayer)

### Advanced XBind

The {x:Bind} markup extension�new for Windows 10�is an alternative to {Binding}. {x:Bind} 
runs in less time and less memory than {Binding} and supports better debugging.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/AdvancedXBind)

### Android Custom Camera  
An Android-specific sample that shows how to start a camera capture intent, and display the result in an `Image` control.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/AndroidCustomCamera)

### Authentication with OpenID Connect (OIDC)

This sample application demonstrates the usage  of the `WebAuthenticationBroker` in Uno with an OpenID Connect endpoint.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/Authentication.OidcDemo)

[Follow the tutorial](https://platform.uno/docs/articles/guides/open-id-connect.html)

### Auto-Suggest  
An implementation of the XAML `AutoSuggest` control, showing how to autofill suggestions for user input. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/AutoSuggestSample)

### Benchmark
An implementation of the .NET Benchmark Control, a performance comparison tool.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/Benchmark)

### BluetoothExplorer
A sample that allows the user to search for nearby Bluetooth connections and connect to a device of their choice. Uses [InTheHand.BluetoothLE](https://www.nuget.org/packages/InTheHand.BluetoothLE)

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/BluetoothExplorer) 

### Camera Capture UI  
A cross-platform implementation of the UWP `CameraCaptureUI` class that allows the user to capture audio, video, and photos from the device camera. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CameraCaptureUI)

### CardView Migration
An Uno conversion of the Xamarin `CardView` sample showing how to migrate a custom control. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CardViewMigration)

### ChatGPT

A ChatGPT sample using OpenAI SDK with C# Markup, MVUX and immutable records.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ChatGPT)

### Chat SignalR

Demonstrates the use of [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-3.1) in an Uno Platform application. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ChatSignalR)

### Control Library  
An example of creating a custom control library and calling a control from your shared project.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ControlLibrary)

### Country Data

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CountryDataSample)


### Custom Sorting
Sample app to accompany "Adding Custom Sorting Logic" blog post.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/CustomSorting)

### Dual-Screen
A simple example using the `TwoPaneView` control spanned across dual screens (such as Neo or Duo dual-screen devices for example). 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/DualScreenSample)

### EmbeddedResources
An example that demonstrates the use of embedded resources and how to read them from your app.
Note that the [`Default namespace`](https://stackoverflow.com/questions/2871314/change-project-namespace-in-visual-studio) property of all projects is the same in order for the embedded resource names to be the same on all platforms.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/EmbeddedResources)

### Entity Framework Core Demo

An example of Entity Framework Core 7 with a SQLite storage for WebAssembly, WinAppSDK, iOS and Android.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/EFCoreSQLiteSample)

### FileSavePicker iOS

A working implementation of a folder-based save file picker for iOS. See [the 'iOS' section in the Windows.Storage.Pickers Uno documentation](https://platform.uno/docs/articles/features/windows-storage-pickers.html#ios) for more information.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/FileSavePickeriOS)

### Food Delivery

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/FoodDeliveryUI)

### HtmlControls

This is a WASM-only sample. It is creating _native_ HTML elements that can be used directly in XAML.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/HtmlControls)

### LiteDB

This is an example that utilizes the [LiteDB NuGet package](http://www.litedb.org/) to save data.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/LiteDB)

### Inserting Separators

This example demonstrates the dynamic creation of a menu incorporating nested items as well as items with separators. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/InsertingSeparators)

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

### .NET MAUI Embedding  
Multiple samples that demonstrate third-party control libraries embedded in Uno Platform applications using .NET MAUI Embedding.
Note that these controls work only for target platforms .NET MAUI reaches – iOS, Android, MacOS, and Windows.

#### ArcGIS Maps SDK for .NET

Embeds the [ArcGIS Maps SDK for .NET](https://developers.arcgis.com/net/) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/ArcGisApp)

#### Esri ArcGIS Maps SDK for .NET

Embeds the [Esri ArcGIS Maps SDK for .NET](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-EsriMaps.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/ArcGisApp)

#### DevExpress .NET MAUI Controls

Embeds the [DevExpress .NET MAUI Controls](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-DevExpres.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/DevExpressApp)

#### GrapeCity ComponentOne .NET MAUI Controls

Embeds the [GrapeCity ComponentOne .NET MAUI Controls](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-GrapeCity.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/GrapeCityApp)

#### Grial UI Kit

Embeds the [Grial UI Kit](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-GrialKit.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/GrialKitApp)

#### .NET MAUI Community Toolkit  

Embeds the [.NET MAUI Community Toolkit](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/MauiCommunityToolkitApp)

#### DevExpress .NET MAUI Controls

Embeds the [DevExpress .NET MAUI Controls](https://www.devexpress.com/maui/) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/DevExpressApp)

#### GrapeCity ComponentOne .NET MAUI Controls 

Embeds the [GrapeCity ComponentOne .NET MAUI Controls](https://www.grapecity.com/componentone/docs/maui/online-maui/overview.html) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/GrapeCityApp)

#### Grial UI Kit for .NET MAUI

Embeds the [Grial UI Kit for .NET MAUI](https://grialkit.com/) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/GrialKitApp)

#### Syncfusion .NET MAUI Controls

Embeds the [Syncfusion .NET MAUI Controls](https://www.syncfusion.com/maui-controls) in an Uno Platform application.
#### Syncfusion .NET MAUI Controls

Embeds the [Syncfusion .NET MAUI Controls](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-Synfusion.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/SyncfusionApp)

#### Telerik UI for .NET MAUI

Embeds the [Telerik UI for .NET MAUI](https://www.telerik.com/maui-ui) in an Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/TelerikApp)
Embeds the [Telerik UI for .NET MAUI](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Maui/ThirdParty-Telerik.html) in an Uno application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MauiEmbedding/TelerikApp)

### Migrating Xamarin.Forms Animations
Code to accompany the [blog post](https://platform.uno/blog/migrating-animations-from-xamarin-forms-to-uno-platform/) on migrating animations from Xamarin Forms to Uno.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MigratingAnimations)

### Migrating Xamarin.Forms Effects
Code samples to accompany the [blog post](https://platform.uno/blog/xamarin-forms-migration-to-uno-platform-effects-and-alternative-approaches/) on Migrating from Xamarin.Forms Effects
- XamarinFormsEffect: A reference implementation of an effect with an Android implementation.
  [Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MigratingEffects/XamarinFormsEffect)
- UnoEffectSample: Showcasing how to replace Effects with either ControlTemplates or custom code accessing the native control.
  [Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/MigratingEffects/UnoEffectsSample)
  

### Native Frame Navigation
An example showcasing how to set up the native frame navigation for iOS and Android, and frame navigation in general for Uno.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/NativeFrameNav)

[Follow the tutorial](https://platform.uno/docs/articles/guides/native-frame-nav-tutorial.html)

### Native Style Switch  
An example of a toggle that allows you to switch between Native UI Controls and UWP UI Controls. The sample includes a checkbox, slider, button, and toggle. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/NativeStylesSwitch)

### Neumorphism  
An example of an app containing an animated lock that you can unlock.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/Neumorphism)

### Package Resources
An example that demonstrates the use of package assets and how to read them from your app. 

Note that for WebAssembly assets are downloaded on demand, as can be seen in the browser's network tab.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/PackageResources)

### Pet Adopt
An example that demonstrates the use of pipspager with a flipview, in an app created with the help of the Figma-to-XAML plugin. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/PetAdoptUI)

### SkiaSharp Test  
An example of the Uno implementation of SkiaSharp creating a basic canvas with text.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SkiaSharpTest)

### SkiaSharp Skottie: Lottie file player

This samples shows the use of the SkiaSharp.Skottie component, which allows the playback of [lottie files](https://airbnb.design/lottie).

This component provides Lottie support for all available Uno Platform targets.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SkottieSample)

### Splash Screen Sample
An example showing how to set the splash/launch screen in Uno.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SplashScreenSample)

[Follow the tutorial](https://platform.uno/docs/articles/splash-screen.html)

### SQLite  
This is a simple standalone app demonstrating the use of SQLite in an Uno application, including WebAssembly. It uses Erik Sink's [SQLitePCLRaw](https://github.com/ericsink/SQLitePCL.raw), and Frank Krueger's [sqlite-net](https://github.com/praeclarum/sqlite-net) libraries.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/SQLiteSample)

### StatusBar Theme Color
An example showing how to adjust the `StatusBar` and `CommandBar` dynamically based on the current light/dark theme.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/StatusBarThemeColor)

[Follow the tutorial](https://platform.uno/docs/articles/guides/status-bar-theme-color.html)

### The Cat Api Client
An example demonstrating an approach to consuming REST web services in Uno using HttpClient.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/TheCatApiClient)

[Follow the tutorial](https://platform.uno/docs/articles/howto-consume-webservices.html)

### Time Entry

Code for the Silverlight migration tutorial.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/TimeEntry)

[Follow the tutorial](https://platform.uno/docs/articles/silverlight-migration-landing.html)

### Toy Car
A proof of concept of a car animation using the `TwoPaneView` control spanned across dual screens (such as Neo or Duo dual-screen devices for example).
Inspiration from Justin Liu's [demo app](https://twitter.com/justinxinliu/status/1281123335410049027).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/ToyCar)

### Travel UI
A sample travel app that shows how a user could 1) search for locations, 2) favorite locations and 3) view their profile as well as others' profiles. For the layout, showcases many ListViews coupled with Grids.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/TravelUI)

### Uno BackgroundWorker: Background Work

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoBackgroundWorker)

### Uno Cakes Mobile
A port of Shaw Yu's Cakes Mobile App from [XampleUI](https://github.com/shawyunz/XampleUI) to Uno Platform.
Used to demonstrate simple page navigation from View and ViewModel.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoCakesMobile)

### Uno Contoso
A port of Microsoft's Contoso Enterprise UWP app to Uno Platform, using Prism.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoContoso)

### Uno Cupertino Theme

An example showing how to set up the [`Uno.Cupertino`](https://github.com/unoplatform/Uno.Themes) library.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoCupertinoSample)

[Consult the documentation](https://platform.uno/docs/articles/external/uno.themes/doc/cupertino-getting-started.html)

### Uno Ethereum+Blockchain

A sample showing how to integrate smart contracts on the Ethereum blockchain with a multi-targeted Uno Platform application.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoEthereumBlockChain)

### Uno GoodReads

A sample showing how to make an app containing several pages in a TabView, as well as fake data generation to populate those pages. The blog post series for this app includes parts on [creating the home page](https://platform.uno/blog/recreating-amazon-goodreads-app-home-page-using-material-ui-figma-and-uno-platform/), [creating the author page](https://platform.uno/blog/how-to-quickly-build-goodreads-author-page-with-figma-and-uno-platform/), [creating the books page](https://platform.uno/blog/replicating-goodreads-detail-page-in-figma-with-uno-platform/), and [code generation with Figma](https://platform.uno/blog/from-figma-to-visual-studio-adding-back-end-logic-to-goodreads-app/).

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoGoodReads)

### Uno Islands

This sample shows how you can integrate Uno Platform XAML controls into existing WPF applications using Uno Islands. This feature allows you to enhance WPF apps with Uno Platform features by hosting Uno Platform XAML files in a Shared project and adding an Uno Island using the UnoXamlHost control.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoIslandsSampleApp)

[Follow the tutorial](https://platform.uno/docs/articles/guides/uno-islands.html)

### Uno Material Theme
An example showing how to set up the [`Uno.Material`](https://github.com/unoplatform/Uno.Themes) library.  

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoMaterialSample)

[Consult the documentation](https://platform.uno/docs/articles/external/uno.themes/doc/material-getting-started.html)

### Uno Onnx

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoOnnxSamples)

### Uno Scroll Reveal

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoScrollReveal)

### Uno SQlite One Drive Invoice

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoSQLiteOneDriveInvoiceSample)

### Uno Toolkit Material
An example showing how to set up the [`Uno.Toolkit.Material`](https://github.com/unoplatform/uno.toolkit.ui/tree/main/src/library/Uno.Toolkit.Material) library, 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoMaterialToolkitSample)

[Consult the documentation](https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/material-getting-started.html)

### WCT DataGrid  
A dynamic grid view ported from the Windows Community Toolkit that allows for x:Bind. 

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoWCTDataGridSample)

[Follow the tutorial](https://platform.uno/docs/articles/uno-community-toolkit.html)

### WCT DataGrid, TreeView, TabView

A combined Windows Community Toolkit sample showing the DataGrid, TreeView, and TabView controls in action.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/WCTDataTreeTabSample)

### WebRTC

Demo of the usage of WebRTC in Uno WebAssembly. This sample establishes a direct WebRTC connection between 2 browsers and uses it to send messages between peers.

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/WebRTC)

### XAML Basics : ListView

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/XamlBasics/ListViewSample)


Visit [our documentation](https://platform.uno/docs/articles/intro.html) for more details.

### XamlBrewer SkiaSharp
Port of the XAML Brewer WinUI3 SkiaSharp Sample application ([blog post](https://xamlbrewer.wordpress.com/2023/09/25/getting-started-with-skiasharp-in-winui-3/) and [source code](https://github.com/XamlBrewer/XamlBrewer.WinUI3.SkiaSharp.Sample))

[Browse source](https://github.com/unoplatform/Uno.Samples/tree/master/UI/XamlBrewerUnoApp)

## Uno Platform Samples - Issues
If you encounter any issues with these samples above, please open an issue [here](https://github.com/unoplatform/uno/issues).

# Contributors
Thanks go to these wonderful people (List made with [contrib.rocks](https://contrib.rocks)):

[![Uno.Samples Contributors](https://contrib.rocks/image?repo=unoplatform/Uno.Samples)](https://github.com/unoplatform/Uno.Samples/graphs/contributors)

💖 Thank you.

