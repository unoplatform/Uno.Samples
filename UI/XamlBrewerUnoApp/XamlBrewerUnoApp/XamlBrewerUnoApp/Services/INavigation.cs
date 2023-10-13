using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace XamlBrewerUnoApp.Services
{
    public interface INavigation
    {
        NavigationViewItem? GetCurrentNavigationViewItem();

        List<NavigationViewItem> GetNavigationViewItems();

        List<NavigationViewItem> GetNavigationViewItems(Type type);

        List<NavigationViewItem> GetNavigationViewItems(Type type, string title);

        void SetCurrentNavigationViewItem(NavigationViewItem item);

        void SetCurrentPage(Type type);
    }
}
