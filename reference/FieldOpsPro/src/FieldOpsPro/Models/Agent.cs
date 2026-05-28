using FieldOpsPro.Models.Enums;

namespace FieldOpsPro.Models;

public partial record Agent(
    string Id,
    string Name,
    string Initials,
    AvatarColor AvatarColor,
    AgentStatus Status,
    Location Location,
    string? CurrentTaskId = null,
    string? CurrentTaskTitle = null,
    string? Eta = null,
    DateTime? BreakReturnTime = null
);
