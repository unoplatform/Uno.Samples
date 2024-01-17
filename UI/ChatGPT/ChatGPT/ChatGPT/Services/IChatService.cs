namespace ChatGPT.Services;
public interface IChatService
{
    ValueTask<Message> AskAsync(string prompt);

    IAsyncEnumerable<Message> AskAsStream(string prompt, CancellationToken ct = default);
}
