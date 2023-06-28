using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Uno.Extensions.Reactive.Messaging;

namespace MessagingPeopleApp;

public interface IChatService
{
    ValueTask<IImmutableList<ChatMessage>> GetAllMessages(CancellationToken ct);

    ValueTask SendMessage(ChatMessage message, CancellationToken ct);
}

public class ChatService : IChatService
{
    public ChatService(IMessenger messenger)
    {
        Messenger = messenger;
    }

    protected IMessenger Messenger { get; }

    public async ValueTask<IImmutableList<ChatMessage>> GetAllMessages(CancellationToken ct)
    {
        await Task.Delay(500, ct);

        return _messages.ToImmutableList();
    }

    public async ValueTask SendMessage(ChatMessage message, CancellationToken ct)
    {
        await Task.Delay(500, ct);

        _messages.Add(message);

        Messenger.Send(new EntityMessage<ChatMessage>(EntityChange.Created, message));
    }


    private readonly List<ChatMessage> _messages = new();
}
