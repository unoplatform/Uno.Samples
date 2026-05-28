namespace Nexus.Models;

public enum AlertType
{
    Info,
    Warning,
    Critical,
    Success
}

public partial record Alert(
    DateTime Timestamp,
    AlertType Type,
    string Message
);
