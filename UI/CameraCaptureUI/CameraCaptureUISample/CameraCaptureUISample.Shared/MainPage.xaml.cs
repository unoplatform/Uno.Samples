using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CameraCaptureUISample
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();
		}

		public async void button_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var captureUI = new CameraCaptureUI();
				captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
				captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);

				var photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

				if (photo == null)
				{
					return;
				}
				else
				{
					var source = new BitmapImage(new Uri(photo.Path));
					image.Source = source;
				}
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}
	}
}
