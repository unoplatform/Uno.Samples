using ChatGPT.Services;
using ChatGPT.Business;
using Windows.Networking.NetworkOperators;
using Uno.Extensions.Reactive;

namespace ChatGPT.Infrastructure;
public partial record MainModel
{
    private readonly IChatService _chatService;
    public MainModel(IChatService chatService) 
    {
        _chatService = chatService;
    }

    public IState<string> UserMessage => State.Value(this, () => String.Empty);

    public IListState<Message> Messages => ListState<Message>.Empty(this);


    public async ValueTask SendMessage(CancellationToken ct)
    {
        var currentMessage = await UserMessage;

        if (currentMessage is not null)
        {
            var userMessage = new Message
            {
                Content = currentMessage,
                Source = Source.User,
                Status = Status.Value
            };

            await AddMessateToList(userMessage, ct);

            await UserMessage.Update(_ => string.Empty, ct);

            var response = await _chatService.AskAsync(currentMessage);

            if (!response.Error) 
            {
                var chatMessage = new Message
                {
                    Content = string.Join(" ", response.Message),
                    Source = Source.AI,
                    Status = Status.Value
                };

                await AddMessateToList(chatMessage, ct);
            }
            else
            {
                var errorMEssage = new Message
                {
                    Source = Source.AI,
                    Status = Status.Error
                };

                await AddMessateToList(errorMEssage, ct);
            }
        }
    }

    private async ValueTask AddMessateToList(Message message, CancellationToken ct)
    {
        await Messages.InsertAsync(message, ct);
    }
}
