using System.Collections.Concurrent;
using System.Reflection;

namespace QuoteCraft.Helpers;

/// <summary>
/// Helper for accessing MVUX model instances from code-behind.
/// MVUX generates ViewModel wrapper classes (e.g., DashboardViewModel for DashboardModel)
/// set as the page DataContext. The underlying model is accessible via the Model property.
/// </summary>
internal static class MvuxHelper
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo?> _cache = new();

    internal static T? GetModel<T>(object? dataContext) where T : class
    {
        if (dataContext is null) return null;
        if (dataContext is T model) return model;

        var prop = _cache.GetOrAdd(dataContext.GetType(),
            type => type.GetProperty("Model", BindingFlags.Public | BindingFlags.Instance));
        return prop?.GetValue(dataContext) as T;
    }
}
