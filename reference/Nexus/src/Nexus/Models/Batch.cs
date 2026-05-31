namespace Nexus.Models;

public enum BatchPriority
{
    Low,
    Normal,
    High,
    Critical
}

public partial record Batch(
    string Id,
    string ProductName,
    int Quantity,
    BatchPriority Priority,
    DateTime EstimatedCompletion
);
