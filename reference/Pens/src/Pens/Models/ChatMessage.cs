namespace Pens.Models;

public partial record ChatMessage(
    string Initials,
    string Sender,
    string Time,
    string Message);
