using Android.Content;
using Android.Graphics;
using Android.Provider;
using Uno.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

            if (e.Data != null)
            {
                var bitmap = (Bitmap)e.Data.Extras.Get("data");

                image.Source = bitmap;
            }
            else
            {
                image.Source = null;
            }
		}

		public void button_Click(object sender, RoutedEventArgs e)
		{
			var intent = new Intent(MediaStore.ActionImageCapture);
			BaseActivity.Current.StartActivityForResult(intent, 0);
		}
	}
}
