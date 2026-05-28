using Nexus.Models;

namespace Nexus.Services;

public interface INexusService
{
    // Dashboard Metrics
    ValueTask<DashboardMetrics> GetDashboardMetricsAsync(CancellationToken ct);
    ValueTask<AnalyticsMetrics> GetAnalyticsMetricsAsync(CancellationToken ct);

    // Production
    ValueTask<IImmutableList<ProductionLine>> GetProductionLinesAsync(CancellationToken ct);
    ValueTask<IImmutableList<Batch>> GetBatchQueueAsync(CancellationToken ct);
    ValueTask<ShiftProgress> GetShiftProgressAsync(CancellationToken ct);
    ValueTask<IImmutableList<Material>> GetMaterialsAsync(CancellationToken ct);

    // Maintenance
    ValueTask<IImmutableList<Equipment>> GetEquipmentAsync(CancellationToken ct);
    ValueTask<IImmutableList<WorkOrder>> GetWorkOrdersAsync(CancellationToken ct);
    ValueTask<IImmutableList<SparePart>> GetSparePartsAsync(CancellationToken ct);

    // Alerts
    ValueTask<IImmutableList<Alert>> GetRecentAlertsAsync(CancellationToken ct, int limit = 10);

    // Settings
    ValueTask<SystemSettings> GetSettingsAsync(CancellationToken ct);
    ValueTask<IImmutableList<User>> GetUsersAsync(CancellationToken ct);

    // Analytics Data
    ValueTask<IImmutableList<double>> GetWeeklyOutputAsync(CancellationToken ct);
    ValueTask<IImmutableList<double>> GetEfficiencyTrendAsync(CancellationToken ct);
}
