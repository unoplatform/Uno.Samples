using System.Collections.ObjectModel;

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
            destinationType.Add(new DestinationType(Icon: "ms-appx:///TravelUI/Assets/beach.png", Description: "Beach"));
            destinationType.Add(new DestinationType(Icon: "ms-appx:///TravelUI/Assets/camping.png", Description: "Mountain"));
            destinationType.Add(new DestinationType(Icon: "ms-appx:///TravelUI/Assets/mountain.png", Description: "Camping"));

            // Popular destination
            popularDestination.Add(new PopularDestination(Picture: "ms-appx:///TravelUI/Assets/mykonos.jpeg", Name: "Mykonos", Location: "Chora, Greece", Price: "48$"));
            popularDestination.Add(new PopularDestination(Picture: "ms-appx:///TravelUI/Assets/venesia.png", Name: "Waterfort", Location: "Venesia, Italy", Price: "50$"));

            // Special for you
            specialForYou.Add(new SpecialForYou(Picture: "ms-appx:///TravelUI/Assets/nubian.png", Name: "Nubian Desert", Location: "Northeastern Sudan"));
            specialForYou.Add(new SpecialForYou(Picture: "ms-appx:///TravelUI/Assets/mykonos.jpeg", Name: "Mykonos", Location: "Chora, Greece"));
            specialForYou.Add(new SpecialForYou(Picture: "ms-appx:///TravelUI/Assets/venesia.png", Name: "Waterfort", Location: "Venesia, Italy"));

        }
    }
}
