using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace ListViewSample
{
    public sealed partial class MainPage : Page
    {
        public readonly ObservableCollection<WorkplaceFriend> Friends;
        private readonly Random _random;

        public MainPage()
        {
            this.InitializeComponent();
            Friends = new ObservableCollection<WorkplaceFriend>();
            _random = new Random();

            // Adds friend connection data to collection
            AddWorkplaceFriend("Luke", "Software Engineer");
            AddWorkplaceFriend("Josh", "Mechanical Engineer");
            AddWorkplaceFriend("Scarlett", "Civil Engineer");
            AddWorkplaceFriend("Hampton", "Manager");
            AddWorkplaceFriend("Tommie", "Software Engineer");
            AddWorkplaceFriend("Ash", "Firefighter");
        }

        private async void AddNewConnection_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            NewConnectionDialog dialog = new NewConnectionDialog();
            dialog.XamlRoot = this.XamlRoot;
            var res = await dialog.ShowAsync();
            if (res != ContentDialogResult.None)
            {
                AddWorkplaceFriend(dialog.ConnectionName, dialog.ConnectionOccupation);
            }
        }

        private void ListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            AlternateRowBackgroundColor((ListViewItem)args.ItemContainer);
        }

        private void AlternateRowBackgroundColor(ListViewItem itemContainer)
        {
            var dataItem = (WorkplaceFriend)FriendsList.ItemFromContainer(itemContainer);
            // Alternate color between each even and odd ListViewItem
            if ((Friends.IndexOf(dataItem)) % 2 == 0)
            {
                itemContainer.Background = App.Current.Resources["LayerFillColorDefaultBrush"] as SolidColorBrush;
            }
        }

        private void AddWorkplaceFriend(string name, string occupation)
        {
            Friends.Add(new WorkplaceFriend()
            {
                Name = name,
                Occupation = occupation,
                Id = _random.Next(999999)   // Generate a random Id for visualization purposes
            });
        }
    }
}