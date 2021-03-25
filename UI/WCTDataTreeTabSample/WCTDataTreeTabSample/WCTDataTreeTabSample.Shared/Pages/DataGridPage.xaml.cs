using Microsoft.Toolkit.Uwp.SampleApp.Data;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using WCTDataTreeTabSample.Entities;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WCTDataTreeTabSample
{
	public sealed partial class DataGridPage : Page
	{
		private MountainDataSource MountainData = new MountainDataSource();
		private LocationDataSource LocationData = new LocationDataSource();

		public DataGridPage()
		{
			this.InitializeComponent();

			Loaded += DataGridPage_Loaded;
		}

		private async void DataGridPage_Loaded(object sender, RoutedEventArgs e)
		{
			var mountains = await MountainData.GetDataAsync();
			this.MountainDataGrid.ItemsSource = mountains.ToList();

			var locations = await LocationData.GetDataAsync();
			this.LocationDataGrid.ItemsSource = locations.ToList();
		}
	}
}
