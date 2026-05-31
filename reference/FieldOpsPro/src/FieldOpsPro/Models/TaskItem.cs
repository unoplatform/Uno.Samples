using FieldOpsPro.Models.Enums;

namespace FieldOpsPro.Models;

public partial record TaskItem(
    string Id,
    string OrderNumber,
    string Title,
    string Description,
    TaskPriority Priority,
    Enums.TaskStatus Status,
    TaskType Type,
    Location Location,
    string? AssigneeId = null,
    string? AssigneeName = null,
    string? AssigneeInitials = null,
    AvatarColor? AssigneeAvatarColor = null,
    string? EstimatedDuration = null,
    DateTime? ScheduledTime = null,
    DateTime CreatedAt = default,
    DateTime UpdatedAt = default,
    DateTime? SlaDeadline = null,
    string[]? PhotoUrls = null
);
