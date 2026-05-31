namespace FieldOpsPro.Models;

public record StatValue(
    int Current,
    int? Previous = null,
    double? ChangePercent = null
);

public record DashboardStats(
    StatValue ActiveWorkOrders,
    StatValue OnlineAgents,
    StatValue AvgResponseTime,
    StatValue CompletionRate,
    DateTime LastUpdated
);
