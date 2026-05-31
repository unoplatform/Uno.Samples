namespace Liveline.Models;

/// <summary>
/// A single data point on the chart.
/// </summary>
public record LivelinePoint(DateTimeOffset Time, double Value);
