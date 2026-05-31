using FieldOpsPro.Models.Enums;

namespace FieldOpsPro.Models;

public partial record Activity(
    string Id,
    ActivityType Type,
    string ActorId,
    string ActorName,
    string Message,
    DateTime Timestamp,
    string? TargetId = null
);
