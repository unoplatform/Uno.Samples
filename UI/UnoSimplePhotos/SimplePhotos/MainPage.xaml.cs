using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.Storage.Search;

namespace SimplePhotos;

public sealed partial class MainPage : Page
{
    public ObservableCollection<ImageFileInfo> Images { get; } =
            new ObservableCollection<ImageFileInfo>();
    public MainPage()
    {
        this.InitializeComponent();
        GetItems();
    }

    private async void GetItems()
    {
        foreach (int i in Enumerable.Range(1, 20))
        {
            var uri = new Uri($"ms-appx:///SimplePhotos/Assets/Samples/{i}.jpg");

            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            Images.Add(new(file, file.Name, $"{file.FileType} File", uri));
        }
    }
}
