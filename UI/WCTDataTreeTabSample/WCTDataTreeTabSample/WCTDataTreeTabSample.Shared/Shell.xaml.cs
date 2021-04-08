using System;
using WCTDataTreeTabSample.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WCTDataTreeTabSample
{
	public sealed partial class Shell : Page
	{
		public Shell()
		{
			this.InitializeComponent();

			this.Loaded += Shell_Loaded;
		}

		private void Shell_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			SetDarkLightToggleInitialState();

			App.NavigationFrame = this.ContentFrame;
			App.NavigationView = this.NavView;

			App.NavigateTo(typeof(TreeViewPage));
		}

		private void NavView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
		{
			var item = args.InvokedItemContainer as Microsoft.UI.Xaml.Controls.NavigationViewItem;

			switch (item.Tag?.ToString() ?? string.Empty)
			{
				case nameof(TreeViewPage):
					App.NavigateTo(typeof(TreeViewPage));
					break;

				case nameof(MountainsPage):
					App.NavigateTo(typeof(MountainsPage));
					break;

				case nameof(LocationsPage):
					App.NavigateTo(typeof(LocationsPage));
					break;

				case nameof(TabViewPage):
					App.NavigateTo(typeof(TabViewPage));
					break;

				case nameof(MasterDetailsPage):
					App.NavigateTo(typeof(MasterDetailsPage));
					break;

				case nameof(TwoPaneViewPage):
					App.NavigateTo(typeof(TwoPaneViewPage));
					break;

				case nameof(ExpanderPage):
					App.NavigateTo(typeof(ExpanderPage));
					break;
			}
		}

		private void ToggleButton_Click(object sender, RoutedEventArgs e)
		{
			// Set theme for window root.
			if (global::Windows.UI.Xaml.Window.Current.Content is FrameworkElement root)
			{
				switch (root.ActualTheme)
				{
					case ElementTheme.Default:
						if (SystemThemeHelper.GetSystemApplicationTheme() == ApplicationTheme.Dark)
						{
							root.RequestedTheme = ElementTheme.Light;
						}
						else
						{
							root.RequestedTheme = ElementTheme.Dark;
						}
						break;
					case ElementTheme.Light:
						root.RequestedTheme = ElementTheme.Dark;
						break;
					case ElementTheme.Dark:
						root.RequestedTheme = ElementTheme.Light;
						break;
				}
			}
		}

		private void SetDarkLightToggleInitialState()
		{
			// Initialize the toggle to the current theme.
			var root = global::Windows.UI.Xaml.Window.Current.Content as FrameworkElement;

			switch (root.ActualTheme)
			{
				case ElementTheme.Default:
					DarkLightModeToggle.IsChecked = SystemThemeHelper.GetSystemApplicationTheme() == ApplicationTheme.Dark;
					break;
				case ElementTheme.Light:
					DarkLightModeToggle.IsChecked = false;
					break;
				case ElementTheme.Dark:
					DarkLightModeToggle.IsChecked = true;
					break;
			}
		}
	}
}