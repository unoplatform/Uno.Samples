using System;
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
			App.NavigationFrame = this.ContentFrame;
			App.NavigationView = this.NavView;

			App.NavigateTo(typeof(TreeViewPage));
		}

		private void NavView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
		{
			var item = args.InvokedItemContainer as Microsoft.UI.Xaml.Controls.NavigationViewItem;

			switch (item.Tag.ToString())
			{
				case nameof(TreeViewPage):
					App.NavigateTo(typeof(TreeViewPage));
					break;

				case nameof(DataGridPage):
					App.NavigateTo(typeof(DataGridPage));
					break;
			}
		}
	}
}