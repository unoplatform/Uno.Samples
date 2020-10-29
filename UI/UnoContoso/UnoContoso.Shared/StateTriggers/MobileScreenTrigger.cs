using Microsoft.EntityFrameworkCore.Update;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.System.Profile;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UnoContoso.StateTriggers
{
    public class MobileScreenTrigger : StateTriggerBase
    {
        private UserInteractionMode _interactionMode;

        public MobileScreenTrigger()
        {
            Windows.UI.Xaml.Window.Current.SizeChanged += Window_SizeChanged;
        }

        private void Window_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            UpdateTrigger(e.Size);
        }

        /// <summary>
        /// The target device family.
        /// </summary>
        public UserInteractionMode InteractionMode
        {
            get { return _interactionMode; }
            set 
            { 
                _interactionMode = value;
                Windows.Foundation.Size size = new Windows.Foundation.Size()
                {
                    Width = Windows.UI.Xaml.Window.Current.Bounds.Width,
                    Height = Windows.UI.Xaml.Window.Current.Bounds.Height
                };
                UpdateTrigger(size); 
            }
        }

        private void UpdateTrigger(Windows.Foundation.Size size)
        {
            // Get the current device family and interaction mode.
            var _currentDeviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
            //not implement
            //var _currentInteractionMode = UIViewSettings.GetForCurrentView().UserInteractionMode;
            //_currentInteractionMode == InteractionMode

            // The trigger will be activated if the current device family is Windows.Mobile
            // and the UserInteractionMode matches the interaction mode value in XAML.
            if (_currentDeviceFamily.Contains("Mobile")
                && size.Width < size.Height)
                SetActive(_currentDeviceFamily.Contains("Mobile"));
        }
    }
}
