using System;
using System.Globalization;
using FieldOpsPro.Models.Enums;
using FieldOpsPro.Presentation.Controls;
using FieldOpsPro.Presentation.Utils;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using TaskStatus = FieldOpsPro.Models.Enums.TaskStatus;

namespace FieldOpsPro.Converters;

/// <summary>Formats a numeric value as a string with an optional suffix from ConverterParameter (e.g. "%", "m").</summary>
public sealed class IntSuffixConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var suffix = parameter as string ?? string.Empty;
        return value switch
        {
            int i => i.ToString("N0", CultureInfo.CurrentCulture) + suffix,
            double d => d.ToString("N0", CultureInfo.CurrentCulture) + suffix,
            _ => (value?.ToString() ?? string.Empty) + suffix
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Maps a <see cref="TaskPriority"/> to a monochrome accent brush from the FieldOps tokens.</summary>
public sealed class TaskPriorityToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var key = value is TaskPriority p
            ? p switch
            {
                TaskPriority.High => "AccentPrimaryBrush",
                TaskPriority.Medium => "AccentTertiaryBrush",
                TaskPriority.Low => "BorderStrongBrush",
                _ => "BorderColorBrush",
            }
            : "BorderColorBrush";

        return ColorUtils.GetBrush(key);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Maps a <see cref="TaskStatus"/> to a human-readable label.</summary>
public sealed class TaskStatusToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is TaskStatus s
            ? s switch
            {
                TaskStatus.Pending => "Pending",
                TaskStatus.InProgress => "In Progress",
                TaskStatus.Completed => "Completed",
                TaskStatus.Cancelled => "Cancelled",
                _ => "Unknown",
            }
            : string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Maps a <see cref="TaskStatus"/> to a <see cref="BadgeType"/> for the StatusBadge control.</summary>
public sealed class TaskStatusToBadgeTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is TaskStatus s
            ? s switch
            {
                TaskStatus.Pending => BadgeType.Pending,
                TaskStatus.InProgress => BadgeType.InProgress,
                TaskStatus.Completed => BadgeType.Completed,
                TaskStatus.Cancelled => BadgeType.Danger,
                _ => BadgeType.Default,
            }
            : BadgeType.Default;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Returns the bound <see cref="AvatarColor"/> or <c>AvatarColor.Blue</c> if the input is null.</summary>
public sealed class AvatarColorOrDefaultConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is AvatarColor c ? c : AvatarColor.Blue;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Visible when the bound string is non-empty, Collapsed otherwise.</summary>
public sealed class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => string.IsNullOrWhiteSpace(value as string) ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Visible when the bound value is non-null, Collapsed otherwise. Handy for nullable records.</summary>
public sealed class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is null ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>ToUpperInvariant of the bound string.</summary>
public sealed class ToUpperConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is string s ? s.ToUpperInvariant() : string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>
/// Maps a <see cref="BadgeType"/> to a brush. With ConverterParameter="bg" returns the
/// theme color at 15% opacity; with "fg" (default) returns the full-opacity color.
/// </summary>
public sealed class BadgeTypeToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var key = value is BadgeType t
            ? t switch
            {
                BadgeType.Danger or BadgeType.Urgent => "Danger",
                BadgeType.Warning or BadgeType.Pending => "Warning",
                BadgeType.Success or BadgeType.Completed => "Success",
                BadgeType.Info or BadgeType.InProgress => "Info",
                BadgeType.Primary => "AccentPrimary",
                _ => "AccentTertiary",
            }
            : "AccentTertiary";

        var color = ColorUtils.GetColor(key);
        var isBackground = (parameter as string) == "bg";
        if (isBackground)
        {
            var faded = Windows.UI.Color.FromArgb((byte)(255 * 0.15), color.R, color.G, color.B);
            return new SolidColorBrush(faded);
        }
        return new SolidColorBrush(color);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Maps an <see cref="AvatarColor"/> to its avatar gradient brush resource.</summary>
public sealed class AvatarColorToGradientBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var color = value is AvatarColor c ? c : AvatarColor.Orange;
        return ColorUtils.GetBrush($"Avatar{color}Gradient");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Maps an <see cref="AgentStatus"/> to a monochrome status-dot brush.</summary>
public sealed class AgentStatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var key = value is AgentStatus s
            ? s switch
            {
                AgentStatus.OnSite or AgentStatus.Available => "AccentPrimaryBrush",
                AgentStatus.OnRoute => "AccentSecondaryBrush",
                AgentStatus.Break => "AccentTertiaryBrush",
                AgentStatus.Offline => "BorderStrongBrush",
                _ => "BorderStrongBrush",
            }
            : "BorderStrongBrush";

        return ColorUtils.GetBrush(key);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Maps an avatar's <c>Size</c> (double) to the matching initials font size.</summary>
public sealed class AvatarSizeToFontSizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var size = value is double d ? d : 48.0;
        return size switch
        {
            <= 36 => 12.0,
            <= 44 => 14.0,
            <= 48 => 16.0,
            _ => 18.0,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Formats a UTC <see cref="DateTime"/> as a relative phrase ("Just now", "5m ago", "3h ago", "Mar 4, 2:30 PM").</summary>
public sealed class TimestampToRelativeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not DateTime timestamp) return string.Empty;
        var diff = DateTime.UtcNow - timestamp;
        if (diff.TotalMinutes < 1) return "Just now";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}m ago";
        if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h ago";
        return timestamp.ToString("MMM d, h:mm tt", CultureInfo.CurrentCulture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Maps an <see cref="ActivityType"/> to its glyph (Segoe MDL2 codepoint).</summary>
public sealed class ActivityTypeToGlyphConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is ActivityType t
            ? t switch
            {
                ActivityType.TaskCompleted => "",
                ActivityType.Arrival => "",
                ActivityType.Assignment => "",
                ActivityType.Report => "",
                ActivityType.StatusChange => "",
                _ => "",
            }
            : "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Maps an <see cref="ActivityType"/> to a monochrome brush (matches the mono theme).</summary>
public sealed class ActivityTypeToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var key = value is ActivityType t
            ? t switch
            {
                ActivityType.TaskCompleted => "SuccessBrush",
                ActivityType.Arrival => "InfoBrush",
                ActivityType.Assignment => "AccentSecondaryBrush",
                ActivityType.Report => "WarningBrush",
                ActivityType.StatusChange => "TextSecondaryBrush",
                _ => "TextMutedBrush",
            }
            : "TextMutedBrush";

        return ColorUtils.GetBrush(key);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Maps a <see cref="Presentation.Controls.QuickActionAccent"/> to a monochrome brush.</summary>
public sealed class QuickActionAccentToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var key = value is QuickActionAccent a
            ? a switch
            {
                QuickActionAccent.Primary => "AccentPrimaryBrush",
                QuickActionAccent.Secondary => "AccentSecondaryBrush",
                QuickActionAccent.Tertiary => "AccentMediumBrush",
                QuickActionAccent.Success => "SuccessBrush",
                _ => "AccentPrimaryBrush",
            }
            : "AccentPrimaryBrush";

        return ColorUtils.GetBrush(key);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>
/// Maps a <see cref="Presentation.Controls.StatAccentColor"/> to a brush.
/// With ConverterParameter="bg" returns the color at 15% opacity; otherwise full opacity.
/// </summary>
public sealed class StatAccentColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var key = value is StatAccentColor a
            ? a switch
            {
                StatAccentColor.Primary => "AccentPrimary",
                StatAccentColor.Secondary => "AccentSecondary",
                StatAccentColor.Tertiary => "AccentMedium",
                StatAccentColor.Success => "Success",
                StatAccentColor.Warning => "Warning",
                StatAccentColor.Danger => "AccentTertiary",
                _ => "AccentPrimary",
            }
            : "AccentPrimary";

        var color = ColorUtils.GetColor(key);
        var isBackground = (parameter as string) == "bg";
        if (isBackground)
        {
            var faded = Windows.UI.Color.FromArgb((byte)(255 * 0.15), color.R, color.G, color.B);
            return new SolidColorBrush(faded);
        }
        return new SolidColorBrush(color);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
