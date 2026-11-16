using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Foundation;
using Windows.Media.Capture;

namespace CameraCaptureUISample;

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

            // The CaptureFileAsync returns null on WinUI3 - https://github.com/microsoft/WindowsAppSDK/issues/1034
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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }
}
