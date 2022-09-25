using System;
using System.Security.AccessControl;

using UnoGoodReads.Views;

using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UnoGoodReads
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        
        public MainPage()
        {
            this.InitializeComponent();
            contentFrame.Navigate(typeof(HomePage), null);
            
        }

        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var item = args.InvokedItem as NavigationViewItem;
            
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var item = sender.SelectedItem as NavigationViewItem;
            Type pageType = typeof(HomePage);
            if (item.Tag.Equals("Home"))
            {
                pageType = typeof(HomePage);
            }
            else if (item.Tag.Equals("Author"))
            {
                pageType = typeof(AuthorPage);
            }
            else if (item.Tag.Equals("Book"))
            {
                pageType = typeof(BookPage);
            }
            contentFrame.Navigate(pageType, null);
        }
    }
}

