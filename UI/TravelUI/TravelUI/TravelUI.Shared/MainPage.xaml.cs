using Android.Graphics;
using Android.Locations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static Android.Icu.Text.CaseMap;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TravelUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public readonly ObservableCollection<DestinationType> destinationType;
        public readonly ObservableCollection<PopularDestination> popularDestination;
        public readonly ObservableCollection<SpecialForYou> specialForYou;

        public MainPage()
        {
            this.InitializeComponent();

            destinationType = new ObservableCollection<DestinationType>();
            popularDestination = new ObservableCollection<PopularDestination>();
            specialForYou = new ObservableCollection<SpecialForYou>();


            GenerateInfo();
        }

        public void GenerateInfo()
        {
            // Destination Type
            destinationType.Add(new DestinationType { Icon = "beach", Description = "Beach" });
            destinationType.Add(new DestinationType { Icon = "camping", Description = "Mountain" });
            destinationType.Add(new DestinationType { Icon = "mountain", Description = "Camping" });

            // Popular destination
            popularDestination.Add(new PopularDestination { Picture = "mykonos", Name = "Mykonos", Location = "Chora, Greece", Price = "48$" });
            popularDestination.Add(new PopularDestination { Picture = "venesia", Name = "Waterfort", Location = "Venesia, Italy", Price = "50$" });

            // Special for you
            specialForYou.Add(new SpecialForYou { Picture = "nubian", Name = "Nubian Desert", Location = "Northeastern Sudan" });
            specialForYou.Add(new SpecialForYou { Picture = "mykonos", Name = "Mykonos", Location = "Chora, Greece" });
            specialForYou.Add(new SpecialForYou { Picture = "venesia", Name = "Waterfort", Location = "Venesia, Italy" });

        }
    }
}
