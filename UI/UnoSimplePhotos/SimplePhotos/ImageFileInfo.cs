using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;

namespace SimplePhotos
{
    public class ImageFileInfo : INotifyPropertyChanged
    {
        public ImageFileInfo(StorageFile imageFile,
        string name,
        string type,
        Uri uri)
        {
            ImageName = name;
            ImageFileType = type;
            ImageFile = imageFile;
            ImageUri = uri;
        }

        public StorageFile ImageFile { get; }

        private Uri ImageUri { get; }

        public string ImageSource => ImageUri.ToString();

        public string ImageName { get; }

        public string ImageFileType { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}