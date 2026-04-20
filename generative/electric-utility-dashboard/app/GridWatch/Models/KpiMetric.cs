using Microsoft.UI.Xaml.Media;

namespace GridWatch.Models;

public enum DeltaDirection { Up, Down, Neutral }

public class KpiMetric
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Delta { get; set; } = string.Empty;
    public DeltaDirection DeltaDirection { get; set; } = DeltaDirection.Neutral;

    public SolidColorBrush DeltaBackground => DeltaDirection switch
    {
        DeltaDirection.Up      => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0x1A, 0x2E, 0x1A)),
        DeltaDirection.Down    => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0x2E, 0x1A, 0x1A)),
        _                      => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0x1E, 0x24, 0x30)),
    };

    public SolidColorBrush DeltaForeground => DeltaDirection switch
    {
        DeltaDirection.Up      => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0x18, 0xD9, 0x6A)),
        DeltaDirection.Down    => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0xF5, 0x43, 0x2E)),
        _                      => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0x88, 0x96, 0xA8)),
    };
}
