namespace XamlBrewerUnoApp;

public partial class App : Application
{
    protected Window? MainWindow { get; private set; }

    private Shell? shell;


    internal INavigation? Navigation => shell;

    internal UIElement? AppRoot => shell?.AppRoot;

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
#if NET6_0_OR_GREATER && WINDOWS && !HAS_UNO
        MainWindow = new Window();
#else
        MainWindow = Microsoft.UI.Xaml.Window.Current;
#endif

        shell = new Shell();
        MainWindow.Content = shell;
        MainWindow.Activate();

        MainWindow.Title = "XAML Brewer WinUI 3 SkiaSharp Sample";
#if WINDOWS
        var appWindow = MainWindow.AppWindow;
        appWindow.SetIcon("Assets/Beer.ico");
#endif
    }
}
