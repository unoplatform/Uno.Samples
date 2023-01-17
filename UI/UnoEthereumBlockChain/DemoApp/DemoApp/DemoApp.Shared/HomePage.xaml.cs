using DemoApp.TabbedPages;
using DemoApp.ViewModels;

using Microsoft.UI.Xaml.Controls;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

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
            var setUp = e.Parameter as SetUp;

            ViewModel.PatientVM.SetUp = setUp;
            ViewModel.PharmacyVM.SetUp = setUp;
            ViewModel.ManufacturerVM.SetUp = setUp;
            ViewModel.WholesalerVM.SetUp = setUp;

            ViewModel.ManufacturerVM.Bind();
            ViewModel.PharmacyVM.Bind();
            ViewModel.WholesalerVM.Bind();
            ViewModel.PatientVM.Bind();

        }


        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void TabView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tabbedView = sender as TabView;
            
        }

        #endregion


    }
}
