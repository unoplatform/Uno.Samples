using ChatGPT.Business;

namespace ChatGPT.Services;
public interface IChatService
{
    ValueTask<ChatResponse> AskAsync(string request);
}
