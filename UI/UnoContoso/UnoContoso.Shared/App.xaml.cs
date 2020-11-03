using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Prism.Modularity;
using Prism;
using Prism.Ioc;
using UnoContoso.Views;
using Windows.Storage;
using UnoContoso.Repository;
using UnoContoso.Repository.Rest;
using UnoContoso.Repository.Sql;
using DryIoc;
using Microsoft.EntityFrameworkCore;
using UnoContoso.Helpers;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Reflection;
using Prism.Regions;
using UnoContoso.Models.Consts;
using UnoContoso.Controls;
using UnoContoso.ControlViewModels;

namespace UnoContoso
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App
	{
		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			ConfigureFilters(global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory);

			this.InitializeComponent();
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="e">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs e)
		{
			base.OnLaunched(e);
		}

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        protected override void OnSuspending(SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        protected override UIElement CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
			//var regionManager = Container.Resolve<IRegionManager>();
			//var contentRegion = regionManager.Regions[Regions.CONTENT_REGION];
			//containerRegistry.RegisterInstance<IRegionNavigationService>(contentRegion.NavigationService);

            containerRegistry.Register<IContosoRepository, RestContosoRepository>("Rest");
            containerRegistry.Register<IContosoRepository, SqlContosoRepository>("Sql");
            //containerRegistry.Register<IContosoRepository, SqlContosoRepository>();

			// Load the database.
			if (ApplicationData.Current.LocalSettings.Values.TryGetValue(
				"data_source", out object dataSource))
			{
				switch (dataSource.ToString())
				{
					case "Rest": UseRest(containerRegistry); break;
					default: UseSqlite(containerRegistry); break;
				}
			}
			else
			{
				//UseSqlite(containerRegistry);
				UseRest(containerRegistry);
			}

			containerRegistry.RegisterForNavigation<HomeView>();
			containerRegistry.RegisterForNavigation<CustomerListView>();
			containerRegistry.RegisterForNavigation<CustomerDetailView>();
			containerRegistry.RegisterForNavigation<OrderListView>();
			containerRegistry.RegisterForNavigation<OrderDetailView>();

			containerRegistry.RegisterDialog<MessageControl, MessageViewModel>();
			containerRegistry.RegisterDialog<ConfirmControl, ConfirmViewModel>();
		}

		protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
			ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
			{
				string viewName = viewType.FullName;
				if (viewName == null)
				{
					return null;
				}

				if (viewName.EndsWith("View"))
				{
					viewName = viewName.Substring(0, viewName.Length - 4);
				}

				if (viewName.EndsWith("Control"))
				{
					viewName = viewName.Substring(0, viewName.Length - 7);
				}

				viewName = viewName.Replace(".Views.", ".ViewModels.");
				viewName = viewName.Replace(".Controls.", ".ControlViewModels.");
				string viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
				string viewModelName = $"{viewName}ViewModel, {viewAssemblyName}";
				return Type.GetType(viewModelName);
			});

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

		/// <summary>
		/// Configures the app to use the Sqlite data source. If no existing Sqlite database exists, 
		/// loads a demo database filled with fake data so the app has content.
		/// </summary>
		public async void UseSqlite(IContainerRegistry containerRegistry)
		{
			await Task.CompletedTask;
			//string databasePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Contoso.db");
			//if (!File.Exists(databasePath))
			//{
			//	using (Stream sourceStream = await StreamHelperEx.GetEmbeddedFileStreamAsync(GetType(), "Contoso.db"))
			//	{
			//		var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Contoso.db");
			//		using (var fileStream = await file.OpenStreamForWriteAsync())
   //                 {
			//			await sourceStream.CopyToAsync(fileStream);
			//			await fileStream.FlushAsync();
   //                 }
			//	}
			//}

   //         var dbOptions = new DbContextOptionsBuilder<ContosoContext>()
			//	.UseSqlite("Data Source=" + databasePath);
   //         //var dbOptions = new DbContextOptionsBuilder<ContosoContext>();

   //         //var repository = Container.Resolve<IContosoRepository>("Sql");
   //         var repository = new SqlContosoRepository(dbOptions);
   //         containerRegistry.RegisterInstance<IContosoRepository>(repository);
        }

		/// <summary>
		/// Configures the app to use the REST data source. For convenience, a read-only source is provided. 
		/// You can also deploy your own copy of the REST service locally or to Azure. See the README for details.
		/// </summary>
		public void UseRest(IContainerRegistry containerRegistry) 
		{
#if __ANDROID__
            var repository = new RestContosoRepository("http://10.0.2.2:5000/api/");
#else
            var repository = new RestContosoRepository("http://localhost:5000/api/");
#endif
            //var repository = new RestContosoRepository("https://unocontososervice20201016190520.azurewebsites.net/api/");
            containerRegistry.RegisterInstance<IContosoRepository>(repository);
		}
	}
}
