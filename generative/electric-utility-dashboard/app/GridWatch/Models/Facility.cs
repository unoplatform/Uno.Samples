using Microsoft.UI.Xaml.Media;

namespace GridWatch.Models;

public enum FacilityStatus
{
    Online,
    Warning,
    Critical
}

public class Facility
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Capacity { get; set; } = string.Empty;
    public string Output { get; set; } = string.Empty;
    public FacilityStatus Status { get; set; } = FacilityStatus.Online;

    public SolidColorBrush StatusBrush => Status switch
    {
        FacilityStatus.Online   => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0x18, 0xD9, 0x6A)),
        FacilityStatus.Warning  => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0xF5, 0x9E, 0x0B)),
        FacilityStatus.Critical => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0xF5, 0x43, 0x2E)),
        _                       => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0x18, 0xD9, 0x6A))
    };

    public string StatusLabel => Status.ToString();
}
