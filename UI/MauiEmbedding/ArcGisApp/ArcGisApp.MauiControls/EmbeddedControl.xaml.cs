using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Map = Esri.ArcGISRuntime.Mapping.Map;

namespace ArcGisApp.MauiControls;

public partial class EmbeddedControl : ContentView
{
	public EmbeddedControl()
	{
		InitializeComponent();

        var myMap = new Map()
        {
            InitialViewpoint = new Viewpoint(new Envelope(-180, -85, 180, 85, SpatialReferences.Wgs84)),
            Basemap = new Basemap(new Uri("https://arcgis.com/home/item.html?id=86265e5a4bbb4187a59719cf134e0018"))
        };

        // Assign the map to the MapView.
        mapView.Map = myMap;
    }
}