using DeepSeek.Business;

namespace DeepSeek.Presentation;

public partial record Message(Guid Id, string Source, Status Status, string? Content)
{
    private readonly string _errorMessage = "No response from DeepSeek.";
    public static Message CreateLoading()
        => new Message(Guid.NewGuid(), Presentation.Source.Assistant, Status.Loading, null);

    public Message(string userMessage)
        : this(Guid.NewGuid(), Presentation.Source.User, Status.Value, userMessage)
    {
    }

    public Message With(ChatResponse response)
    {
        var content = response.Choices?.FirstOrDefault()?.Message.Content ?? _errorMessage;
        var status = content == _errorMessage ? Status.Error : Status.Value;

        return this with
        {
            Content = content,
            Status = status
        };
    }
}
