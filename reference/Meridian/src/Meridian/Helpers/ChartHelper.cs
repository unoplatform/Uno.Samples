using Meridian.Models;

namespace Meridian.Helpers;

public static class ChartHelper
{
	public static IImmutableList<ChartPoint> FilterByTimeframe(IImmutableList<ChartPoint> points, string timeframe)
	{
		if (points.Count < 2) return points;
		if (!DateTime.TryParse(points[^1].Date, out var latest)) return points;

		var cutoff = timeframe switch
		{
			"1D" => latest.AddDays(-1),
			"1W" => latest.AddDays(-7),
			"1M" => latest.AddMonths(-1),
			"3M" => latest.AddMonths(-3),
			"6M" => latest.AddMonths(-6),
			"YTD" => new DateTime(latest.Year, 1, 1),
			"1Y" => latest.AddYears(-1),
			"5Y" => latest.AddYears(-5),
			"ALL" => DateTime.MinValue,
			_ => latest.AddMonths(-3)
		};

		var filtered = points
			.Where(p => DateTime.TryParse(p.Date, out var d) && d >= cutoff)
			.ToImmutableList();

		// Ensure at least 2 points for chart rendering
		return filtered.Count >= 2 ? filtered : points;
	}
}
