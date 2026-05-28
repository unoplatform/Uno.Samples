namespace Nexus.Models;

public partial record DashboardMetrics(
    double Throughput,
    double ThroughputTrend,
    double Efficiency,
    double EfficiencyTrend,
    double Uptime,
    double UptimeTrend,
    double EnergyConsumption,
    double EnergyTrend
);

public partial record AnalyticsMetrics(
    double OEE,
    double OEETrend,
    double MTBF,
    double MTBFTrend,
    double MTTR,
    double MTTRTrend,
    double FPY,
    double FPYTrend
);

public partial record ShiftProgress(
    string ShiftName,
    DateTime StartTime,
    DateTime EndTime,
    double ProgressPercent,
    int UnitsProduced,
    int TargetUnits
);
