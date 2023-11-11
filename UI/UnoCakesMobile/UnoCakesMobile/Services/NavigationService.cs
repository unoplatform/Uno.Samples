using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoCakesMobile.Services
{
    public sealed class NavigationService : INavigationService
    {
        private Frame _frame;
        internal NavigationService(Frame frame)
        {
            _frame = frame;
        }

        public bool CanGoBack => _frame.CanGoBack;

        public void GoBack()
        {
            if (_frame.DispatcherQueue.HasThreadAccess)
                _frame.GoBack();
            else
                _frame.DispatcherQueue.TryEnqueue(_frame.GoBack);
        }

        public bool Navigate(Type sourcePageType, object parameter = null, NavigationTransitionInfo transition = null)
        {
            if (_frame.DispatcherQueue.HasThreadAccess)
                return _frame.Navigate(sourcePageType, parameter, transition);
            else
            {
                bool success = false;
                ManualResetEvent mre = new ManualResetEvent(false);
                _frame.DispatcherQueue.TryEnqueue(() =>
                {
                    success = _frame.Navigate(sourcePageType, parameter, transition);
                    mre.Set();
                });

                // wait for completion on UI thread and return result
                mre.WaitOne();
                return success;
            }
        }
    }
}
