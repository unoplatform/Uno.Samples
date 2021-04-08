using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WCTDataTreeTabSample
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public sealed partial class App : Application
	{
		internal static Frame NavigationFrame { get; set; }
		internal static Microsoft.UI.Xaml.Controls.NavigationView NavigationView { get; set; }

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			ConfigureFilters(global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory);

			this.InitializeComponent();
			this.Suspending += OnSuspending;
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="e">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs e)
		{
#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached)
			{
				// this.DebugSettings.EnableFrameRateCounter = true;
			}
#endif

			InitializeMaterialStyles();

#if NET5_0 && WINDOWS
			var window = new Window();
			window.Activate();
#else
			var window = Windows.UI.Xaml.Window.Current;
#endif

			var rootFrame = window.Content as Frame;

			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active
			if (rootFrame == null)
			{
				// Create a Frame to act as the navigation context and navigate to the first page
				rootFrame = new Frame();

				rootFrame.NavigationFailed += OnNavigationFailed;

				if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
				{
					//TODO: Load state from previously suspended application
				}

				// Place the frame in the current Window
				window.Content = rootFrame;
			}

#if !(NET5_0 && WINDOWS)
			if (e.PrelaunchActivated == false)
#endif
			{
				if (rootFrame.Content == null)
				{
					// When the navigation stack isn't restored navigate to the first page,
					// configuring the new page by passing required information as a navigation
					// parameter
					rootFrame.Navigate(typeof(Shell), e.Arguments);
				}
				// Ensure the current window is active
				window.Activate();
			}
		}

		/// <summary>
		/// Invoked when Navigation to a certain page fails
		/// </summary>
		/// <param name="sender">The Frame which failed navigation</param>
		/// <param name="e">Details about the navigation failure</param>
		void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			throw new Exception($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
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
			//TODO: Save application state and stop any background activity
			deferral.Complete();
		}

		internal static void NavigateTo(Type targetPageType)
		{
			App.NavigationFrame.Navigate(targetPageType);

			switch (targetPageType.Name)
			{
				case nameof(TreeViewPage):
					App.NavigationView.Header = "TreeView Sample";
					break;

				case nameof(MountainsPage):
					App.NavigationView.Header = "DataGrid Sample (Mountains)";
					break;

				case nameof(LocationsPage):
					App.NavigationView.Header = "DataGrid Sample (Locations)";
					break;

				case nameof(TabViewPage):
					App.NavigationView.Header = "TabView Sample";
					break;

				case nameof(MasterDetailsPage):
					App.NavigationView.Header = "Master-Details Sample";
					break;

				case nameof(TwoPaneViewPage):
					App.NavigationView.Header = "TwoPaneView Sample";
					break;

				case nameof(ExpanderPage):
					App.NavigationView.Header = "Expander Sample";
					break;
			}

			App.NavigationView.SelectedItem =
				App.NavigationView
					.MenuItems
					.Cast<Microsoft.UI.Xaml.Controls.NavigationViewItem>()
					.Where(item => item.Tag != null)
					.FirstOrDefault(item => item.Tag.ToString().Equals(targetPageType.Name, StringComparison.OrdinalIgnoreCase));
		}


		/// <summary>
		/// Configures global logging
		/// </summary>
		/// <param name="factory"></param>
		static void ConfigureFilters(ILoggerFactory factory)
		{
			factory
				.WithFilter(new FilterLoggerSettings
					{
						{ "Uno", LogLevel.Warning },
						{ "Windows", LogLevel.Warning },

						// Debug JS interop
						// { "Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug },

						// Generic Xaml events
						// { "Windows.UI.Xaml", LogLevel.Debug },
						// { "Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug },
						// { "Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.UIElement", LogLevel.Debug },

						// Layouter specific messages
						// { "Windows.UI.Xaml.Controls", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.Panel", LogLevel.Debug },
						// { "Windows.Storage", LogLevel.Debug },

						// Binding related messages
						// { "Windows.UI.Xaml.Data", LogLevel.Debug },

						// DependencyObject memory references tracking
						// { "ReferenceHolder", LogLevel.Debug },

						// ListView-related messages
						// { "Windows.UI.Xaml.Controls.ListViewBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.ListView", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.GridView", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.VirtualizingPanelLayout", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.NativeListViewBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.ListViewBaseSource", LogLevel.Debug }, //iOS
						// { "Windows.UI.Xaml.Controls.ListViewBaseInternalContainer", LogLevel.Debug }, //iOS
						// { "Windows.UI.Xaml.Controls.NativeListViewBaseAdapter", LogLevel.Debug }, //Android
						// { "Windows.UI.Xaml.Controls.BufferViewCache", LogLevel.Debug }, //Android
						// { "Windows.UI.Xaml.Controls.VirtualizingPanelGenerator", LogLevel.Debug }, //WASM
					}
				)
#if DEBUG
				.AddConsole(LogLevel.Debug);
#else
				.AddConsole(LogLevel.Information);
#endif
		}

		private void InitializeMaterialStyles()
		{
			// Set a default palette to make sure all colors used by MaterialResources exist
			this.Resources.MergedDictionaries.Add(new global::Uno.Material.MaterialColorPalette());

			// Overlap the default colors with the application's colors palette. 
			this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("ms-appx:///Styles/ColorPaletteOverride.xaml") });

			// Add all the material resources. Those resources depend on the colors above, which is why this one must be added last.
			this.Resources.MergedDictionaries.Add(new global::Uno.Material.MaterialResources());
		}
	}
}
