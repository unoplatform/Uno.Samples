using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;

#if __ANDROID__ || __IOS__ || __WINDOWS__
using Windows.Media.Capture;
#endif

namespace CameraCaptureUISample;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    private async void CaptureButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            StorageFile? photo = null;

#if __ANDROID__ || __IOS__ || __WINDOWS__
            var captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Windows.Foundation.Size(200, 200);

            photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
#else
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");

            photo = await picker.PickSingleFileAsync();
#endif

            if (photo is null)
            {
                StatusText.Text = "No photo selected.";
                return;
            }

            var bitmapImage = new BitmapImage();
            using (var stream = await photo.OpenReadAsync())
            {
                await bitmapImage.SetSourceAsync(stream);
            }

            CapturedImage.Source = bitmapImage;
            StatusText.Text = "Photo loaded successfully.";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }
}
