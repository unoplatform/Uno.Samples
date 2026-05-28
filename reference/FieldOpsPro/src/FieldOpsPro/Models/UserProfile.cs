using FieldOpsPro.Models.Enums;

namespace FieldOpsPro.Models;

public partial record UserProfile(
    string Id,
    string Name,
    string Initials,
    string Role,
    AvatarColor AvatarColor,
    AgentStatus Status
);
