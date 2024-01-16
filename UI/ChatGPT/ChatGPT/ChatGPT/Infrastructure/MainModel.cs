using ChatGPT.Services;
using ChatGPT.Business;
using Uno.Extensions.Reactive.Commands;

namespace ChatGPT.Infrastructure;
public partial record MainModel
{
    private readonly IChatService _chatService;
    public MainModel(IChatService chatService)
    {
        _chatService = chatService;
    }

    public IState<string> UserMessage => State.Value(this, () => string.Empty);

    public IListState<Message> Messages => ListState<Message>.Empty(this);


    public async ValueTask SendMessage(string userMessage, CancellationToken ct)
    {
        if (userMessage is null or { Length: 0 })
        {
            return;
        }

        await Messages.AddAsync(new Message
        {
            Content = userMessage,
            Source = Source.User,
            Status = Status.Value
        }, ct);
        await UserMessage.Set(string.Empty, ct);

        await foreach (var message in _chatService.AskAsStream(userMessage).WithCancellation(ct))
        {
            await Messages.Update(
                messages =>
                {
                    var msgIndex = messages.IndexOf(message, KeyEqualityComparer.Find<Message>());
                    return msgIndex >= 0
                        ? messages.RemoveAt(msgIndex).Insert(msgIndex, message)
                        : messages.Add(message);
                },
                ct);
        }
    }
}
