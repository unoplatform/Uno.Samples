using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PetAdoptUI.Model
{
    public class FileInfo : INotifyPropertyChanged
    {
        public FileInfo(byte[] file) => File = file;

        public byte[] File
        {
            get => _file; 
            set
            {
                _file = value;
                OnPropertyChanged();
            }
        }

        private byte[] _file;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}