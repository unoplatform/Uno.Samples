---
uid: Uno.Workshops.Counter.Overview
---
# Counter App

Discover the simplicity and power of Uno.Extensions with the Counter App, a straightforward yet powerful demonstration of basic and advanced features of the Uno Platform. This app provides a hands-on experience with fundamental concepts such as state management, user interaction, and UI updates in real-time.
This sample app was built using the four variants of the [Counter workshop](https://aka.platform.uno/counter-tutorial), combining markup language (XAML or C# Markup) and presentation framework (MVVM or MVUX).

![CounterApp Image](doc/assets/counterApp.png)

## Codebase

### MVVM

* [**C#-MVVM MainViewModel.cs**](CSharp-MVVM/Counter/MainViewModel.cs) C#-MVVM model with [**C#-MVVM MainPage.cs**](CSharp-MVVM/Counter/MainPage.cs) C#-MVVM data binding.
* [**XAML-MVVM MainViewModel.cs**](XAML-MVVM/Counter/MainViewModel.cs) XAML-MVVM model with [**XAML-MVVM MainPage.xaml**](XAML-MVVM/Counter/MainPage.xaml) XAML-MVVM data binding.

### MVUX

* [**C#-MVUX MainModel.cs**](CSharp-MVUX/Counter/MainModel.cs) C#-MVUX states and record immutability with [**C#-MVUX MainPage.cs**](CSharp-MVUX/Counter/MainPage.cs) C#-MVUX data binding.
* [**XAML-MVUX MainViewModel.cs**](XAML-MVUX/Counter/MainModel.cs) XAML-MVUX states and record immutability with [**XAML-MVUX MainPage.xaml**](XAML-MVUX/Counter/MainPage.xaml) XAML-MVUX data binding.

## What is the Uno Platform

[Uno Platform](https://platform.uno) is an open-source .NET platform for building single codebase native mobile, web, desktop, and embedded apps quickly.
For additional information about Uno Platform or if you have any feedback to share, please refer to the [README.md](xref:Uno.Samples.Readme) file in this Samples repository.
