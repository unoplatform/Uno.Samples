using Uno.Resizetizer;

namespace ToDo;

public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    public Window? MainWindow { get; private set; }
    protected IHost? Host { get; private set; }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            // Add navigation support for toolkit controls such as TabBar and NavigationView
            .UseToolkitNavigation()
            .Configure(host => host
#if DEBUG
                // Switch to Development environment when running in DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseLogging(configure: (context, logBuilder) =>
                {
                    // Configure log levels for different categories of logging
                    logBuilder
                        .SetMinimumLevel(
                            context.HostingEnvironment.IsDevelopment() ?
                                Microsoft.Extensions.Logging.LogLevel.Information :
                                Microsoft.Extensions.Logging.LogLevel.Warning)

                        // Default filters for core Uno Platform namespaces
                        .CoreLogLevel(Microsoft.Extensions.Logging.LogLevel.Warning);

                    // Uno Platform namespace filter groups
                    // Uncomment individual methods to see more detailed logging
                    //// Generic Xaml events
                    //logBuilder.XamlLogLevel(LogLevel.Debug);
                    //// Layout specific messages
                    //logBuilder.XamlLayoutLogLevel(LogLevel.Debug);
                    //// Storage messages
                    //logBuilder.StorageLogLevel(LogLevel.Debug);
                    //// Binding related messages
                    //logBuilder.XamlBindingLogLevel(LogLevel.Debug);
                    //// Binder memory references tracking
                    //logBuilder.BinderMemoryReferenceLogLevel(LogLevel.Debug);
                    //// DevServer and HotReload related
                    //logBuilder.HotReloadCoreLogLevel(LogLevel.Information);
                    //// Debug JS interop
                    //logBuilder.WebAssemblyLogLevel(LogLevel.Debug);

                }, enableUnoLogging: true)
                .UseConfiguration(configure: configBuilder =>
                    configBuilder
                        // Load configuration information from appconfig.json
                        .EmbeddedSource<App>()
                        .EmbeddedSource<App>("platform")
                        // Load OAuth configuration
                        .Section<Auth>()
                        // Load Mock configuration
                        .Section<Mock>()
                        // Enable app settings
                        .Section<ToDoApp>()
                )
                // Enable localization (see appsettings.json for supported languages)
                .UseLocalization()
                // Register Json serializers (ISerializer and ISerializer)
                .UseSerialization(services =>
                {
                    services
                        .AddJsonTypeInfo(AuthJsonContext.Default.Auth)
                        .AddSingleton(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                })
                .ConfigureServices(
                    (context, services) => {
                        var section = context.Configuration.GetSection(nameof(Mock));
                        var useMocks = bool.TryParse(section[nameof(Mock.IsEnabled)], out var isMocked) ? isMocked : false;
#if USE_MOCKS
                        // This is required for UI Testing where USE_MOCKS is enabled
                        useMocks=true;;
#endif
                        services
                            .AddEndpoints(context, useMocks: useMocks)
                            .AddServices(useMocks: useMocks);
                    })
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
                .UseThemeSwitching()
            );
        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio();
#endif
        MainWindow.SetWindowIcon();

        Host = await builder.NavigateAsync<Shell>();
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        LocalizableMessageDialogViewMap BuildDialogViewMap(string section, bool delayUserInput, int defaultButtonIndex, params (object Id, string labelKeyPath)[] buttons)
        {
            return new LocalizableMessageDialogViewMap
            (
                Content: localizer => localizer![ResourceKey(ResourceKeys.DialogContent)],
                Title: localizer => localizer![ResourceKey(ResourceKeys.DialogTitle)],
                DelayUserInput: delayUserInput,
                DefaultButtonIndex: defaultButtonIndex,
                Buttons: buttons
                    .Select(x => new LocalizableDialogAction(LabelProvider: localizer => localizer![ResourceKey(x.labelKeyPath)], Id: x.Id))
                    .ToArray()
            );
            string ResourceKey(string keyPath)
            {
                // map absolute/relative path accordingly
                return keyPath.StartsWith("./") ? keyPath.Substring(2) : $"Dialog_{section}_{keyPath}";
            }
        }

        var deleteButton = (DialogResults.Affirmative, ResourceKeys.DeleteButton);
        var cancelButton = (DialogResults.Negative, ResourceKeys.CancelButton);
        var confirmDeleteListDialog = BuildDialogViewMap(Dialog.ConfirmDeleteList, true, 0, deleteButton, cancelButton);
        var confirmDeleteTaskDialog = BuildDialogViewMap(Dialog.ConfirmDeleteTask, true, 0, deleteButton, cancelButton);
        var confirmSignOutDialog = BuildDialogViewMap(Dialog.ConfirmSignOut, true, 0, (DialogResults.Affirmative, ResourceKeys.SignOutButton), cancelButton);

        views.Register(
            // Dialogs and Flyouts
            new ViewMap<AddTaskFlyout, AddTaskViewModel>(),
            new ViewMap<AddListFlyout, AddListViewModel>(),
            new ViewMap<ExpirationDateFlyout, ExpirationDateViewModel>(Data: new DataMap<PickedDate>()),
            new ViewMap<RenameListFlyout, RenameListViewModel>(),

            // Views
            new ViewMap<HomePage, HomeViewModel>(),
            new ViewMap<TaskSearchFlyout>(),
            new ViewMap<SearchPage, SearchViewModel>(),
            new ViewMap<SettingsFlyout, SettingsViewModel>(),
            new ViewMap<Shell, ShellViewModel>(),
            new ViewMap<WelcomePage, WelcomeViewModel>(),
            new DataViewMap<TaskListPage, TaskListViewModel, TaskList>(),
            new DataViewMap<TaskPage, TaskViewModel, ToDoTask>(),
            confirmDeleteListDialog,
            confirmDeleteTaskDialog,
            confirmSignOutDialog
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellViewModel>(), Nested: new RouteMap[]
            {
                new("Welcome", View: views.FindByViewModel<WelcomeViewModel>()),
                new("Home", View: views.FindByViewModel<HomeViewModel>()),
                new("TaskList", View: views.FindByViewModel<TaskListViewModel>(), Nested: new[]
                {
                    new RouteMap("ToDo", IsDefault:true),
                    new RouteMap("Completed")
                }),
                new("Task", View: views.FindByViewModel<TaskViewModel>(), DependsOn:"TaskList"),
                new("TaskSearch", View: views.FindByView<TaskSearchFlyout>(), Nested: new RouteMap[]
                {
                    new("Search", View: views.FindByViewModel<SearchViewModel>(), IsDefault: true)
                }),
                new("Settings", View: views.FindByViewModel<SettingsViewModel>()),
                new("AddTask", View: views.FindByViewModel<AddTaskViewModel>()),
                new("AddList", View: views.FindByViewModel<AddListViewModel>()),
                new("ExpirationDate", View: views.FindByViewModel<ExpirationDateViewModel>()),
                new("RenameList", View: views.FindByViewModel<RenameListViewModel>()),
                new(Dialog.ConfirmDeleteList, confirmDeleteListDialog),
                new(Dialog.ConfirmDeleteTask, confirmDeleteTaskDialog),
                new(Dialog.ConfirmSignOut, confirmSignOutDialog)
            })
        );
    }
}
