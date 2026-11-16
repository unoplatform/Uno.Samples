using ChatUI.Services;

namespace ChatUI.Models;

public partial record MessageModel(IMessageService MessageService)
{
    public IListState<Message> Messages => ListState
        .Async(this, MessageService.GetMessages);

    public IState<Message> NewMessage => State<Message>.Value(this, () => Message.Empty);

    public async ValueTask AddMessage(Message newMessage, CancellationToken ct)
    {
        await MessageService.AddMessage(newMessage, ct);

        await Messages.InsertAsync(newMessage, ct);

        await NewMessage.Update(old => Message.Empty, ct);
    }
}