namespace GridWatch.Models;

public class DashboardViewModel
{
    public List<KpiMetric> KpiMetrics { get; set; } = new();
    public List<Facility> Facilities { get; set; } = new();
    public List<Alert> Alerts { get; set; } = new();
}
