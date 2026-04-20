using Microsoft.UI.Xaml.Media;

namespace GridWatch.Models;

public enum AlertSeverity
{
    Info,
    Warning,
    Critical
}

public class Alert
{
    public string Id { get; set; } = string.Empty;
    public AlertSeverity Severity { get; set; }
    public string Message { get; set; } = string.Empty;
    public string FacilityName { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }

    public string TimestampDisplay
    {
        get
        {
            var diff = DateTimeOffset.Now - Timestamp;
            if (diff.TotalMinutes < 1) return "just now";
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} min ago";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} hr ago";
            return $"{(int)diff.TotalDays} day ago";
        }
    }

    public SolidColorBrush SeverityBrush => Severity switch
    {
        AlertSeverity.Critical => new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 0xF5, 0x43, 0x2E)),
        AlertSeverity.Warning  => new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 0xF5, 0x9E, 0x0B)),
        AlertSeverity.Info     => new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 0x3D, 0x98, 0xE0)),
        _                      => new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 0x88, 0x96, 0xA8))
    };

    public SolidColorBrush RowBackground => Severity switch
    {
        AlertSeverity.Critical => new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 0x2E, 0x1A, 0x1A)),
        AlertSeverity.Warning  => new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 0x2E, 0x2A, 0x1A)),
        _                      => new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(0, 0, 0, 0))
    };
}
