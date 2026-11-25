namespace QuoteCraft.Presentation;

/// <summary>
/// A banner dialog with a message and up to two buttons (route <c>!Confirm</c>). An empty
/// <see cref="PrimaryText"/> leaves only the close button, for a notice.
/// </summary>
public record ConfirmRequest(string Glyph, string Title, string Message, string PrimaryText = "", string CloseText = "OK");

public partial record ConfirmModel(ConfirmRequest Request)
{
    public string Glyph => Request.Glyph;
    public string Title => Request.Title;
    public string Message => Request.Message;
    public string PrimaryText => Request.PrimaryText;
    public string CloseText => Request.CloseText;
}
