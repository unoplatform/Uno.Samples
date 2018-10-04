using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.UI;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace AndroidCustomCamera
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();

			BaseActivity.Current.ActivityResult += Current_ActivityResult;
		}

		private void Current_ActivityResult(object sender, BaseActivity.ActivityResultEventArgs e)
		{
			var bitmap = (Bitmap)e.Data.Extras.Get("data");

			image.Source = bitmap;
		}

		public void button_Click(object sender, RoutedEventArgs e)
		{
			var intent = new Intent(MediaStore.ActionImageCapture);
			BaseActivity.Current.StartActivityForResult(intent, 0);
		}
	}
}
