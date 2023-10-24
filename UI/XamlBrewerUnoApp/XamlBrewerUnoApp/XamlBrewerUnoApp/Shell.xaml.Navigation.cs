namespace XamlBrewerUnoApp
{
    public sealed partial class Shell : INavigation
    {
        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            SetCurrentNavigationViewItem(GetNavigationViewItems(typeof(HomePage)).First());
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            SetCurrentNavigationViewItem(args.SelectedItemContainer as NavigationViewItem);
        }

        public List<NavigationViewItem> GetNavigationViewItems()
        {
            List<NavigationViewItem> result = new();
            var items = NavigationView.MenuItems.Select(i => (NavigationViewItem)i).ToList();
            items.AddRange(NavigationView.FooterMenuItems.Select(i => (NavigationViewItem)i));
            result.AddRange(items);

            foreach (NavigationViewItem mainItem in items)
            {
                result.AddRange(mainItem.MenuItems.Select(i => (NavigationViewItem)i));
            }

            return result;
        }

        public List<NavigationViewItem> GetNavigationViewItems(Type type)
        {
            return GetNavigationViewItems().Where(i => i.Tag.ToString() == type.FullName).ToList();
        }

        public List<NavigationViewItem> GetNavigationViewItems(Type type, string title)
        {
            return GetNavigationViewItems(type).Where(ni => ni.Content.ToString() == title).ToList();
        }

        public void SetCurrentNavigationViewItem(NavigationViewItem? item)
        {
            if (item?.Tag is null)
            {
                return;
            }

            ContentFrame.Navigate(Type.GetType(item.Tag.ToString() ?? string.Empty), item.Content);
            NavigationView.Header = item.Content;
            NavigationView.SelectedItem = item;
        }

        public NavigationViewItem? GetCurrentNavigationViewItem()
        {
            return NavigationView.SelectedItem as NavigationViewItem;
        }

        public void SetCurrentPage(Type type)
        {
            ContentFrame.Navigate(type);
        }
    }
}
