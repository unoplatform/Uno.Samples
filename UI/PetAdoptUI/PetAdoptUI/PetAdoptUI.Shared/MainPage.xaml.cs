using Microsoft.UI.Xaml.Controls;
using PetAdoptUI.Image;
using PetAdoptUI.Model;
using System.Collections.ObjectModel;

namespace PetAdoptUI
{
    public sealed partial class MainPage : Page
    {
        public readonly ObservableCollection<FileInfo> PetImages = new ObservableCollection<FileInfo>();

        public MainPage()
        {
            this.InitializeComponent();

            this.GenerateImagesAsync();
        }

        private async void GenerateImagesAsync()
        {
            PetImages.Add(new FileInfo(await ImageFileManager.DownloadFileAsync("https://picsum.photos/id/237/390/360")));
            PetImages.Add(new FileInfo(await ImageFileManager.DownloadFileAsync("https://picsum.photos/id/40/390/360")));
            PetImages.Add(new FileInfo(await ImageFileManager.DownloadFileAsync("https://picsum.photos/id/582/390/360")));
            PetImages.Add(new FileInfo(await ImageFileManager.DownloadFileAsync("https://picsum.photos/id/593/390/360")));
            PetImages.Add(new FileInfo(await ImageFileManager.DownloadFileAsync("https://picsum.photos/id/659/390/360")));
            PetImages.Add(new FileInfo(await ImageFileManager.DownloadFileAsync("https://picsum.photos/id/718/390/360")));
            PetImages.Add(new FileInfo(await ImageFileManager.DownloadFileAsync("https://picsum.photos/id/783/390/360")));
        }

        private ImageManager ImageFileManager => imageFileManager ??= new();
        private ImageManager imageFileManager;
    }
}