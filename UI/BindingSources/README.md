# ItemTemplate Command Samples

This repository contains concise examples demonstrating how to execute commands with parameters within an ItemTemplate using both XAML and C# Markup. These samples aim to address a common request and illustrate a typical scenario encountered in Uno Platform development.

## Purpose

The purpose of these samples is to provide developers with a clear understanding of how to implement command execution with parameters within an ItemTemplate. By showcasing both XAML and C# Markup approaches, developers can choose the method that best suits their preferences or project requirements.

## Problem

When working with DataTemplates, accessing data or commands from the parent context can be challenging. DataTemplates operate within their own scope, making it difficult to bind properties or trigger actions in the parent view model. This separation can lead to confusion, especially when dealing with nested templates and multiple levels of data context. In these samples, we provide simple examples to help developers navigate and solve binding difficulties within DataTemplates.

## Related Docs

### XAML

For the XAML Sample we make use of `ItemsControlBinding` which is a markup extensions that provides relative binding based on ancestor type. To know more, access [AncestorBinding & ItemsControlBinding](https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/helpers/ancestor-itemscontrol-binding.html) docs.

### C# Markup

For the C# Markup Sample we take advantage of the `Source` and `Relative Source` Binding methods. To know more, acess [Source and Relative Source](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Markup/SourceUsage.html) docs.