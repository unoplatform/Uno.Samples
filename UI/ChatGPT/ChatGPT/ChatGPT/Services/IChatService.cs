using System.Runtime.CompilerServices;
using ChatGPT.Business;

namespace ChatGPT.Services;
public interface IChatService
{
    ValueTask<ChatResponse> AskAsync(string request);

    IAsyncEnumerable<Message> AskAsStream(string prompt, CancellationToken ct = default);
}
