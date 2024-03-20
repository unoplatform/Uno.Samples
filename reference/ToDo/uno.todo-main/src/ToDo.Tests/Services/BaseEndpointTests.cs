using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uno.Extensions;
using Uno.Extensions.Serialization;

namespace ToDo.Tests.Services;

internal class BaseEndpointTests<T> where T : notnull
{
	protected readonly T service;

	protected BaseEndpointTests()
	{
		var host = Host.CreateDefaultBuilder()
			.UseSerialization()
			.ConfigureAppConfiguration(builder =>
			{
				var appsettingsPrefix = new Dictionary<string, string>
						{
							{ "ITaskEndpoint:Url", "https://graph.microsoft.com/beta/me" },
							{ "ITaskEndpoint:UseNativeHandler","true" }
						};
				builder.AddInMemoryCollection(appsettingsPrefix);

			})
			.ConfigureServices((context, services) =>
			{
				services.AddEndpoints(context, (sp, settings) => settings.AuthorizationHeaderValueGetter = GetAccessToken);
			})
			.Build();

		service = host.Services.GetRequiredService<T>();
	}
	private Task<string> GetAccessToken()
	{
		return Task.FromResult("**AccessToken**");
	}
}
