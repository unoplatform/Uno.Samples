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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UnoLocalization
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LanguageSettings : Page
	{
		public LanguageSettings()
		{
			this.InitializeComponent();
		}

		private void SetAppLanguage(object sender, RoutedEventArgs e)
		{
			if ((sender as Button)?.Tag is string tag)
			{
				// Change the app language
				Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = tag;

				// Clear the back-navigation stack, and send the user to MainPage
				// This is done because any loaded pages (MainPage(in back-stack) and LanguageSettings (current active page))
				// will stay in the previous language until reloaded.
				Frame.BackStack.Clear();
				Frame.Navigate(typeof(MainPage));
			}
		}

		private void GoBack(object sender, RoutedEventArgs e) => Frame.GoBack();
	}
}
