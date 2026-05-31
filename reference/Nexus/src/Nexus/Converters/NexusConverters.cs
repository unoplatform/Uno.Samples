using System;
using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Nexus.Models;

namespace Nexus.Converters;

/// <summary>Formats a numeric value with the .NET format string passed as ConverterParameter (e.g. "F1", "N0").</summary>
public sealed class NumberFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var format = parameter as string ?? "G";
        return value switch
        {
            double d => d.ToString(format, CultureInfo.CurrentCulture),
            int i => i.ToString(format, CultureInfo.CurrentCulture),
            _ => value?.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Turns a signed trend value into a "+12.5%" / "-8.3%" label.</summary>
public sealed class TrendTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is double d ? $"{(d >= 0 ? "+" : "-")}{Math.Abs(d):F1}%" : string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Maps a trend sign to the success (non-negative) or danger (negative) brush.</summary>
public sealed class TrendBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => Brush(value is double d && d < 0 ? "NexusDangerBrush" : "NexusSuccessBrush");

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();

    internal static object Brush(string key) => Application.Current.Resources[key];
}

/// <summary>Maps a <see cref="LineStatus"/> to its short table label.</summary>
public sealed class LineStatusTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is LineStatus s
            ? s switch
            {
                LineStatus.Active => "ACTIVE",
                LineStatus.Standby => "STANDBY",
                LineStatus.Maintenance => "MAINT",
                _ => s.ToString().ToUpperInvariant()
            }
            : string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Maps a <see cref="LineStatus"/> to its semantic status brush.</summary>
public sealed class LineStatusBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => TrendBrushConverter.Brush(value is LineStatus s
            ? s switch
            {
                LineStatus.Active => "NexusSuccessBrush",
                LineStatus.Standby => "NexusWarningBrush",
                LineStatus.Maintenance => "NexusDangerBrush",
                _ => "NexusTextSecondaryBrush"
            }
            : "NexusTextSecondaryBrush");

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Maps an <see cref="AlertType"/> to its semantic status brush.</summary>
public sealed class AlertBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => TrendBrushConverter.Brush(value is AlertType t
            ? t switch
            {
                AlertType.Critical => "NexusDangerBrush",
                AlertType.Warning => "NexusWarningBrush",
                AlertType.Success => "NexusSuccessBrush",
                _ => "NexusInfoBrush"
            }
            : "NexusInfoBrush");

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Formats a <see cref="DateTime"/> as a 24-hour clock time.</summary>
public sealed class TimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is DateTime dt ? dt.ToString("HH:mm:ss", CultureInfo.CurrentCulture) : string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Formats a <see cref="DateTime"/> with the .NET format from ConverterParameter (default "MMM dd").</summary>
public sealed class DateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is DateTime dt ? dt.ToString(parameter as string ?? "MMM dd", CultureInfo.CurrentCulture) : string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Uppercases an enum/string value for table labels (e.g. Preventive -> "PREVENTIVE").</summary>
public sealed class EnumUpperConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value?.ToString()?.ToUpperInvariant() ?? string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Maps a <see cref="StockStatus"/> to its semantic status brush.</summary>
public sealed class StockStatusBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => TrendBrushConverter.Brush(value is StockStatus s
            ? s switch
            {
                StockStatus.InStock => "NexusSuccessBrush",
                StockStatus.LowStock => "NexusWarningBrush",
                StockStatus.OutOfStock => "NexusDangerBrush",
                _ => "NexusTextSecondaryBrush"
            }
            : "NexusTextSecondaryBrush");

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Maps a <see cref="BatchPriority"/> to its semantic status brush.</summary>
public sealed class PriorityBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => TrendBrushConverter.Brush(value is BatchPriority p
            ? p switch
            {
                BatchPriority.Critical => "NexusDangerBrush",
                BatchPriority.High => "NexusWarningBrush",
                BatchPriority.Normal => "NexusInfoBrush",
                _ => "NexusTextSecondaryBrush"
            }
            : "NexusTextSecondaryBrush");

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Maps a <see cref="WorkOrderType"/> to its semantic status brush.</summary>
public sealed class WorkOrderTypeBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => TrendBrushConverter.Brush(value is WorkOrderType t
            ? t switch
            {
                WorkOrderType.Emergency => "NexusDangerBrush",
                WorkOrderType.Corrective => "NexusWarningBrush",
                _ => "NexusInfoBrush"
            }
            : "NexusInfoBrush");

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Maps a <see cref="WorkOrderStatus"/> to its semantic status brush.</summary>
public sealed class WorkOrderStatusBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => TrendBrushConverter.Brush(value is WorkOrderStatus s
            ? s switch
            {
                WorkOrderStatus.InProgress => "NexusWarningBrush",
                WorkOrderStatus.Completed => "NexusSuccessBrush",
                WorkOrderStatus.Cancelled => "NexusDangerBrush",
                _ => "NexusInfoBrush"
            }
            : "NexusInfoBrush");

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Maps a 0-100 health value to a success/warning/danger brush.</summary>
public sealed class HealthBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => TrendBrushConverter.Brush(value is double h
            ? h >= 85 ? "NexusSuccessBrush" : h >= 70 ? "NexusWarningBrush" : "NexusDangerBrush"
            : "NexusTextSecondaryBrush");

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Maps a <see cref="UserStatus"/> to its semantic status brush.</summary>
public sealed class UserStatusBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => TrendBrushConverter.Brush(value is UserStatus s && s == UserStatus.Active
            ? "NexusSuccessBrush" : "NexusTextTertiaryBrush");

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
