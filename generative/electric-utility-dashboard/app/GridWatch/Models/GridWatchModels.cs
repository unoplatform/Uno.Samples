using System.Collections.Immutable;

namespace GridWatch.Models;

public enum AlertSeverity { Info, Warning, Critical }

public enum FacilityStatus { Online, Warning, Critical }

public partial record Alert(
	string Id,
	AlertSeverity Severity,
	string Message,
	string FacilityName,
	string TimestampDisplay);

public partial record Facility(
	string Id,
	string Name,
	string Type,
	string Region,
	string Capacity,
	string Output,
	FacilityStatus Status)
{
	public string StatusLabel => Status.ToString();
}

public partial record FacilityRow(
	string Id,
	string Name,
	string Region,
	string Type,
	string Capacity,
	string Output,
	FacilityStatus Status)
{
	public string StatusDisplay => Status switch
	{
		FacilityStatus.Online => "Online",
		FacilityStatus.Warning => "Warning",
		FacilityStatus.Critical => "Critical",
		_ => "Unknown"
	};
}

public enum DeltaDirection { Up, Down, Neutral }

public partial record KpiMetric(
	string Id,
	string Label,
	string Value,
	string Unit,
	string Delta,
	DeltaDirection DeltaDirection);

public partial record StatusCounts(int Online, int Warning, int Critical, int Total)
{
	public string OnlineText => $"{Online} Online";
	public string WarningText => $"{Warning} Warning";
	public string CriticalText => $"{Critical} Critical";
	public string TotalText => $"{Total} facilities total";
}
