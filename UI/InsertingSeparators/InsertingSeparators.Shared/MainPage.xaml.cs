using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace InsertingSeparators
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<Bird> Birds;

        public MainPage()
        {
            this.InitializeComponent();

            Birds = new ObservableCollection<Bird> { new Bird { CommonName="Common Gull", ScientificName="Larus canus", Family="Gulls and terns"},
                new Bird { CommonName = "Robin", ScientificName = "Erithacus rubecula", Family = "Passerine" },
                new Bird { CommonName = "House Sparrow", ScientificName = "Passer domesticus", Family = "Passerine" },
                new Bird { CommonName = "Arctic Tern", ScientificName = "Sterna paradisaea", Family = "Gulls and terns" },
                new Bird { CommonName = "Tawny Owl", ScientificName = "Strix aluco", Family = "Owls" } };

            // We are not using it here but this handler means we will rebuild the menu if the collection changes.
            Birds.CollectionChanged += Birds_CollectionChanged;
            
            //Force complete rebuild for initial collection
            Birds_CollectionChanged(this, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }

        private void Birds_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                GenerateGroupedMenu();
                GenerateSeparatedMenu();
            });
        }

        private void GenerateGroupedMenu()
        {
            GroupedMenu.Items.Clear();

            var groupedBirds = Birds.GroupBy(x => x.Family);

            foreach (var group in groupedBirds)
            {
                var menuGroup = new MenuFlyoutSubItem() { Text = group.Key };
                GroupedMenu.Items.Add(menuGroup);

                foreach (var bird in group)
                {
                    var item = new MenuFlyoutItem() { Text = bird.CommonName };
                    item.DataContext = bird;
                    menuGroup.Items.Add(item);
                }
            }
        }

        private void GenerateSeparatedMenu()
        {
            SeparatedMenu.Items.Clear();
            var groupedBirds = Birds.GroupBy(x => x.Family);
            var template = Resources["MenuItemTemplate"] as DataTemplate;
            foreach (var group in groupedBirds)
            {
                foreach (var bird in group)
                {
                    var item = template.LoadContent() as MenuFlyoutItem;
                    item.DataContext = bird;
                    SeparatedMenu.Items.Add(item);
                }

                // add separator after last item in group
                SeparatedMenu.Items.Add(new MenuFlyoutSeparator());
            }

            if (SeparatedMenu.Items.Count > 0)
            {
                // remove trailing separator (if items added)
                SeparatedMenu.Items.RemoveAt(SeparatedMenu.Items.Count - 1);
            }
        }
    }
}
