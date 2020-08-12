using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
	}

	class StyleSwitch : IValueConverter
	{
		public Style Native { get; set; }

		public Style WinUI { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if(value is bool isNative)
			{
				return isNative ? Native : WinUI;
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) 
			=> throw new NotImplementedException();
	}
}

