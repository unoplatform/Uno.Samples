using IAuthenticationService = ToDo.Business.IAuthenticationService;
using MockAuthenticationService = ToDo.Business.Mock.MockAuthenticationService;

namespace ToDo;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddEndpoints(
		this IServiceCollection services,
		HostBuilderContext context,
		Action<IServiceProvider, RefitSettings>? settingsBuilder = null,
		bool useMocks=false)
	{
        _ = services
            .AddNativeHandler(context)
            .AddContentSerializer(context)
            .AddRefitClient<ITaskEndpoint>(context, name: nameof(ITaskEndpoint))
            .AddRefitClient<ITaskListEndpoint>(context, name: nameof(ITaskListEndpoint))
			.AddRefitClient<IUserProfilePictureEndpoint>(context, name: nameof(IUserProfilePictureEndpoint));

		if (useMocks)
		{
			_ = services.AddSingleton<ITaskListEndpoint, ToDo.Data.Mock.MockTaskListEndpoint>()
				.AddSingleton<IUserProfilePictureEndpoint, ToDo.Data.Mock.MockUserProfilePictureEndpoint>()
				.AddSingleton<ITaskEndpoint, ToDo.Data.Mock.MockTaskEndpoint>();
		}
		return services;
	}

	public static IServiceCollection AddServices(
		this IServiceCollection services,
		bool useMocks = false)
	{
		_ = services
		   .AddSingleton<ITaskService, TaskService>()
		   .AddSingleton<ITaskListService, TaskListService>()
		   .AddSingleton<IUserProfilePictureService, UserProfilePictureService>()
		   .AddSingleton<IAuthenticationService, AuthenticationService>()
		   .AddSingleton<IAuthenticationTokenProvider>(sp => sp.GetRequiredService<IAuthenticationService>())
		   .AddSingleton<IMessenger, WeakReferenceMessenger>();

		if (useMocks)
		{
			// Comment out the USE_MOCKS definition (top of this file) to prevent using mocks in development
			_ = services.AddSingleton<IAuthenticationService, MockAuthenticationService>();
		}
		return services;
	}
}
