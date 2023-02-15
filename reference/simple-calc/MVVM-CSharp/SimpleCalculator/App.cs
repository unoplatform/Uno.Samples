using Uno.Extensions;
using Uno.Extensions.Toolkit;
using Application = Microsoft.UI.Xaml.Application;

namespace SimpleCalculator;

public class App : Application
{
    public static Window? _window;
    public static IThemeService? ThemeService { get; private set; }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
#if NET6_0_OR_GREATER && WINDOWS && !HAS_UNO
		_window = new Window();
#else
        _window = Microsoft.UI.Xaml.Window.Current;
#endif

        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (_window.Content is not Frame rootFrame)
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = new Frame();

            // Place the frame in the current Window
            _window.Content = rootFrame;
            ThemeService = _window.GetThemeService();
        }

        if (rootFrame.Content == null)
        {
            // When the navigation stack isn't restored navigate to the first page,
            // configuring the new page by passing required information as a navigation
            // parameter
            rootFrame.Navigate(typeof(MainPage), args.Arguments);
        }

        // Ensure the current window is active
        _window.Activate();
    }
}