namespace Liveline.Models;

/// <summary>A single time-stamped data point in a <see cref="LivelineChart"/> series.</summary>
public record LivelinePoint(DateTimeOffset Time, double Value);
