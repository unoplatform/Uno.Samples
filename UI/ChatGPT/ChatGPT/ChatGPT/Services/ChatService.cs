using ChatGPT.Business;
using OpenAI.Managers;
using OpenAI;
using System.Runtime.CompilerServices;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using System.Text;
using OpenAI.ObjectModels.ResponseModels;

namespace ChatGPT.Services;
public class ChatService : IChatService
{
    private OpenAIService _client;

    public ChatService(IOptions<AppConfig> appConfig)
    {
        var apiKey = appConfig.Value.ApiKey;

        _client = new OpenAIService(
            new OpenAiOptions()
            {
                ApiKey = apiKey
            });
    }

    public async ValueTask<Message> AskAsync(string prompt)
    {
        var result = await _client.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest()
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromUser(prompt)
            },
            Model = Models.Gpt_3_5_Turbo
        });

        if (result.Successful)
        {
            var _result = result.Choices.Select(choice => choice.Message.Content);
            return new Message { Content = string.Join("", _result), Source = Source.AI, Status = Status.Value };
        }
        else
        {
            if (result.Error == null)
            {
                throw new Exception("Unknown Error");
            }

            return new Message
            {
                Status = Status.Error,
                Content = $"{result.Error.Message ?? "Unknown Error Message"} {result.Error.Code ?? "Unknown Error Code"}",
                Source = Source.AI
            };
        }
    }

    public async IAsyncEnumerable<Message> AskAsStream(string prompt, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var message = new Message { Id = Guid.NewGuid(), Status = Status.Loading, Source = Source.AI, Content = "..." };
        var content = new StringBuilder();

        yield return message;

        var request = new ChatCompletionCreateRequest()
        {
            Messages = new List<ChatMessage> { ChatMessage.FromUser(prompt) },
            Model = Models.Gpt_3_5_Turbo
        };

        IAsyncEnumerator<ChatCompletionCreateResponse>? responseStream = default;
        while (message is { Status: Status.Loading })
        {
            try
            {
                responseStream ??= _client.ChatCompletion.CreateCompletionAsStream(request).GetAsyncEnumerator(ct);
                if (await responseStream.MoveNextAsync())
                {
                    foreach (var choice in responseStream.Current.Choices)
                    {
                        content.Append(choice.Message.Content);
                    }
                    message = message with { Content = content.ToString() };
                }
                else
                {
                    message = message with { Status = Status.Value };
                }
            }
            catch (Exception)
            {
                message = message with { Status = Status.Error };
            }

            yield return message;
        }
    }
}
