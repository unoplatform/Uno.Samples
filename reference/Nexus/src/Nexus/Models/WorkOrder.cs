namespace Nexus.Models;

public enum WorkOrderType
{
    Preventive,
    Corrective,
    Emergency
}

public enum WorkOrderStatus
{
    Scheduled,
    InProgress,
    Completed,
    Cancelled
}

public partial record WorkOrder(
    string Id,
    string EquipmentId,
    string EquipmentName,
    WorkOrderType Type,
    DateTime ScheduledDate,
    string TechnicianId,
    string TechnicianName,
    WorkOrderStatus Status
);
