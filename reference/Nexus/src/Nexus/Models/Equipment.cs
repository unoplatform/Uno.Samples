namespace Nexus.Models;

public partial record Equipment(
    string Id,
    string Name,
    double HealthPercent,
    DateTime NextService,
    int RuntimeHours
);
