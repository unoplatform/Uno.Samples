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
            UpdateTrigger();
        }

        /// <summary>
        /// The target device family.
        /// </summary>
        public UserInteractionMode InteractionMode
        {
            get { return _interactionMode; }
            set { _interactionMode = value; UpdateTrigger(); }
        }

        private void UpdateTrigger()
        {
            // Get the current device family and interaction mode.
            var _currentDeviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
            //not implement
            //var _currentInteractionMode = UIViewSettings.GetForCurrentView().UserInteractionMode;
            //_currentInteractionMode == InteractionMode

            // The trigger will be activated if the current device family is Windows.Mobile
            // and the UserInteractionMode matches the interaction mode value in XAML.
            if (_currentDeviceFamily.Contains("Mobile"))
                SetActive(_currentDeviceFamily.Contains("Mobile"));
        }
    }
}
