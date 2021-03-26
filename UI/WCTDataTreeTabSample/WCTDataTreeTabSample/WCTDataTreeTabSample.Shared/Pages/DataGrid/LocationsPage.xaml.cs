using Microsoft.Toolkit.Uwp.SampleApp.Data;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WCTDataTreeTabSample
{
	public sealed partial class LocationsPage : Page
	{
		private LocationDataSource LocationData = new LocationDataSource();

		public LocationsPage()
		{
			this.InitializeComponent();

			Loaded += LocationsPage_Loaded;
		}

		private async void LocationsPage_Loaded(object sender, RoutedEventArgs e)
		{
			var locations = await LocationData.GetDataAsync();
			this.LocationDataGrid.ItemsSource = locations.ToList();
		}
	}
}
