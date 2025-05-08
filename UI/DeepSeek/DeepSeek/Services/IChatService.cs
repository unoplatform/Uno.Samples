using DeepSeek.Business;

namespace DeepSeek.Services;

public interface IChatService
{
    ValueTask<ChatResponse> AskAsync(ChatRequest request, CancellationToken ct = default);
}
