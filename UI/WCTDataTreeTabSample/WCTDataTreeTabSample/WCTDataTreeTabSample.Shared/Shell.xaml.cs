namespace WCTDataTreeTabSample;

public sealed partial class Shell : Page
{
        public Shell()
	{
		this.InitializeComponent();

		this.Loaded += Shell_Loaded;
	}

	private void Shell_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		SetDarkLightToggleInitialState();

		App.NavigationFrame = this.ContentFrame;
		App.NavigationView = this.NavView;

		App.NavigateTo(typeof(TreeViewPage));
	}

	private void NavViewToggleButton_Click(object sender, RoutedEventArgs e)
	{
		NavView.IsPaneVisible = !NavView.IsPaneVisible;
		NavView.IsPaneOpen = NavView.IsPaneVisible;
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
            if (SystemThemeHelper.IsRootInDarkMode(this.XamlRoot))
            {
			SystemThemeHelper.SetRootTheme(this.XamlRoot, false);
            }
            else
            {
                SystemThemeHelper.SetRootTheme(this.XamlRoot, true);
            }
	}

	private void SetDarkLightToggleInitialState()
	{
		DarkLightModeToggle.IsChecked = SystemThemeHelper.IsRootInDarkMode(this.XamlRoot);
	}
}