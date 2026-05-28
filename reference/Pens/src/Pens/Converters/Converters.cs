using Pens.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Pens.Converters;

/// <summary>
/// Helper class to retrieve brushes from Application resources.
/// </summary>
internal static class ResourceHelper
{
    public static Brush GetBrush(string key, Brush fallback)
    {
        if (Application.Current.Resources.TryGetValue(key, out var resource) && resource is Brush brush)
        {
            return brush;
        }
        return fallback;
    }

    public static Color GetColor(string key, Color fallback)
    {
        if (Application.Current.Resources.TryGetValue(key, out var resource) && resource is Color color)
        {
            return color;
        }
        return fallback;
    }
}

public class ToUpperConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, string language)
    {
        return value?.ToString()?.ToUpperInvariant();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class StatusToTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, string language)
    {
        return value switch
        {
            PlayerStatus.In => "IN",
            PlayerStatus.Out => "OUT",
            PlayerStatus.Pending => "?",
            _ => ""
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class StatusToBackgroundConverter : IValueConverter
{
    private static readonly SolidColorBrush TransparentBrush = new(Colors.Transparent);

    public object? Convert(object? value, Type targetType, object? parameter, string language)
    {
        return value switch
        {
            PlayerStatus.In => ResourceHelper.GetBrush("StatusInBackgroundBrush", TransparentBrush),
            PlayerStatus.Out => ResourceHelper.GetBrush("StatusOutBackgroundBrush", TransparentBrush),
            PlayerStatus.Pending => ResourceHelper.GetBrush("StatusPendingBackgroundBrush", TransparentBrush),
            _ => TransparentBrush
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class StatusToForegroundConverter : IValueConverter
{
    private static readonly SolidColorBrush WhiteBrush = new(Colors.White);

    public object? Convert(object? value, Type targetType, object? parameter, string language)
    {
        return value switch
        {
            PlayerStatus.In => ResourceHelper.GetBrush("SuccessGreenBrush", WhiteBrush),
            PlayerStatus.Out => ResourceHelper.GetBrush("HotRedBrush", WhiteBrush),
            PlayerStatus.Pending => ResourceHelper.GetBrush("NeonAmberBrush", WhiteBrush),
            _ => WhiteBrush
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class DutyTypeToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, string language)
    {
        return value switch
        {
            DutyType.Ice => ResourceHelper.GetColor("IceBlueColor", Colors.White),
            DutyType.Beer => ResourceHelper.GetColor("NeonAmberColor", Colors.White),
            DutyType.Cooler => ResourceHelper.GetColor("PurpleAccentColor", Colors.White),
            DutyType.Food => ResourceHelper.GetColor("SuccessGreenColor", Colors.White),
            _ => Colors.White
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class DutyTypeToIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, string language)
    {
        return value switch
        {
            DutyType.Ice => "\uE9C4",      // Snowflake icon
            DutyType.Beer => "\uE799",      // Coffee cup/drink icon
            DutyType.Cooler => "\uE74C",    // Shop/box icon
            DutyType.Food => "\uE7E6",      // Emoji/food icon
            _ => ""
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class BoolToConsumedBackgroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, string language)
    {
        var fallback = ResourceHelper.GetBrush("FallbackDarkBrush", new SolidColorBrush(Colors.DarkGray));
        if (value is bool isConsumed && isConsumed)
        {
            return ResourceHelper.GetBrush("NeonAmberBrush", fallback);
        }
        return ResourceHelper.GetBrush("BoardsMidBrush", fallback);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class BoolToConsumedBorderConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, string language)
    {
        var fallback = ResourceHelper.GetBrush("FallbackSubtleBorderBrush", new SolidColorBrush(Colors.Gray));
        if (value is bool isConsumed && isConsumed)
        {
            return ResourceHelper.GetBrush("NeonAmberSemiBorderBrush", fallback);
        }
        return ResourceHelper.GetBrush("SubtleWhiteBorderBrush", fallback);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class BoolToPowderBlueBorderConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, string language)
    {
        var fallback = ResourceHelper.GetBrush("FallbackSubtleBorderBrush", new SolidColorBrush(Colors.Gray));
        if (value is bool isPenguins && isPenguins)
        {
            return ResourceHelper.GetBrush("PowderBlueSemiBorderBrush", fallback);
        }
        return ResourceHelper.GetBrush("SubtleWhiteBorderBrush", fallback);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class GameNightEmojiConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, string language)
    {
        if (value is bool isGameToday && isGameToday)
        {
            return "\U0001F6A8"; // 🚨 Red rotating light
        }
        return "\U000026A1"; // ⚡ Lightning bolt
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class InverseBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, string language)
    {
        if (value is bool b)
        {
            return !b;
        }
        return true;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, string language)
        => value is true ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
        => value is Visibility.Visible;
}

public class InverseBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, string language)
        => value is true ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
        => value is Visibility.Collapsed;
}

// M2: TabBarItem.IsSelected -> accent (selected) vs muted (unselected) brush.
public class SelectedToBrushConverter : IValueConverter
{
    private static readonly SolidColorBrush Fallback = new(Colors.Gray);

    public object? Convert(object? value, Type targetType, object? parameter, string language)
        => value is true
            ? ResourceHelper.GetBrush("PowderBlueBrush", Fallback)
            : ResourceHelper.GetBrush("TextMutedBrush", Fallback);

    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
        => throw new NotImplementedException();
}

// C2: non-color cue — show a checkmark glyph only on consumed beer cases.
public class ConsumedToGlyphVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, string language)
        => value is true ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
        => throw new NotImplementedException();
}
