using DeepSeek.Business;
using Windows.ApplicationModel.Chat;

namespace DeepSeek.Services;
public class ChatService : IChatService
{
    private const string systemPrompt = "You are Uno DeepSeek Sample, a helpful assistant helping users to learn more about how to develop using Uno Platform.";

    private IDeepSeekApiClient _deepSeekClient;

    public ChatService(IDeepSeekApiClient deepSeekClient)
    {
        _deepSeekClient = deepSeekClient;
    }

    public async ValueTask<ChatResponse> AskAsync(ChatRequest chatRequest, CancellationToken ct = default)
    {
        var completionRequest = ToCompletionRequest(chatRequest);

        var response = await _deepSeekClient.AskAsync(ct, completionRequest);

        if (response.IsSuccessStatusCode && response.Content is not null)
        {
            var content = response.Content;
            return content;
        }

        return await Task.FromException<ChatResponse>(response.Error);
    }

    private ChatRequest ToCompletionRequest(ChatRequest request)
        => request with
           {
                Messages = request.Messages
                     .Prepend(new ChatEntry(systemPrompt, "system"))
                     .ToImmutableList()
           };
}
