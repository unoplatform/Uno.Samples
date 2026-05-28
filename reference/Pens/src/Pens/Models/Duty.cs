namespace Pens.Models;

public partial record Duty(
    DutyType Type,
    string Role,
    string Name,
    int? PlayerId,
    bool IsManuallyAssigned);

public enum DutyType
{
    Ice,
    Beer,
    Cooler,
    Food
}
