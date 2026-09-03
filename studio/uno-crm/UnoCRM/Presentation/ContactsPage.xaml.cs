using System.Collections;
using System.Collections.Specialized;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;

namespace UnoCRM.Presentation;

/// <summary>
/// Backs the Contacts page. Filtering lives entirely in <see cref="ContactsModel"/> (states + a list
/// feed); this code-behind is responsible only for the Mapsui side-effect that can't data-bind. The
/// page binds the model's <c>FilteredContacts</c> straight to its own <see cref="ContactsSource"/>
/// property, so when the filters change the feed re-emits and we rebuild the map layers from the
/// current set.
/// </summary>
public sealed partial class ContactsPage : Page
{
    public ContactsPage()
    {
        this.InitializeComponent();

        Loaded += OnLoaded;
    }

    private bool _initialFitDone;
    private bool _isLoaded;
    private INotifyCollectionChanged? _observedContacts;

    // OpenStreetMap's tile usage policy requires a descriptive User-Agent that identifies the app;
    // requests without a valid one are rejected. The framework-derived default is unreliable
    // (notably on iOS, where there is no conventional entry assembly), so identify the sample
    // explicitly to make tiles load on every platform.
    private const string TileUserAgent = "UnoCRM/1.0 (Uno Platform sample; +https://platform.uno)";

    public static readonly DependencyProperty ContactsSourceProperty =
        DependencyProperty.Register(
            nameof(ContactsSource),
            typeof(object),
            typeof(ContactsPage),
            new PropertyMetadata(null, static (page, args) => ((ContactsPage)page).OnContactsSourceChanged(args.NewValue)));

    /// <summary>
    /// The model's filtered contact list, bound on the page itself in XAML. The generated view-model
    /// hands over a bindable list proxy — an observable collection view — so the page can read the
    /// current contacts and be told when a filter change re-emits them, with no placeholder list
    /// control in the visual tree. Typed as <see cref="object"/> (like <c>ItemsSource</c>) so the
    /// binding resolves against whichever view-model is the current DataContext.
    /// </summary>
    public object? ContactsSource
    {
        get => GetValue(ContactsSourceProperty);
        set => SetValue(ContactsSourceProperty, value);
    }

    // Fires when the bound proxy first resolves, and again if the DataContext is replaced — Uno
    // Extensions Navigation swaps the design-time mock for the real view-model — so the subscription
    // always follows the live source instead of stranding on a stale one. Swapping (rather than
    // adding) is what keeps handlers from stacking; the proxy shares the view-model's per-navigation
    // lifetime, so it dies with the page and needs no explicit unwiring.
    private void OnContactsSourceChanged(object? source)
    {
        if (_observedContacts is not null)
        {
            _observedContacts.CollectionChanged -= OnContactsChanged;
        }

        _observedContacts = source as INotifyCollectionChanged;

        if (_observedContacts is not null)
        {
            _observedContacts.CollectionChanged += OnContactsChanged;
        }

        // Before Loaded the map controls can't render yet; OnLoaded does the first rebuild.
        if (_isLoaded)
        {
            RefreshMaps(fitViewport: TakeInitialFit());
        }
    }

    // Rebuild the maps whenever the filtered set changes.
    private void OnContactsChanged(object? sender, NotifyCollectionChangedEventArgs args)
        => RefreshMaps(fitViewport: TakeInitialFit());

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _isLoaded = true;

        // The list feed materializes asynchronously, so the viewport is fit on the first rebuild that
        // actually has contacts (which may be a later collection change, not this initial call);
        // after that, filter changes keep the user's current pan/zoom.
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
        => ContactsSource is IEnumerable contacts
            ? contacts.OfType<ContactLocation>().ToList()
            : [];

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
        // No per-pin LabelStyle: labelling all 85 points overlaps into an unreadable mess.
        // The contact names live in the filtered list beside the map instead.

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
