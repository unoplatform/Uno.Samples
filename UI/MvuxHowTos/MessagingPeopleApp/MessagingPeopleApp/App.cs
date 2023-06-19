using CommunityToolkit.Mvvm.Messaging;

namespace MessagingPeopleApp
{
    public class App : Application
    {
        protected Window? MainWindow { get; private set; }
        protected IHost? Host { get; private set; }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var builder = this.CreateBuilder(args)
                .Configure(host => host
#if DEBUG
				// Switch to Development environment when running in DEBUG
				.UseEnvironment(Environments.Development)
#endif
                    .ConfigureServices((context, services) =>
                    {
                        services.AddSingleton<IPeopleService, PeopleService>();
                        services.AddSingleton<IMessenger, WeakReferenceMessenger>();
                        services.AddTransient<BindablePeopleModel>();
                        services.AddTransient<MainPage>();
                    })
                );
            MainWindow = builder.Window;

            Host = builder.Build();

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            MainWindow.Content = Host.Services.GetRequiredService<MainPage>();

            // Ensure the current window is active
            MainWindow.Activate();
        }
    }
}