using Microsoft.Toolkit.Uwp.SampleApp.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WCTTestbed2
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
        private DataGridDataSource viewModel = new DataGridDataSource();
		
		public MainPage()
		{
			this.InitializeComponent();

			Loading += MainPage_Loading;
		}

		private async void MainPage_Loading(object sender, object args)
		{
			dataGrid.ItemsSource = await viewModel.GetDataAsync();
		}
	}
}
