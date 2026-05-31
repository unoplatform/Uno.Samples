using FieldOpsPro.Models.Enums;

namespace FieldOpsPro.Models;

public record CurrentAssignment(
    AgentStatus Status,
    string? Destination = null,
    string? Eta = null,
    Location? DestinationLocation = null
)
{
    /// <summary>Human-friendly status label for the shift header.</summary>
    public string StatusDisplay => Status switch
    {
        AgentStatus.OnRoute => "En Route to Site",
        AgentStatus.OnSite => "On Site",
        AgentStatus.Available => "Available",
        AgentStatus.Break => "On Break",
        AgentStatus.Offline => "Offline",
        _ => "Unknown"
    };
}
