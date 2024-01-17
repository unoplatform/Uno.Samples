using ChatGPT.Business;

namespace ChatGPT.Services;
public interface IChatService
{
	ValueTask<ChatResponse> AskAsync(string prompt);

	IAsyncEnumerable<ChatResponse> AskAsStream(string prompt, CancellationToken ct = default);
}
