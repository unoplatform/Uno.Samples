using FieldOpsPro.Models.Enums;

namespace FieldOpsPro.Models;

public partial record TeamMember(
    string Id,
    string Name,
    string Initials,
    AvatarColor AvatarColor,
    AgentStatus Status,
    string LocationDescription,
    string? Distance = null,
    int BatteryLevel = 100,
    int SignalStrength = 4
);
