using Nexus.Models;

namespace Nexus.Services;

public class MockNexusService : INexusService
{
    private readonly Random _random = new();

    public ValueTask<DashboardMetrics> GetDashboardMetricsAsync(CancellationToken ct)
    {
        var metrics = new DashboardMetrics(
            Throughput: 847.2 + (_random.NextDouble() - 0.5) * 4,
            ThroughputTrend: 2.4,
            Efficiency: 94.7 + (_random.NextDouble() - 0.5),
            EfficiencyTrend: 0.8,
            Uptime: 99.2 + (_random.NextDouble() * 0.2 - 0.1),
            UptimeTrend: -0.1,
            EnergyConsumption: 1.24 + (_random.NextDouble() - 0.5) * 0.04,
            EnergyTrend: -3.2
        );
        return ValueTask.FromResult(metrics);
    }

    public ValueTask<AnalyticsMetrics> GetAnalyticsMetricsAsync(CancellationToken ct)
    {
        var metrics = new AnalyticsMetrics(
            OEE: 87.3,
            OEETrend: 2.1,
            MTBF: 847,
            MTBFTrend: 5.2,
            MTTR: 2.4,
            MTTRTrend: -12.5,
            FPY: 98.7,
            FPYTrend: 0.3
        );
        return ValueTask.FromResult(metrics);
    }

    public ValueTask<IImmutableList<ProductionLine>> GetProductionLinesAsync(CancellationToken ct)
    {
        var lines = new List<ProductionLine>
        {
            new("A1", "Assembly Line A1", LineStatus.Active, 87 + _random.Next(-2, 3), 72.4 + _random.NextDouble(), 4.2),
            new("A2", "Assembly Line A2", LineStatus.Active, 92 + _random.Next(-2, 3), 71.8 + _random.NextDouble(), 4.1),
            new("B1", "Processing Unit B1", LineStatus.Standby, 0, 45.2, 2.8),
            new("C1", "Packaging Line C1", LineStatus.Active, 78 + _random.Next(-2, 3), 68.5 + _random.NextDouble(), 3.9),
            new("C2", "Packaging Line C2", LineStatus.Maintenance, 0, 22.1, 0.0)
        };
        return ValueTask.FromResult<IImmutableList<ProductionLine>>(lines.ToImmutableList());
    }

    public ValueTask<IImmutableList<Batch>> GetBatchQueueAsync(CancellationToken ct)
    {
        var batches = new List<Batch>
        {
            new("B-2024-001", "Widget Assembly Kit", 1500, BatchPriority.High, DateTime.Now.AddHours(2)),
            new("B-2024-002", "Sensor Module Pack", 800, BatchPriority.Normal, DateTime.Now.AddHours(4)),
            new("B-2024-003", "Control Unit Set", 2000, BatchPriority.Critical, DateTime.Now.AddHours(1)),
            new("B-2024-004", "Housing Components", 1200, BatchPriority.Low, DateTime.Now.AddHours(6))
        };
        return ValueTask.FromResult<IImmutableList<Batch>>(batches.ToImmutableList());
    }

    public ValueTask<ShiftProgress> GetShiftProgressAsync(CancellationToken ct)
    {
        var now = DateTime.Now;
        var shiftStart = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0);
        var shiftEnd = shiftStart.AddHours(8);
        var elapsed = (now - shiftStart).TotalHours;
        var progress = Math.Clamp(elapsed / 8.0 * 100, 0, 100);

        var shift = new ShiftProgress(
            ShiftName: "Day Shift A",
            StartTime: shiftStart,
            EndTime: shiftEnd,
            ProgressPercent: progress,
            UnitsProduced: (int)(2400 * progress / 100),
            TargetUnits: 2400
        );
        return ValueTask.FromResult(shift);
    }

    public ValueTask<IImmutableList<Material>> GetMaterialsAsync(CancellationToken ct)
    {
        var materials = new List<Material>
        {
            new("M001", "Steel Plates", 2450, 500, "units", StockStatus.InStock),
            new("M002", "Copper Wire", 180, 200, "meters", StockStatus.LowStock),
            new("M003", "Plastic Pellets", 5200, 1000, "kg", StockStatus.InStock),
            new("M004", "Lubricant Oil", 45, 50, "liters", StockStatus.LowStock),
            new("M005", "Electronic Components", 890, 300, "units", StockStatus.InStock)
        };
        return ValueTask.FromResult<IImmutableList<Material>>(materials.ToImmutableList());
    }

    public ValueTask<IImmutableList<Equipment>> GetEquipmentAsync(CancellationToken ct)
    {
        var equipment = new List<Equipment>
        {
            new("EQ001", "CNC Mill #1", 94, DateTime.Now.AddDays(15), 4520),
            new("EQ002", "CNC Mill #2", 87, DateTime.Now.AddDays(8), 3890),
            new("EQ003", "Robot Arm R-05", 72, DateTime.Now.AddDays(3), 6240),
            new("EQ004", "Robot Arm R-07", 91, DateTime.Now.AddDays(22), 5180),
            new("EQ005", "Conveyor Belt A2", 68, DateTime.Now.AddDays(1), 8920),
            new("EQ006", "Hydraulic Press #1", 85, DateTime.Now.AddDays(12), 3450),
            new("EQ007", "Packaging Station P1", 96, DateTime.Now.AddDays(30), 2100),
            new("EQ008", "Quality Scanner QS-1", 78, DateTime.Now.AddDays(5), 4680)
        };
        return ValueTask.FromResult<IImmutableList<Equipment>>(equipment.ToImmutableList());
    }

    public ValueTask<IImmutableList<WorkOrder>> GetWorkOrdersAsync(CancellationToken ct)
    {
        var orders = new List<WorkOrder>
        {
            new("WO-001", "EQ003", "Robot Arm R-05", WorkOrderType.Preventive, DateTime.Now.AddDays(3), "T001", "J. Martinez", WorkOrderStatus.Scheduled),
            new("WO-002", "EQ005", "Conveyor Belt A2", WorkOrderType.Corrective, DateTime.Now.AddDays(1), "T002", "S. Johnson", WorkOrderStatus.InProgress),
            new("WO-003", "EQ002", "CNC Mill #2", WorkOrderType.Preventive, DateTime.Now.AddDays(8), "T001", "J. Martinez", WorkOrderStatus.Scheduled),
            new("WO-004", "EQ008", "Quality Scanner QS-1", WorkOrderType.Emergency, DateTime.Now, "T003", "A. Williams", WorkOrderStatus.InProgress)
        };
        return ValueTask.FromResult<IImmutableList<WorkOrder>>(orders.ToImmutableList());
    }

    public ValueTask<IImmutableList<SparePart>> GetSparePartsAsync(CancellationToken ct)
    {
        var parts = new List<SparePart>
        {
            new("SP001", "Drive Belt 50mm", 24, 10, StockStatus.InStock),
            new("SP002", "Ball Bearing 6205", 8, 15, StockStatus.LowStock),
            new("SP003", "Hydraulic Seal Kit", 12, 5, StockStatus.InStock),
            new("SP004", "Motor Brushes", 0, 8, StockStatus.OutOfStock),
            new("SP005", "Sensor Module SM-7", 18, 10, StockStatus.InStock)
        };
        return ValueTask.FromResult<IImmutableList<SparePart>>(parts.ToImmutableList());
    }

    public ValueTask<IImmutableList<Alert>> GetRecentAlertsAsync(CancellationToken ct, int limit = 10)
    {
        var alerts = new List<Alert>
        {
            new(DateTime.Now.AddMinutes(-2), AlertType.Info, "System backup completed successfully"),
            new(DateTime.Now.AddMinutes(-15), AlertType.Warning, "Material M002 stock below threshold"),
            new(DateTime.Now.AddMinutes(-32), AlertType.Critical, "Equipment EQ005 requires immediate attention"),
            new(DateTime.Now.AddMinutes(-45), AlertType.Info, "Shift handover initiated"),
            new(DateTime.Now.AddHours(-1), AlertType.Success, "Batch B-2024-001 completed ahead of schedule"),
            new(DateTime.Now.AddHours(-2), AlertType.Warning, "Temperature spike detected on Line A1"),
            new(DateTime.Now.AddHours(-3), AlertType.Info, "New work order WO-004 created"),
            new(DateTime.Now.AddHours(-4), AlertType.Info, "User admin@nexus.io logged in"),
            new(DateTime.Now.AddHours(-5), AlertType.Success, "Equipment EQ001 maintenance completed"),
            new(DateTime.Now.AddHours(-6), AlertType.Warning, "Network latency detected on subnet 10.0.2.x")
        };
        return ValueTask.FromResult<IImmutableList<Alert>>(alerts.Take(limit).ToImmutableList());
    }

    public ValueTask<SystemSettings> GetSettingsAsync(CancellationToken ct)
    {
        var settings = new SystemSettings(
            AutoBackup: true,
            EmailAlerts: true,
            SoundAlerts: false,
            DataRetention: "90 days",
            TemperatureUnit: "Celsius",
            RefreshRate: 5,
            TemperatureThresholdHigh: 85.0,
            TemperatureThresholdLow: 15.0,
            PressureThresholdHigh: 6.0,
            PressureThresholdLow: 2.0
        );
        return ValueTask.FromResult(settings);
    }

    public ValueTask<IImmutableList<User>> GetUsersAsync(CancellationToken ct)
    {
        var users = new List<User>
        {
            new("U001", "Admin User", "admin@nexus.io", UserRole.Admin, DateTime.Now.AddMinutes(-30), UserStatus.Active),
            new("U002", "M. Chen", "m.chen@nexus.io", UserRole.Supervisor, DateTime.Now.AddHours(-2), UserStatus.Active),
            new("U003", "J. Santos", "j.santos@nexus.io", UserRole.Technician, DateTime.Now.AddHours(-4), UserStatus.Active),
            new("U004", "R. Kim", "r.kim@nexus.io", UserRole.Operator, DateTime.Now.AddDays(-1), UserStatus.Inactive)
        };
        return ValueTask.FromResult<IImmutableList<User>>(users.ToImmutableList());
    }

    public ValueTask<IImmutableList<double>> GetWeeklyOutputAsync(CancellationToken ct)
    {
        var output = new List<double> { 2450, 2680, 2520, 2890, 2750, 1200, 980 };
        return ValueTask.FromResult<IImmutableList<double>>(output.ToImmutableList());
    }

    public ValueTask<IImmutableList<double>> GetEfficiencyTrendAsync(CancellationToken ct)
    {
        var trend = new List<double> { 88.2, 89.5, 87.8, 90.1, 91.3, 89.7, 92.4, 91.8, 93.2, 92.1, 94.5, 94.2 };
        return ValueTask.FromResult<IImmutableList<double>>(trend.ToImmutableList());
    }
}
