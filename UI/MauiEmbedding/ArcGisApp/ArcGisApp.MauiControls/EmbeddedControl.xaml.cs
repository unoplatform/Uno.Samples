using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Mapping.Floor;
using Esri.ArcGISRuntime.Maui;
using Map = Esri.ArcGISRuntime.Mapping.Map;

namespace ArcGisApp.MauiControls;

public partial class EmbeddedControl : ContentView
{
	public EmbeddedControl()
	{
		InitializeComponent();


        //// Create a map with 'Imagery with Labels' basemap and an initial location.
        //Map myMap = new Map(BasemapStyle.ArcGISImagery);
        //myMap.InitialViewpoint = new Viewpoint(-33.867886, -63.985, 25000);

        //// Assign the map to the MapView.
        //MyMapView.Map = myMap;
        _ = Initialize();
    }

    private FloorManager _floorManager;

    // Collection of floors.
    private readonly Dictionary<string, FloorLevel> _floorOptions = new Dictionary<string, FloorLevel>();

    private async Task Initialize()
    {
        try
        {
            // Gets the floor data from ArcGIS Online and creates a map with it.
            Map map = new Map(new Uri("https://ess.maps.arcgis.com/home/item.html?id=f133a698536f44c8884ad81f80b6cfc7"));

            mapView.Map = map;

            // Map needs to be loaded in order for floormanager to be used.
            await mapView.Map.LoadAsync();
            List<string> floorName = new List<string>();

            // Checks to see if the layer is floor aware.
            if (mapView.Map.FloorDefinition == null)
            {
                System.Diagnostics.Debug.WriteLine("The layer is not floor aware.");
                return;
            }

            await mapView.Map.FloorManager.LoadAsync();
            _floorManager = mapView.Map.FloorManager;

            // Use the dictionary to add the level's name as the key and the FloorLevel object with the associated level's name.
            foreach (FloorLevel level in _floorManager.Facilities[0].Levels)
            {
                _floorOptions.Add(level.ShortName, level);
                floorName.Add(level.ShortName);
            }

            //FloorChooser.ItemsSource = floorName;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

}