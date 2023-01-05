using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PackageResourcesSample
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

        private async void LoadPackageFile()
        {
            try
            {
                var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri(assetFileName.Text));

                output.Text = await FileIO.ReadTextAsync(file);
            }
            catch (Exception e)
            {
                output.Text = e.ToString();
            }
        }

        private async void LoadNestedPackageFile()
        {
            try
            {
                var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri(nestedAssetFileName.Text));

                output.Text = await FileIO.ReadTextAsync(file);
            }
            catch (Exception e)
            {
                output.Text = e.ToString();
            }
        }
    }
}