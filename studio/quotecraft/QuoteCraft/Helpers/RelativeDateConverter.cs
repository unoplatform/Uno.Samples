using Microsoft.UI.Xaml.Data;

namespace QuoteCraft.Helpers;

public class RelativeDateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        DateTimeOffset date;
        if (value is DateTimeOffset dto)
            date = dto;
        else if (value is string dateStr && DateTimeOffset.TryParse(dateStr, out date))
        { }
        else
            return string.Empty;

        return RelativeDate.Format(date, DateTimeOffset.Now);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>
/// "Just now", "3 days ago", "Tomorrow", "In 2 weeks": a date relative to now, in the past or the future (a quote's
/// valid-until date is usually ahead). Calendar words (Today, Yesterday, Tomorrow) use the calendar day in the time
/// zone of the "now" passed in (the device's local time in the app).
/// </summary>
public static class RelativeDate
{
    public static string Format(DateTimeOffset date, DateTimeOffset now)
    {
        var diff = now - date;
        var days = (date.ToOffset(now.Offset).Date - now.Date).Days; // calendar days ahead (negative: behind)

        if (diff < TimeSpan.Zero)
        {
            return days switch
            {
                0 => "Today",
                1 => "Tomorrow",
                < 7 => $"In {days} days",
                < 30 => Plural("In", days / 7, "week"),
                < 365 => Plural("In", days / 30, "month"),
                _ => date.ToString("MMM d, yyyy"),
            };
        }

        if (diff.TotalMinutes < 1) return "Just now";
        if (diff.TotalHours < 1) return $"{(int)diff.TotalMinutes}m ago";
        if (days == 0) return "Today";
        if (days == -1) return "Yesterday";
        if (-days < 7) return $"{-days} days ago";
        var ago = (int)diff.TotalDays;
        if (diff.TotalDays < 30) return Plural(null, ago / 7, "week") + " ago";
        if (diff.TotalDays < 365) return Plural(null, ago / 30, "month") + " ago";

        return date.ToString("MMM d, yyyy");
    }

    private static string Plural(string? prefix, int count, string unit)
    {
        var text = $"{count} {unit}{(count == 1 ? string.Empty : "s")}";
        return prefix is null ? text : $"{prefix} {text}";
    }
}
