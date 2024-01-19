using OpenAI;

namespace ChatGPT.Business.Models;

public class ChatAiOptions: OpenAiOptions
{
	public ChatAiOptions(IOptions<AppConfig> appConfig)
	{
		this.ApiKey = appConfig.Value.ApiKey ?? throw new InvalidOperationException("You must define an API key in application settings file.");
	}
}