using System;
using System.Collections.Generic;
using TimeEntryUno.Shared.Helpers;
using TimeEntryUno.Shared.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TimeEntryUno
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationSyncHelper _navigationSyncHelper;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            _navigationSyncHelper = new NavigationSyncHelper(
                NavView,
                ContentFrame,
                new Dictionary<string, Type>()
                {
                    {"HomePage",        typeof(HomePage) },
                    {"TimeEntryPage",   typeof(TimeEntryPage) },
                    {"ReportsPage",     typeof(ReportsPage) },
                    {"AdminPage",       typeof(AdminPage) },
                    {"AboutPage",       typeof(AboutPage) },
                });

        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(HomePage), null, new EntranceNavigationTransitionInfo());
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            // log errors
            // show error window
        }
    }
}
