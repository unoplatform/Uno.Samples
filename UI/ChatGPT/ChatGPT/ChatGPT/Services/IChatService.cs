using ChatGPT.Business;

namespace ChatGPT.Services;
public interface IChatService
{
	ValueTask<ChatResponse> AskAsync(ChatRequest request, CancellationToken ct = default);

	IAsyncEnumerable<ChatResponse> AskAsStream(ChatRequest request, CancellationToken ct = default);
}
