using Windows.System;

namespace UnoWCTSettingsCardSample;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    private async void OpenLink_Click(object sender, RoutedEventArgs e)
    {
        var uri = new Uri("https://aka.platform.uno/get-started");
        await Launcher.LaunchUriAsync(uri);
    }
}
