using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NativeStylesSwitch
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();


#if !(__IOS__ || __ANDROID__)
            useNative.Content = "Available only on iOS and Android";
            useNative.IsEnabled = false;
#endif
        }

        private Style SwitchStyle(bool isNative, Style native, Style winui)
        {
            return isNative ? native : winui;
        }

        public class StyleSwitch : IValueConverter
        {
            public Style Native { get; set; }

            public Style WinUI { get; set; }

            public object Convert(object value, Type targetType, object parameter, string language)
            {
                if (value is bool isNative)
                {
                    return isNative ? Native : WinUI;
                }

                return null;
            }

            public object ConvertBack(object value, Type targetType, object parameter, string language)
                => throw new NotImplementedException();
        }
    }
}
