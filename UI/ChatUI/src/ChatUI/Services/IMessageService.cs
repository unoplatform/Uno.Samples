using ChatUI.Models;

namespace ChatUI.Services;

public interface IMessageService
{
    ValueTask<IImmutableList<Message>> GetMessages(CancellationToken ct);

    ValueTask AddMessage(Message newMessage, CancellationToken ct);
}
