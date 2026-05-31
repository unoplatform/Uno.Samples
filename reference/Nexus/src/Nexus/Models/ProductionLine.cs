namespace Nexus.Models;

public enum LineStatus
{
    Active,
    Standby,
    Maintenance
}

public partial record ProductionLine(
    string Id,
    string Name,
    LineStatus Status,
    double OutputPercent,
    double Temperature,
    double Pressure
);
