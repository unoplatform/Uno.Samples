using InTheHand.DependencyInjection;
using System.Diagnostics;
using Windows.UI.Core;
using UnoCakesMobile.Services;
using UnoCakesMobile.ViewModels;
using UnoCakesMobile.Views;

namespace UnoCakesMobile
{
    public class App : Application
    {
        protected Window? MainWindow { get; private set; }

        public App()
        {
#if __IOS__ || __ANDROID__
           Uno.UI.FeatureConfiguration.Style.ConfigureNativeFrameNavigation();
#endif
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
#if NET6_0_OR_GREATER && WINDOWS && !HAS_UNO
		MainWindow = new Window();
#else
            MainWindow = Microsoft.UI.Xaml.Window.Current;
#endif

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (MainWindow.Content is not Frame rootFrame)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // Place the frame in the current Window
                MainWindow.Content = rootFrame;

                // Register service for frame navigation (so we can navigate from View Models)
                DependencyService.RegisterSingleton<INavigationService>(new NavigationService(rootFrame));
                DependencyService.Register<CakeListViewModel>();

                rootFrame.NavigationFailed += OnNavigationFailed;
                rootFrame.Navigated += RootFrame_Navigated;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(StartPage), args.Arguments);
            }

            // Ensure the current window is active
            MainWindow.Activate();

            ConfigureNavigation();
        }

        private void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            Debug.WriteLine($"{e.NavigationMode} {e.SourcePageType} {e.Parameter} {e.NavigationTransitionInfo}");
        }

        private void ConfigureNavigation()
        {
#if __ANDROID__ || __WASM__
            var frame = (Frame)MainWindow.Content;
            var manager = Windows.UI.Core.SystemNavigationManager.GetForCurrentView();

            // Toggle the visibility of back button based on if the frame can navigate back.
            // Setting it to visible has the follow effect on the platform:
            // - uwp: show a `<-` back button on the title bar
            // - wasm: add a dummy entry in the browser back stack
            frame.Navigated += (s, e) => manager.AppViewBackButtonVisibility = frame.CanGoBack
				? Windows.UI.Core.AppViewBackButtonVisibility.Visible
				: Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;

			// On some platforms, the back navigation request needs to be hooked up to the back navigation of the Frame.
			// These requests can come from:
			// - uwp: title bar back button
			// - droid: CommandBar back button, os back button/gesture
			// - wasm: browser back button
			manager.BackRequested += (s, e) =>
			{
				if (frame.CanGoBack)
				{
					frame.GoBack();

					e.Handled = true;
				}
			};
#endif
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
        }
    }
}