---
uid: Uno.Workshops.ToDo-App.SerializeJsonForOptions
---
# Serialize Records loaded by Options

## Immutability with IOptions

<!-- TODO: Add Guide explaining how this is done in ToDo App correctly with examples - DevTKSS assigning -->

### Not like this

This full immutable Record definition does not work in case we want to use it to be serialized from `appsettings.json` and load it via Options, even with all parameters marked as Nullable like this:

```csharp
internal record Auth(string? ApplicationId, string[]? Scopes, string? RedirectUri, string? KeychainSecurityGroup);
```

## Possible Errors

It would mark this part in the App HostBuilder:

[!code-csharp[](../src/ToDo/App.xaml.cs#L59-L70?highlight=65)]

With one of this Error Messages:

```plaintext
IOptions<T> requires the type T to have a parameterless constructor so that it can be instantiated and populated with values from the configuration source. A fully immutable record does not have a parameterless constructor because all properties must be initialized in the constructor.
```

```plaintext
IOptions<T> uses setters to populate the properties with values. In a fully immutable record, the properties are read-only (get-only), meaning they cannot be modified after initialization.
```

Which would both result in an `System.InvalidOperationException` not allowing us to even build our Application.

## Related Pages

* [How To: Uno.Extensions.Serialization](https://aka.platform.uno/docs/articles/external/uno.extensions/doc/Learn/Serialization/HowTo-Serialization.html)
