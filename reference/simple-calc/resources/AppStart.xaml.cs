using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Uno.Material;
using Uno.Toolkit.UI.Material;
using Uno.Toolkit.UI;
using Windows.ApplicationModel;
using Application = Microsoft.UI.Xaml.Application;
using ApplicationExecutionState = Windows.ApplicationModel.Activation.ApplicationExecutionState;
using Uno.Extensions;
using Uno.Extensions.Toolkit;

namespace SimpleCalculator;
public partial class AppStart
{
    public static Window? Window { get; private set; }
    public static IThemeService? ThemeService { get; private set; }

    public static void OnLaunched(Application app, LaunchActivatedEventArgs args)
    {
#if NET6_0_OR_GREATER && WINDOWS && !HAS_UNO
        Window = new Window();
        Window.Activate();
#else
        Window = Microsoft.UI.Xaml.Window.Current;
#endif
        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (Window.Content is not Frame rootFrame)
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = new Frame();
            rootFrame.NavigationFailed += OnNavigationFailed;
            if (args.UWPLaunchActivatedEventArgs.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                // TODO: Load state from previously suspended application
            }
            // Place the frame in the current Window
            Window.Content = rootFrame;
            ThemeService = Window.GetThemeService();
        }


        //Comment to test on windows as this conditional crashes the app       
        //#if !(NETSTANDARD && WINDOWS)
#if !(NET6_0_OR_GREATER && WINDOWS)
        if (args.UWPLaunchActivatedEventArgs.PrelaunchActivated == false)
#endif
        {
            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), args.Arguments);
            }
            // Ensure the current window is active
            Window.Activate();
        }
    }
    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    private static void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
    }
}