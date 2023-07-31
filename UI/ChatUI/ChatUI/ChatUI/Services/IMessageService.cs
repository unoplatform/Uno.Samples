using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using ChatUI.Entities;

namespace ChatUI.Services;

public interface IMessageService
{
	ValueTask<IImmutableList<Message>> GetMessages(CancellationToken ct);

	ValueTask AddMessage(Message newMessage, CancellationToken ct);
}
