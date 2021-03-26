using Microsoft.Toolkit.Uwp.SampleApp.Data;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WCTDataTreeTabSample
{
	public sealed partial class MountainsPage : Page
	{
		private MountainDataSource MountainData = new MountainDataSource();

		public MountainsPage()
		{
			this.InitializeComponent();

			Loaded += MountainDataGridPage_Loaded;
		}

		private async void MountainDataGridPage_Loaded(object sender, RoutedEventArgs e)
		{
			var mountains = await MountainData.GetDataAsync();
			this.MountainDataGrid.ItemsSource = mountains.ToList();
		}
	}
}
