using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace PetAdoptUI
{
    public sealed partial class MainPage : Page
    {
        public readonly ObservableCollection<string> PetImages = new ObservableCollection<string>();

        public MainPage()
        {
            this.InitializeComponent();
            GenerateImages();
        }

        private void GenerateImages()
        {
            PetImages.Add("https://picsum.photos/id/237/390/360");
            PetImages.Add("https://picsum.photos/id/40/390/360");
            PetImages.Add("https://picsum.photos/id/582/390/360");
            PetImages.Add("https://picsum.photos/id/593/390/360");
            PetImages.Add("https://picsum.photos/id/659/390/360");
            PetImages.Add("https://picsum.photos/id/718/390/360");
            PetImages.Add("https://picsum.photos/id/783/390/360");

        }
    }
}
