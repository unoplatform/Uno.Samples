using System.Collections.Concurrent;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace QuoteCraft.Helpers;

public class StatusBackgroundConverter : IValueConverter
{
    private static readonly ConcurrentDictionary<string, SolidColorBrush> _cache = new();
    private static readonly SolidColorBrush _fallback = new(Windows.UI.Color.FromArgb(255, 243, 244, 246));

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var status = value?.ToString() ?? string.Empty;
        return _cache.GetOrAdd(status, key =>
        {
            var resKey = $"Status{key}BackgroundBrush";
            if (Application.Current.Resources.TryGetValue(resKey, out var resource) && resource is SolidColorBrush brush)
                return brush;
            if (Application.Current.Resources.TryGetValue("StatusDraftBackgroundBrush", out var fallback) && fallback is SolidColorBrush fb)
                return fb;
            return _fallback;
        });
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

public class StatusForegroundConverter : IValueConverter
{
    private static readonly ConcurrentDictionary<string, SolidColorBrush> _cache = new();
    private static readonly SolidColorBrush _fallback = new(Windows.UI.Color.FromArgb(255, 107, 114, 128));

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var status = value?.ToString() ?? string.Empty;
        return _cache.GetOrAdd(status, key =>
        {
            var resKey = $"Status{key}ForegroundBrush";
            if (Application.Current.Resources.TryGetValue(resKey, out var resource) && resource is SolidColorBrush brush)
                return brush;
            if (Application.Current.Resources.TryGetValue("StatusDraftForegroundBrush", out var fallback) && fallback is SolidColorBrush fb)
                return fb;
            return _fallback;
        });
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
