using System;
using Microsoft.Extensions.Logging;
using Uno.Resizetizer;

namespace DestinationRolodex;

public partial class App : Application
{
    public App()
    {
        this.InitializeComponent();
    }

    protected Window? MainWindow { get; private set; }

#if DEBUG
    /// <summary>Parsed once per process; pages read it to pin deterministic start states for captures.</summary>
    internal static DestinationRolodex.Harness.DebugHarness HarnessOptions { get; private set; } = new();
#endif

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
#if DEBUG
        HarnessOptions = DestinationRolodex.Harness.DebugHarness.Parse(Environment.GetCommandLineArgs());
        if (HarnessOptions.ExportFixtures is { } exportDir)
        {
            _ = RunFixtureExportAsync(exportDir);
            return;
        }
#endif

        MainWindow = new Window();
#if DEBUG
        // Hot Design carries the App MCP connection on desktop; headless harness runs opt out.
        if (!HarnessOptions.IsActive)
        {
            MainWindow.UseStudio();
        }
#endif

        if (MainWindow.Content is not Frame rootFrame)
        {
            rootFrame = new Frame();
            MainWindow.Content = rootFrame;
            rootFrame.NavigationFailed += OnNavigationFailed;
        }

        if (rootFrame.Content == null)
        {
#if DEBUG
            rootFrame.Navigate(HarnessOptions.Spike ? typeof(Presentation.SpikePage) : typeof(MainPage), args.Arguments);
#else
            rootFrame.Navigate(typeof(MainPage), args.Arguments);
#endif
        }

        MainWindow.SetWindowIcon();
        MainWindow.Activate();
#if DEBUG
        if (HarnessOptions.Viewport is { } vp)
        {
            // Resize twice: the Skia surface can lag the frame on the first resize (runtime gotcha).
            // AppWindow.Resize is the outer size; add the Win32 frame so the client area approximates the viewport.
            var size = new Windows.Graphics.SizeInt32 { Width = vp.Width + 16, Height = vp.Height + 39 };
            MainWindow.AppWindow.Resize(new Windows.Graphics.SizeInt32 { Width = size.Width + 8, Height = size.Height + 8 });
            var timer = MainWindow.DispatcherQueue.CreateTimer();
            timer.Interval = TimeSpan.FromMilliseconds(600);
            timer.IsRepeating = false;
            timer.Tick += (_, _) => MainWindow.AppWindow.Resize(size);
            timer.Start();
        }
#endif
    }

#if DEBUG
    private static async Task RunFixtureExportAsync(string directory)
    {
        var exit = 0;
        try
        {
            var json = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Assets", "Data", "destinations.json"));
            var catalog = Services.DestinationRepository.Parse(json);
            var status = await DestinationRolodex.Harness.FixtureExport.RunAsync(directory, catalog, DestinationRolodex.Harness.DebugHarness.OpenLocalAsset);
            Console.Error.WriteLine($"fixture export: {status}");
            if (status != "ok")
            {
                exit = 2;
            }
        }
        catch (Exception ex)
        {
            await File.WriteAllTextAsync(Path.Combine(directory, "error.txt"), ex.ToString());
            exit = 1;
        }

        Environment.Exit(exit);
    }
#endif

    // xaml-lint: allow codebehind - template navigation failure handler
    void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
    }

    /// <summary>Configures global Uno Platform logging.</summary>
    public static void InitializeLogging()
    {
#if DEBUG
        var factory = LoggerFactory.Create(builder =>
        {
#if __WASM__
            builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
            builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
            builder.AddConsole();
#else
            builder.AddConsole();
#endif
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddFilter("Uno", LogLevel.Warning);
            builder.AddFilter("Windows", LogLevel.Warning);
            builder.AddFilter("Microsoft", LogLevel.Warning);
        });

        global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
        global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
#endif
    }
}
