using ChatGPT.Business;

namespace ChatGPT.Services;
public interface IChatService
{
	ValueTask<ChatResponse> AskAsync(IImmutableList<ChatEntry> history, CancellationToken ct = default);

	IAsyncEnumerable<ChatResponse> AskAsStream(IImmutableList<ChatEntry> history, CancellationToken ct = default);
}
