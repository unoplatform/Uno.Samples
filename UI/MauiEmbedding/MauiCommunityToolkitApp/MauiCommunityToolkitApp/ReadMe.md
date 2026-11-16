# Getting Started

Welcome to the Uno Platform!

To discover how to get started with your new app: https://aka.platform.uno/get-started

For more information on how to use the Uno.Sdk or upgrade Uno Platform packages in your solution: https://aka.platform.uno/using-uno-sdk

## .NET MAUI embedding

This app shows controls of the .NET MAUI Community Toolkit in an Uno Platform app:

- **Desktop and WebAssembly:** the `MauiEmbedding` UnoFeature ([Uno.Maui.Embedding](https://www.nuget.org/packages/Uno.Maui.Embedding), experimental preview).
- **Android, iOS and Windows:** the `MauiNativeEmbedding` UnoFeature (the native embedding of Uno.Extensions).

In the browser, the Gravatar image of the Avatar page stays empty: `GravatarImageSource` hashes the email with MD5, which .NET does not support in the browser.

More information: https://aka.platform.uno/feature-maui-embedding
