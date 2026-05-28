using Microsoft.UI.Xaml.Media.Imaging;

namespace QuoteCraft.Presentation;

public sealed partial class SettingsPage : Page
{
    /// <summary>Access the underlying MVUX model from the generated ViewModel DataContext.</summary>
    private SettingsModel? Model => MvuxHelper.GetModel<SettingsModel>(DataContext);

    public SettingsPage()
    {
        this.InitializeComponent();
        this.Loaded += SettingsPage_Loaded;
    }

    private async void SettingsPage_Loaded(object sender, RoutedEventArgs e)
    {
        // Wait briefly for the model to load, then refresh logo thumbnails
        await Task.Delay(300);
        var model = Model;
        if (model is null) return;

        var logoPath = await model.LogoPath;
        if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
        {
            UpdateLogoThumbnails(logoPath);
        }
    }

    private async void UploadLogo_Click(object sender, RoutedEventArgs e)
    {
        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.FileTypeFilter.Add(".jpg");
        picker.FileTypeFilter.Add(".jpeg");
        picker.FileTypeFilter.Add(".png");
        picker.FileTypeFilter.Add(".bmp");
        picker.FileTypeFilter.Add(".webp");

#if !__BROWSERWASM__
        WinRT.Interop.InitializeWithWindow.Initialize(picker,
            WinRT.Interop.WindowNative.GetWindowHandle(App.MainAppWindow));
#endif

        var file = await picker.PickSingleFileAsync();
        if (file is null) return;

        // Copy to local data directory
        var logoDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "QuoteCraft", "logo");
        Directory.CreateDirectory(logoDir);

        var ext = Path.GetExtension(file.Name);
        var destPath = Path.Combine(logoDir, $"logo{ext}");
        var sourceBytes = await File.ReadAllBytesAsync(file.Path);
        await File.WriteAllBytesAsync(destPath, sourceBytes);

        // Update model
        var model = Model;
        if (model is not null)
            await model.SetLogoPath(destPath, CancellationToken.None);

        // Update thumbnails in UI
        UpdateLogoThumbnails(destPath);
    }

    private void UpdateLogoThumbnails(string filePath)
    {
        try
        {
            var uri = new Uri(filePath);
            var bitmap = new BitmapImage(uri);

            // Settings form thumbnail
            LogoThumbnailImage.Source = bitmap;
            LogoThumbnailBorder.Visibility = Visibility.Visible;
            LogoFileName.Text = Path.GetFileName(filePath);
            LogoFileName.Visibility = Visibility.Visible;

            // Preview quote logo
            var previewBitmap = new BitmapImage(uri);
            PreviewLogoImage.Source = previewBitmap;
            PreviewLogoImage.Visibility = Visibility.Visible;
            PreviewLogoPlaceholder.Visibility = Visibility.Collapsed;
        }
        catch
        {
            // If we can't load the image, just leave the placeholders
        }
    }
}
