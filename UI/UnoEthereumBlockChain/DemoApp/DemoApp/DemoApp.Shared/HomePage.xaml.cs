using DemoApp.TabbedPages;
using DemoApp.ViewModels;

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

namespace DemoApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {

        #region Properties

        public Type CurrentPage { get; set; }
        public HomeViewModel ViewModel { get; set; }

        #endregion

        public HomePage()
        {
            this.InitializeComponent();
            ViewModel = DataContext as HomeViewModel;
            
        }

        #region Method(s)

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (ViewModel == null)
            {
                ViewModel = DataContext as HomeViewModel;
            }
            ViewModel.SetUp = e.Parameter as SetUp;
            CurrentPage = typeof(ManufacturerPage);
            pageView.Navigate(CurrentPage, ViewModel.SetUp);
        }

        void OnAppBarButtonTapped(object sender, RoutedEventArgs args)
        {
            if (sender is FrameworkElement element)
            {
                var tag = element.Tag.ToString();
                Type page = null;
                switch (tag)
                {
                    case "Manufacturer":
                        page = typeof(ManufacturerPage);
                        break;
                    case "Wholesaler":
                        page = typeof(WholesalerPage);
                        break;
                    case "Pharmacy":
                        page = typeof(PharmacyPage);
                        break;
                    case "Patient":
                        
                        page = typeof(PatientPage);
                        break;

                    default:
                        page = typeof(ManufacturerPage);
                        break;
                }

                if (CurrentPage != page)
                {
                    CurrentPage = page;
                    pageView.Navigate(page, ViewModel.SetUp);
                }
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        #endregion
    }
}
