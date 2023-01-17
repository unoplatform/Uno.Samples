using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace TimeEntryUno.Shared.Helpers
{
    internal class NavigationSyncHelper
    {
        private Microsoft.UI.Xaml.Controls.NavigationView _navigationView;
        private Frame _frame;
        private Microsoft.UI.Xaml.Controls.NavigationViewItem _lastInvokedMenuItem;
        private Dictionary<string, Type> _pageMap;

        internal NavigationSyncHelper(
            Microsoft.UI.Xaml.Controls.NavigationView navigationView, 
            Frame frame, 
            Dictionary<string, Type> pageMap)
        {
            _frame = frame;
            _navigationView = navigationView;
            _pageMap = pageMap;

            _navigationView.ItemInvoked += NavView_ItemInvoked;
            _navigationView.BackRequested += NavView_BackRequested;
            _frame.Navigated += Frame_Navigated;
        }

        private void NavView_ItemInvoked(
            Microsoft.UI.Xaml.Controls.NavigationView sender, 
            Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            var invokedMenuItem = args.InvokedItemContainer as Microsoft.UI.Xaml.Controls.NavigationViewItem;

            if (invokedMenuItem == null || invokedMenuItem == _lastInvokedMenuItem)
            {
                return;
            }

            var tag = invokedMenuItem.Tag.ToString();
            if (_pageMap.ContainsKey(tag))
            {
                var destinationType = _pageMap[tag];
                if (!_frame.Navigate(destinationType, null, args.RecommendedNavigationTransitionInfo))
                {
                    return;
                }
                _lastInvokedMenuItem = invokedMenuItem;
            }
        }

        private void NavView_BackRequested(
            Microsoft.UI.Xaml.Controls.NavigationView sender, 
            Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            if (_frame.CanGoBack)
            {
                _frame.GoBack();
            }
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            var currentSelectedItem = _navigationView.MenuItems
                .FirstOrDefault(mi => ((Microsoft.UI.Xaml.Controls.NavigationViewItem)mi).IsSelected) as Microsoft.UI.Xaml.Controls.NavigationViewItem;
            if (currentSelectedItem != null)
            {
                var tag = currentSelectedItem.Tag.ToString();
                var currentSelectedType = _pageMap[currentSelectedItem.Tag.ToString()];
                if (e.SourcePageType != currentSelectedType)
                {
                    SetSelectedItem();
                }
            }
            else
            {
                SetSelectedItem();
            }

            void SetSelectedItem()
            {
                var tagToFind = _pageMap.FirstOrDefault(entry => entry.Value == e.SourcePageType).Key;
                if (_navigationView.MenuItems.FirstOrDefault(mi => ((Microsoft.UI.Xaml.Controls.NavigationViewItem)mi).Tag.Equals(tagToFind)) is Microsoft.UI.Xaml.Controls.NavigationViewItem matchedItem)
                {
                    matchedItem.IsSelected = true;
                    _lastInvokedMenuItem = matchedItem;
                }
            }
        }
    }
}
