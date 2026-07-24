using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using UnoCRM.Presentation.MockData;

namespace UnoCRM.Presentation;

/// <summary>
/// Backs the Contacts page. Filtering lives entirely in <see cref="ContactsModel"/> (states + a list
/// feed); this code-behind is responsible only for the Mapsui side-effect that can't data-bind. A
/// hidden probe list is bound to the model's <c>FilteredContacts</c> feed, so when the filters change
/// the feed re-emits and we rebuild the map layers from the current set.
/// </summary>
public sealed partial class ContactsPage : Page
{
    public ContactsPage()
    {
        this.InitializeComponent();

        // Design-time DataContext for Hot Design / Studio (seed on the page, never a child).
        // At runtime Uno.Extensions Navigation injects the generated ContactsModel VM.
        this.DataContext = ContactsPageMockData.Data;

        Loaded += OnLoaded;
    }

    private bool _initialFitDone;

    // OpenStreetMap's tile usage policy requires a descriptive User-Agent that identifies the app;
    // requests without a valid one are rejected. The framework-derived default is unreliable
    // (notably on iOS, where there is no conventional entry assembly), so identify the sample
    // explicitly to make tiles load on every platform.
    private const string TileUserAgent = "UnoCRM/1.0 (Uno Platform sample; +https://platform.uno)";

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Rebuild the maps whenever the bound filtered set changes. The list feed materializes
        // asynchronously, so the viewport is fit on the first rebuild that actually has contacts
        // (which may be a later VectorChanged, not this initial call); after that, filter changes
        // keep the user's current pan/zoom.
        ContactsProbe.Items.VectorChanged += (_, _) => RefreshMaps(fitViewport: TakeInitialFit());
        RefreshMaps(fitViewport: TakeInitialFit());
    }

    // True exactly once — for the first rebuild that has contacts — so the initial viewport
    // zoom-fits the pins even though the feed populates after Loaded.
    private bool TakeInitialFit()
    {
        if (_initialFitDone || CurrentContacts().Count == 0)
        {
            return false;
        }

        _initialFitDone = true;
        return true;
    }

    private IReadOnlyList<ContactLocation> CurrentContacts()
        => ContactsProbe.Items.OfType<ContactLocation>().ToList();

    private void RefreshMaps(bool fitViewport)
    {
        var contacts = CurrentContacts();
        UpdateMapForControl(DesktopMapControl, contacts, fitViewport);
        UpdateMapForControl(MobileMapControl, contacts, fitViewport);
    }

    // "Reset view" re-fits the visible map to the current results (a pure view operation).
    private void ResetView_Click(object sender, RoutedEventArgs e) => RefreshMaps(fitViewport: true);

    private static void UpdateMapForControl(Mapsui.UI.WinUI.MapControl? mapControl, IReadOnlyList<ContactLocation> contacts, bool fitViewport)
    {
        if (mapControl is null)
        {
            return;
        }

        mapControl.Map ??= CreateBaseMap();

        var map = mapControl.Map;
        if (map is null)
        {
            return;
        }

        if (!map.Layers.Any(layer => string.Equals(layer.Name, "BaseMap", StringComparison.Ordinal)))
        {
            var baseLayer = OpenStreetMap.CreateTileLayer(TileUserAgent);
            baseLayer.Name = "BaseMap";
            map.Layers.Insert(0, baseLayer);
        }

        map.Layers.Remove(layer => layer.Name == "ContactsLayer");

        var features = contacts.Select(CreateFeature).ToList();

        var memoryLayer = new MemoryLayer("ContactsLayer")
        {
            Features = features,
            Style = null,
        };

        map.Layers.Add(memoryLayer);

        if (fitViewport && memoryLayer.Extent is not null)
        {
            map.Navigator.ZoomToBox(memoryLayer.Extent, MBoxFit.Fit);
        }

        mapControl.ForceUpdate();
    }

    private static PointFeature CreateFeature(ContactLocation contact)
    {
        var projected = SphericalMercator.FromLonLat(contact.Longitude, contact.Latitude);

        var feature = new PointFeature(new MPoint(projected.x, projected.y));

        feature["Name"] = contact.Name;
        feature["Company"] = contact.Company;
        feature["Region"] = contact.Region;
        feature["Segment"] = contact.Segment;
        feature.Styles.Add(new SymbolStyle
        {
            SymbolScale = 0.75,
            Fill = new Mapsui.Styles.Brush(Color.FromString("#0D6E6E")),
            Outline = new Pen { Color = Color.FromString("#FFFFFF"), Width = 2 },
        });
        feature.Styles.Add(new LabelStyle { LabelColumn = "Name" });

        return feature;
    }

    private static Mapsui.Map CreateBaseMap()
    {
        var map = new Mapsui.Map();
        var baseLayer = OpenStreetMap.CreateTileLayer(TileUserAgent);
        baseLayer.Name = "BaseMap";
        map.Layers.Add(baseLayer);
        return map;
    }
}
