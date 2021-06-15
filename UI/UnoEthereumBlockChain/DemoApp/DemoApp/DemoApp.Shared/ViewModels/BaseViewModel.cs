using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DemoApp.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implements

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}
