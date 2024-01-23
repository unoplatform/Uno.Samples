using OpenAI;

namespace ChatGPT.Business.Models;

public class ChatAiOptions: OpenAiOptions
{
	public ChatAiOptions(IOptions<AppConfig> appConfig)
	{
		this.ApiKey = appConfig.Value.ApiKey ?? throw new InvalidOperationException("You must define an OpenAI API key in appsettings.json file.");
	}
}