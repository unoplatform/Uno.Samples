using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

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
            AddWorkplaceFriend("Scarlett", "Architect");
            AddWorkplaceFriend("Hampton", "Manager");
            AddWorkplaceFriend("Tommie", "Software Engineer");
            AddWorkplaceFriend("Ash", "Firefighter");
        }

        private void ListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            // Alternate color between each even and odd ListViewItem
            if ((args.ItemIndex + 1) % 2 == 0)
            {
                args.ItemContainer.Background = App.Current.Resources["LayerFillColorDefaultBrush"] as SolidColorBrush;
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
