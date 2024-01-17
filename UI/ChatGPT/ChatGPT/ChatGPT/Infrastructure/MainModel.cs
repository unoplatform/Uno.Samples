using ChatGPT.Services;
using ChatGPT.Business;
using Windows.Networking.NetworkOperators;

namespace ChatGPT.Infrastructure;
public partial record MainModel
{
    private readonly IChatService _chatService;
    public MainModel(IChatService chatService)
    {
        _chatService = chatService;
    }

    public IState<string> Prompt => State.Value(this, () => string.Empty);

    public IListState<Message> Messages => ListState<Message>.Empty(this);

    public async ValueTask SendMessageSimple(string prompt, CancellationToken ct)
    {
        if (prompt is null or { Length: 0 })
        {
            return;
        }

        await Messages.AddAsync(new Message
        {
            Content = prompt,
            Source = Source.User,
            Status = Status.Value
        }, ct);
        await Prompt.Set(string.Empty, ct);

        var response = await _chatService.AskAsync(prompt);

        await Messages.AddAsync(response, ct);
    }

    public async ValueTask SendMessage(
                                        //string prompt,
                                        CancellationToken ct)
    {
        var prompt = await Prompt;

        if (prompt is null or { Length: 0 })
        {
            return;
        }

        await Messages.AddAsync(new Message
        {
            Content = prompt,
            Source = Source.User,
            Status = Status.Value
        }, ct);

        await Prompt.Set(string.Empty, ct);

        await foreach (var message in _chatService.AskAsStream(prompt).WithCancellation(ct))
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
