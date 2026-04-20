using Microsoft.UI.Xaml.Media;

namespace GridWatch.Models;

public class FacilityRow
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Capacity { get; set; } = string.Empty;
    public string Output { get; set; } = string.Empty;

    private FacilityStatus _status;
    public FacilityStatus Status
    {
        get => _status;
        set
        {
            _status = value;
            UpdateStatusBrushes();
        }
    }

    public string StatusDisplay => _status switch
    {
        FacilityStatus.Online   => "Online",
        FacilityStatus.Warning  => "Warning",
        FacilityStatus.Critical => "Critical",
        _                       => "Unknown"
    };

    public SolidColorBrush StatusBackground { get; private set; } = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
    public SolidColorBrush StatusForeground { get; private set; } = new SolidColorBrush(Microsoft.UI.Colors.White);

    public SolidColorBrush RowBackground => new SolidColorBrush(Microsoft.UI.Colors.Transparent);

    private void UpdateStatusBrushes()
    {
        switch (_status)
        {
            case FacilityStatus.Online:
                StatusBackground = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(0x33, 0x18, 0xD9, 0x6A));
                StatusForeground = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(0xFF, 0x18, 0xD9, 0x6A));
                break;
            case FacilityStatus.Warning:
                StatusBackground = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(0x33, 0xF5, 0x9E, 0x0B));
                StatusForeground = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(0xFF, 0xF5, 0x9E, 0x0B));
                break;
            case FacilityStatus.Critical:
                StatusBackground = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(0x33, 0xF5, 0x43, 0x2E));
                StatusForeground = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(0xFF, 0xF5, 0x43, 0x2E));
                break;
            default:
                StatusBackground = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
                StatusForeground = new SolidColorBrush(Microsoft.UI.Colors.White);
                break;
        }
    }
}
