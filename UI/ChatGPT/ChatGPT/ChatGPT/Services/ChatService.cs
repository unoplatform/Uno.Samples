using ChatGPT.Business;
using Microsoft.Extensions.Configuration;

namespace ChatGPT.Services;
public class ChatService : IChatService
{
    private readonly IConfiguration _configuration;
    private string _apiKey;
    private OpenAIClient _openAIClient;

    public ChatService(
            //IConfiguration configuration
            )
    {
        //_configuration = configuration;

        //var apikey = configuration["ApiKey"];
        var apikey = "<insert api key here>";
        _openAIClient = new OpenAIClient(apikey);
    }

    public async ValueTask<ChatResponse> AskAsync(string request)
    {
        var response = await _openAIClient.ChatCompletions(request);

        return response;
    }
}
