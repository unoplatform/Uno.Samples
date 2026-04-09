# ElementLevelTheme

Demonstrates element-level theming in Uno Platform, where individual UI elements can declare their own `RequestedTheme` (Light, Dark, or Default) and all theme resources resolve locally within each subtree.

## Codebase

- **[MainPage.xaml](src/ElementLevelTheme/MainPage.xaml)**: Three-column layout (Light, Dark, Default) with nested theme islands, dynamic theme switching, and a matryoshka nesting showcase demonstrating up to 5 levels of alternating themes.
- **[MainPage.xaml.cs](src/ElementLevelTheme/MainPage.xaml.cs)**: Page-level theme toggling and dynamic runtime theme switching logic.

## What is the Uno Platform

[Uno Platform](https://platform.uno) is an open-source .NET platform for building single codebase native mobile, web, desktop, and embedded apps quickly.
For additional information about Uno Platform please visit our [documentation](https://platform.uno/docs/articles/intro.html).
