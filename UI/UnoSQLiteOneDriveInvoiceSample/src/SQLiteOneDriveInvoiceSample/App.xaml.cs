#if __IOS__

using Foundation;
using UIKit;

#endif
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;



using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using System.Threading.Tasks;
using SQLiteOneDriveInvoiceSample.Views;

namespace SQLiteOneDriveInvoiceSample
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private Window _window;

        public static object AuthenticationUiParent { get; set; }
        public string iOSKeychainSecurityGroup { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeLogging();
#if __IOS__ || __ANDROID__
    Uno.UI.FeatureConfiguration.Style.ConfigureNativeFrameNavigation();
#endif
            this.InitializeComponent();

#if HAS_UNO || NETFX_CORE
            this.Suspending += OnSuspending;
#endif
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

#if NET6_0_OR_GREATER && WINDOWS
            _window = new Window();
            _window.Activate();
#else
            _window = Microsoft.UI.Xaml.Window.Current;
#endif

            var rootFrame = _window.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (args.UWPLaunchActivatedEventArgs.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                _window.Content = rootFrame;
            }

#if !(NET6_0_OR_GREATER && WINDOWS)
            if (args.UWPLaunchActivatedEventArgs.PrelaunchActivated == false)
#endif
            {
                if (rootFrame.Content == null)
                {
#if __WASM__
                    _ = Windows.Storage.StorageFolder.GetFolderFromPathAsync(Windows.Storage.ApplicationData.Current.LocalFolder.Path);
#endif
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), args.Arguments);
                }
                // Ensure the current window is active
                _window.Activate();
            }
            //ConfigureNavigation();
        }

//        private void ConfigureNavigation()
//        {
//            var frame = (Microsoft.UI.Xaml.Controls.Frame)_window.Content;
//            var manager = Windows.UI.Core.SystemNavigationManager.GetForCurrentView();


//#if WINDOWS_UWP || __WASM__
//            // Toggle the visibility of back button based on if the frame can navigate back.
//            // Setting it to visible has the follow effect on the platform:
//            // - uwp: add a `<-` back button on the title bar
//            // - wasm: add a dummy entry in the browser back stack
//            frame.Navigated += (s, e) => manager.AppViewBackButtonVisibility = frame.CanGoBack
//                ? Windows.UI.Core.AppViewBackButtonVisibility.Visible
//                : Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;
//#endif

//#if WINDOWS_UWP || __ANDROID__ || __WASM__
//            // On some platforms, the back navigation request needs to be hooked up to the back navigation of the Frame.
//            // These requests can come from:
//            // - uwp: title bar back button
//            // - droid: CommandBar back button, os back button/gesture
//            // - wasm: browser back button
//            manager.BackRequested += (s, e) =>
//            {
//                if (frame.CanGoBack)
//                {
//                    frame.GoBack();

//                    e.Handled = true;
//                }
//            };
//#endif
//        }


        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        /// <summary>
        /// Configures global Uno Platform logging
        /// </summary>
        private static void InitializeLogging()
        {
            var factory = LoggerFactory.Create(builder =>
            {
#if __WASM__
                builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
                builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
                builder.AddDebug();
#else
                builder.AddConsole();
#endif

                // Exclude logs below this level
                builder.SetMinimumLevel(LogLevel.Information);

                // Default filters for Uno Platform namespaces
                builder.AddFilter("Uno", LogLevel.Warning);
                builder.AddFilter("Windows", LogLevel.Warning);
                builder.AddFilter("Microsoft", LogLevel.Warning);

                // Generic Xaml events
                // builder.AddFilter("Windows.UI.Xaml", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.UIElement", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.FrameworkElement", LogLevel.Trace );

                // Layouter specific messages
                // builder.AddFilter("Windows.UI.Xaml.Controls", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Controls.Panel", LogLevel.Debug );

                // builder.AddFilter("Windows.Storage", LogLevel.Debug );

                // Binding related messages
                // builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );

                // Binder memory references tracking
                // builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

                // RemoteControl and HotReload related
                // builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

                // Debug JS interop
                // builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
            });

            global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
            global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
        }

#if __IOS__

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // Specify the Keychain access group
            iOSKeychainSecurityGroup = NSBundle.MainBundle.BundleIdentifier;            

            return base.FinishedLaunching(app, options);
        }

        // Handling redirect URL
        // See: https://docs.microsoft.com/azure/active-directory/develop/msal-net-xamarin-ios-considerations
        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {   
            return true;
        }

#endif

    }
}
