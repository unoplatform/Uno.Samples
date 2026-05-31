namespace Nexus.Models;

public enum UserRole
{
    Admin,
    Supervisor,
    Operator,
    Technician
}

public enum UserStatus
{
    Active,
    Inactive
}

public partial record User(
    string Id,
    string Name,
    string Email,
    UserRole Role,
    DateTime LastAccess,
    UserStatus Status
);
