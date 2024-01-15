namespace ChatGPT.Business;
public class ChatResponse
{
    public List<string>? Message { get; set; }
    public bool Error { get; set; }
    public List<string>? ErrorMessage { get; set; }
}
