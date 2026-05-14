using System.Collections.Immutable;

namespace GridWatch.Services;

public interface IGridDataService
{
	ValueTask<IImmutableList<KpiMetric>> GetKpiMetricsAsync(CancellationToken ct = default);
	ValueTask<IImmutableList<Facility>> GetFacilitiesAsync(CancellationToken ct = default);
	ValueTask<IImmutableList<FacilityRow>> GetFacilityRowsAsync(CancellationToken ct = default);
	ValueTask<IImmutableList<Alert>> GetAlertsAsync(CancellationToken ct = default);
}

public class GridDataService : IGridDataService
{
	private static readonly ImmutableArray<KpiMetric> _kpiMetrics = ImmutableArray.Create(
		new KpiMetric("demand", "TOTAL DEMAND", "48,320", "MW", "+1.2%", DeltaDirection.Up),
		new KpiMetric("supply", "TOTAL SUPPLY", "51,140", "MW", "+0.8%", DeltaDirection.Up),
		new KpiMetric("reserve", "RESERVE MARGIN", "5.83", "%", "-0.4%", DeltaDirection.Down),
		new KpiMetric("frequency", "GRID FREQUENCY", "59.98", "Hz", "-0.02", DeltaDirection.Down)
	);

	private static readonly ImmutableArray<Facility> _facilities = ImmutableArray.Create(
		new Facility("f1", "Hoover Dam", "Hydro", "Southwest", "2,080", "1,950", FacilityStatus.Online),
		new Facility("f2", "Diablo Canyon", "Nuclear", "West", "2,256", "2,200", FacilityStatus.Online),
		new Facility("f3", "Mojave Solar", "Solar", "Southwest", "1,600", "1,420", FacilityStatus.Online),
		new Facility("f4", "Pecos Wind Farm", "Wind", "South", "900", "610", FacilityStatus.Warning),
		new Facility("f5", "Four Corners Plant", "Coal", "Southwest", "2,040", "1,200", FacilityStatus.Critical),
		new Facility("f6", "Beacon Hill Gas", "Gas", "Northeast", "800", "760", FacilityStatus.Online),
		new Facility("f7", "Altamont Wind", "Wind", "West", "580", "490", FacilityStatus.Online),
		new Facility("f8", "Navajo Generating", "Coal", "Southwest", "2,250", "0", FacilityStatus.Critical)
	);

	private static readonly ImmutableArray<FacilityRow> _facilityRows = ImmutableArray.Create(
		new FacilityRow("F01", "Hoover Dam", "Southwest", "Hydro", "2080", "1843", FacilityStatus.Online),
		new FacilityRow("F02", "Grand Coulee", "Northwest", "Hydro", "6809", "6201", FacilityStatus.Online),
		new FacilityRow("F03", "Palo Verde", "Southwest", "Nuclear", "3942", "3610", FacilityStatus.Online),
		new FacilityRow("F04", "Robert Moses", "Northeast", "Hydro", "2429", "2115", FacilityStatus.Online),
		new FacilityRow("F05", "Moss Landing", "West", "Battery", "1060", "820", FacilityStatus.Warning),
		new FacilityRow("F06", "Navajo Station", "Southwest", "Coal", "2250", "0", FacilityStatus.Critical),
		new FacilityRow("F07", "Browns Ferry", "Southeast", "Nuclear", "3456", "3100", FacilityStatus.Online),
		new FacilityRow("F08", "Diablo Canyon", "West", "Nuclear", "2256", "1980", FacilityStatus.Online),
		new FacilityRow("F09", "Comanche Peak", "South", "Nuclear", "2430", "2200", FacilityStatus.Online),
		new FacilityRow("F10", "Prairie Island", "Midwest", "Nuclear", "1100", "870", FacilityStatus.Warning),
		new FacilityRow("F11", "Lake Keowee", "Southeast", "Hydro", "760", "710", FacilityStatus.Online),
		new FacilityRow("F12", "Calvert Cliffs", "Northeast", "Nuclear", "1825", "0", FacilityStatus.Critical),
		new FacilityRow("F13", "Mojave Solar", "Southwest", "Solar", "1600", "1420", FacilityStatus.Online),
		new FacilityRow("F14", "Pecos Wind Farm", "South", "Wind", "900", "610", FacilityStatus.Warning),
		new FacilityRow("F15", "Beacon Hill Gas", "Northeast", "Gas", "800", "760", FacilityStatus.Online)
	);

	private static readonly ImmutableArray<Alert> _alerts = ImmutableArray.Create(
		new Alert("a1", AlertSeverity.Critical, "Four Corners Plant: Unit 3 tripped offline — automatic shutdown initiated.", "Four Corners Plant", "4 min ago"),
		new Alert("a2", AlertSeverity.Critical, "Navajo Generating Station: All units offline — emergency grid import required.", "Navajo Generating", "11 min ago"),
		new Alert("a3", AlertSeverity.Critical, "Calvert Cliffs: Reactor coolant pressure anomaly — unit placed in safe shutdown.", "Calvert Cliffs", "19 min ago"),
		new Alert("a4", AlertSeverity.Warning, "Pecos Wind Farm: Output below forecast by 32% — sustained wind speed dropping.", "Pecos Wind Farm", "25 min ago"),
		new Alert("a5", AlertSeverity.Warning, "Reserve margin approaching 5% threshold — standby peakers placed on notice.", "System", "33 min ago"),
		new Alert("a6", AlertSeverity.Warning, "Moss Landing: Battery storage SOC below 20% — curtailing exports.", "Moss Landing", "48 min ago"),
		new Alert("a7", AlertSeverity.Warning, "Prairie Island: Unit 2 scheduled maintenance overrun by 6 hours.", "Prairie Island", "1 hr ago"),
		new Alert("a8", AlertSeverity.Info, "Mojave Solar: Daily generation target reached 18 minutes ahead of schedule.", "Mojave Solar", "1 hr ago"),
		new Alert("a9", AlertSeverity.Info, "Diablo Canyon: Routine coolant system inspection completed — no anomalies detected.", "Diablo Canyon", "2 hr ago"),
		new Alert("a10", AlertSeverity.Info, "Grand Coulee: Turbine 7 maintenance window completed — unit returned to service.", "Grand Coulee", "3 hr ago")
	);

	public ValueTask<IImmutableList<KpiMetric>> GetKpiMetricsAsync(CancellationToken ct = default)
		=> ValueTask.FromResult<IImmutableList<KpiMetric>>(_kpiMetrics);

	public ValueTask<IImmutableList<Facility>> GetFacilitiesAsync(CancellationToken ct = default)
		=> ValueTask.FromResult<IImmutableList<Facility>>(_facilities);

	public ValueTask<IImmutableList<FacilityRow>> GetFacilityRowsAsync(CancellationToken ct = default)
		=> ValueTask.FromResult<IImmutableList<FacilityRow>>(_facilityRows);

	public ValueTask<IImmutableList<Alert>> GetAlertsAsync(CancellationToken ct = default)
		=> ValueTask.FromResult<IImmutableList<Alert>>(_alerts);
}
