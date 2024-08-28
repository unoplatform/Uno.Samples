#if __IOS__ || __ANDROID__
using Uno.Samples;
#endif

namespace MediaGallerySample;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    private async void CheckAccessClick(object sender, RoutedEventArgs e)
    {
#if __IOS__ || __ANDROID__
        var success = await MediaGallery.CheckAccessAsync();
        await new ContentDialog
        {
            Title = "Permission check",
            Content = $"Access {(success ? "granted" : "denied")}.",
            CloseButtonText = "OK",
            XamlRoot = XamlRoot
        }.ShowAsync();
#endif
    }

    private async void SaveClick(object sender, RoutedEventArgs e)
    {
#if __IOS__ || __ANDROID__
        if (await MediaGallery.CheckAccessAsync())
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/UnoLogo.png", UriKind.Absolute));
            using var stream = await file.OpenStreamForReadAsync();
            await MediaGallery.SaveAsync(MediaFileType.Image, stream, "UnoLogo.png");
        }
        else
        {
            await new ContentDialog
            {
                Title = "Permission required",
                Content = "The app requires access to the device's gallery to save the image.",
                CloseButtonText = "OK",
                XamlRoot = XamlRoot
            }.ShowAsync();
        }
#endif
    }

    private async void SaveRandomNameClick(object sender, RoutedEventArgs e)
    {
#if __IOS__ || __ANDROID__
        if (await MediaGallery.CheckAccessAsync())
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/UnoLogo.png", UriKind.Absolute));
            using var stream = await file.OpenStreamForReadAsync();

            var fileName = Guid.NewGuid() + ".png";
            await MediaGallery.SaveAsync(MediaFileType.Image, stream, fileName);
        }
        else
        {
            await new ContentDialog
            {
                Title = "Permission required",
                Content = "The app requires access to the device's gallery to save the image.",
                CloseButtonText = "OK",
                XamlRoot = XamlRoot
            }.ShowAsync();
        }
#endif
    }
}
