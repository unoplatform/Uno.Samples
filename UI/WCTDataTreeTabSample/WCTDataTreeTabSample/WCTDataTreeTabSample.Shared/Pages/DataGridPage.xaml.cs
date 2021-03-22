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
		private MountainDataSource MountainGridData = new MountainDataSource();
		private LocationDataSource LocationGridData = new LocationDataSource();

		public DataGridPage()
		{
			this.InitializeComponent();

			Loaded += DataGridPage_Loaded;
		}

		private async void DataGridPage_Loaded(object sender, RoutedEventArgs e)
		{
			var mountains = await MountainGridData.GetDataAsync();
			this.MountainDataGrid.ItemsSource = mountains.ToList();

			var locations = await LocationGridData.GetDataAsync();
			this.LocationDataGrid.ItemsSource = locations.ToList();
		}
	}
}
