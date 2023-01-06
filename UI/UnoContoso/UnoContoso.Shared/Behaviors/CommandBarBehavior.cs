using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Core;
using Microsoft.UI.Xaml.Controls;

namespace UnoContoso.Behaviors
{
    public class CommandBarBehavior : Behavior<CommandBar>
    {
        protected override void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            Microsoft.UI.Xaml.Window.Current.SizeChanged += Window_SizeChanged;
        }

        private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            UpdateCommandBar();
        }

        private void AssociatedObject_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            UpdateCommandBar();
        }

        private void UpdateCommandBar()
        {
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.Contains("Mobile"))
            {
                AssociatedObject.DefaultLabelPosition = CommandBarDefaultLabelPosition.Bottom;
            }
            else
            {
                var width = (double)App.Current.Resources["MediumWindowSnapPoint"];
                if(Microsoft.UI.Xaml.Window.Current.Bounds.Width < width)
                {
                    AssociatedObject.DefaultLabelPosition = CommandBarDefaultLabelPosition.Bottom;
                }
                else
                {
                    AssociatedObject.DefaultLabelPosition = CommandBarDefaultLabelPosition.Right;
                }
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            Microsoft.UI.Xaml.Window.Current.SizeChanged -= Window_SizeChanged;
        }
    }
}
