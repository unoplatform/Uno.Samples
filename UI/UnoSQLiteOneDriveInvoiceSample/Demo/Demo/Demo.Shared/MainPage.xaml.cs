using Demo.Database.Entities;
using Demo.Pages;
using Demo.ViewModels;

using Microsoft.Graph.TermStore;
using Microsoft.UI.Xaml.Controls;

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

namespace Demo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            ViewModel = new MainVM();
            this.InitializeComponent();

        }

        public MainVM ViewModel { get; private set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //Frame.Navigate(typeof(CUInvoice), new Invoice());
            if (ViewModel == null)
            {
                ViewModel = new MainVM();
            }

            ViewModel.SettingsVM = new SettingsVM();
            

        }

        private async void tabView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            
        }
    }
}
