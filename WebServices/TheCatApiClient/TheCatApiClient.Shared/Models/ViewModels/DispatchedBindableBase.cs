using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace TheCatApiClient.Shared.Models.ViewModels
{
    public abstract class DispatchedBindableBase : INotifyPropertyChanged
    {
        // Insert the event and property below here
        public event PropertyChangedEventHandler PropertyChanged;
        protected CoreDispatcher Dispatcher => CoreApplication.MainView.Dispatcher;


        // Insert SetProperty below here
        protected virtual bool SetProperty<T>(ref T backingVariable, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingVariable, value)) return false;

            backingVariable = value;
            RaisePropertyChanged(propertyName);

            return true;
        }

        // Insert RaisePropertyChanged below here
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            DispatchAsync(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        // Insert Dispatch below here
        protected async Task DispatchAsync(DispatchedHandler callback)
        {
            if (Dispatcher.HasThreadAccess)
            {
                callback.Invoke();
            }
            else
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, callback);
            }
        }
    }
}
