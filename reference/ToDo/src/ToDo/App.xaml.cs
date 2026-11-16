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
                .UseSerialization()
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
            new ViewMap<AddTaskFlyout, AddTaskModel>(),
            new ViewMap<AddListFlyout, AddListModel>(),
            new ViewMap<ExpirationDateFlyout, ExpirationDateModel>(Data: new DataMap<PickedDate>()),
            new ViewMap<RenameListFlyout, RenameListModel>(),

            // Views
            new ViewMap<HomePage, HomeModel>(),
            new ViewMap<TaskSearchFlyout>(),
            new ViewMap<SearchPage, SearchModel>(),
            new ViewMap<SettingsFlyout, SettingsModel>(),
            new ViewMap<Shell, ShellModel>(),
            new ViewMap<WelcomePage, WelcomeModel>(),
            new DataViewMap<TaskListPage, TaskListModel, TaskList>(),
            new DataViewMap<TaskPage, TaskModel, ToDoTask>(),
            confirmDeleteListDialog,
            confirmDeleteTaskDialog,
            confirmSignOutDialog
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellModel>(), Nested: new RouteMap[]
            {
                new("Welcome", View: views.FindByViewModel<WelcomeModel>()),
                new("Home", View: views.FindByViewModel<HomeModel>()),
                new("TaskList", View: views.FindByViewModel<TaskListModel>(), Nested: new[]
                {
                    new RouteMap("ToDo", IsDefault:true),
                    new RouteMap("Completed")
                }),
                new("Task", View: views.FindByViewModel<TaskModel>(), DependsOn:"TaskList"),
                new("TaskSearch", View: views.FindByView<TaskSearchFlyout>(), Nested: new RouteMap[]
                {
                    new("Search", View: views.FindByViewModel<SearchModel>(), IsDefault: true)
                }),
                new("Settings", View: views.FindByViewModel<SettingsModel>()),
                new("AddTask", View: views.FindByViewModel<AddTaskModel>()),
                new("AddList", View: views.FindByViewModel<AddListModel>()),
                new("ExpirationDate", View: views.FindByViewModel<ExpirationDateModel>()),
                new("RenameList", View: views.FindByViewModel<RenameListModel>()),
                new(Dialog.ConfirmDeleteList, confirmDeleteListDialog),
                new(Dialog.ConfirmDeleteTask, confirmDeleteTaskDialog),
                new(Dialog.ConfirmSignOut, confirmSignOutDialog)
            })
        );
    }
}
