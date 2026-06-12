using System.Collections.ObjectModel;
using System.ComponentModel;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;

namespace UnoCRM;

public sealed partial class ContactsPage : Page, INotifyPropertyChanged
{
    private const string AllRegions = "All Regions";
    private const string AllSegments = "All Segments";

    private readonly List<ContactLocation> _allContacts = [];
    private string _searchQuery = string.Empty;
    private string _regionFilter = AllRegions;
    private string _segmentFilter = AllSegments;
    private bool _initialized;
    private bool _syncing;

    public ContactsPage()
    {
        InitializeComponent();
        BuildSampleContacts();

        // Filter options are derived from the contact data (not hardcoded), with an
        // "All …" entry first. Populated before DataContext is set so the bound
        // ComboBoxes pick them up.
        Regions = new[] { AllRegions }
            .Concat(_allContacts.Select(c => c.Region).Distinct(StringComparer.OrdinalIgnoreCase))
            .ToArray();
        Segments = new[] { AllSegments }
            .Concat(_allContacts.Select(c => c.Segment).Distinct(StringComparer.OrdinalIgnoreCase))
            .ToArray();

        DataContext = this;
        Loaded += ContactsPage_Loaded;
    }

    public IReadOnlyList<string> Regions { get; }

    public IReadOnlyList<string> Segments { get; }

    private void ContactsPage_Loaded(object sender, RoutedEventArgs e)
    {
        // ItemsSource is bound after InitializeComponent, so apply the initial selection
        // here — a parse-time SelectedIndex can be coerced to -1 on the then-empty combo
        // and is not re-applied when the items arrive (behavior varies per target).
        DesktopRegionFilter.SelectedIndex = 0;
        DesktopSegmentFilter.SelectedIndex = 0;
        MobileRegionFilter.SelectedIndex = 0;
        MobileSegmentFilter.SelectedIndex = 0;
        _initialized = true;

        ApplyFiltersAndRefreshMap(resetViewport: true);
    }

    public ObservableCollection<ContactLocation> FilteredContacts { get; } = [];

    public string TotalFilteredLabel => $"{FilteredContacts.Count} contacts";

    public string RegionsLabel
    {
        get
        {
            var count = FilteredContacts.Select(x => x.Region).Distinct(StringComparer.OrdinalIgnoreCase).Count();
            return $"{count} regions";
        }
    }

    public string SegmentsLabel
    {
        get
        {
            var count = FilteredContacts.Select(x => x.Segment).Distinct(StringComparer.OrdinalIgnoreCase).Count();
            return $"{count} segments";
        }
    }

    private void BuildSampleContacts()
    {
        var random = new Random();

        AddRegionalContacts("North America", "Enterprise", "Seattle", 47.6062, -122.3321, 8, random);
        AddRegionalContacts("North America", "Mid-Market", "New York", 40.7128, -74.0060, 8, random);
        AddRegionalContacts("North America", "SMB", "Toronto", 43.6532, -79.3832, 7, random);

        AddRegionalContacts("Europe", "Enterprise", "London", 51.5074, -0.1278, 8, random);
        AddRegionalContacts("Europe", "Mid-Market", "Berlin", 52.5200, 13.4050, 7, random);
        AddRegionalContacts("Europe", "SMB", "Madrid", 40.4168, -3.7038, 6, random);

        AddRegionalContacts("APAC", "Enterprise", "Singapore", 1.3521, 103.8198, 8, random);
        AddRegionalContacts("APAC", "Mid-Market", "Tokyo", 35.6762, 139.6503, 7, random);
        AddRegionalContacts("APAC", "SMB", "Sydney", -33.8688, 151.2093, 6, random);

        AddRegionalContacts("LATAM", "Enterprise", "Sao Paulo", -23.5505, -46.6333, 7, random);
        AddRegionalContacts("LATAM", "Mid-Market", "Mexico City", 19.4326, -99.1332, 7, random);
        AddRegionalContacts("LATAM", "SMB", "Bogota", 4.7110, -74.0721, 6, random);
    }

    private void AddRegionalContacts(
        string region,
        string segment,
        string city,
        double centerLat,
        double centerLon,
        int count,
        Random random)
    {
        var firstNames = new[] { "Alex", "Jordan", "Casey", "Morgan", "Taylor", "Jamie", "Riley", "Avery", "Cameron", "Harper", "Dakota", "Quinn", "Reese", "Skyler", "Logan" };
        var lastNames = new[] { "Chen", "Patel", "Garcia", "Kim", "Ross", "Rivera", "Brooks", "Nguyen", "Singh", "Wright", "Davis", "Lopez", "Adams", "Walker", "Wilson" };
        var companyStarts = new[] { "North", "Blue", "Summit", "Vertex", "Clear", "Nova", "Prime", "Vector", "Cloud", "Bright", "Atlas", "Global" };
        var companyEnds = new[] { "Analytics", "Systems", "Dynamics", "Logistics", "Health", "Finance", "Retail", "Labs", "Energy", "Media", "Networks", "Advisors" };

        for (var i = 0; i < count; i++)
        {
            var latitude = centerLat + (random.NextDouble() - 0.5) * 2.5;
            var longitude = centerLon + (random.NextDouble() - 0.5) * 2.5;
            var name = $"{firstNames[random.Next(firstNames.Length)]} {lastNames[random.Next(lastNames.Length)]}";
            var company = $"{companyStarts[random.Next(companyStarts.Length)]} {companyEnds[random.Next(companyEnds.Length)]}";

            _allContacts.Add(new ContactLocation(name, company, city, region, segment, latitude, longitude));
        }
    }

    private void SearchContacts_TextChanged(object sender, TextChangedEventArgs e)
    {
        var desktopText = DesktopSearchBox?.Text?.Trim() ?? string.Empty;
        var mobileText = MobileSearchBox?.Text?.Trim() ?? string.Empty;

        if (sender == DesktopSearchBox && MobileSearchBox is not null && MobileSearchBox.Text != desktopText)
        {
            MobileSearchBox.Text = desktopText;
        }

        if (sender == MobileSearchBox && DesktopSearchBox is not null && DesktopSearchBox.Text != mobileText)
        {
            DesktopSearchBox.Text = mobileText;
        }

        _searchQuery = sender == DesktopSearchBox ? desktopText : mobileText;
        ApplyFiltersAndRefreshMap(resetViewport: false);
    }

    private void RegionFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Skip while applying the initial selection, and while mirroring to the counterpart
        // layout (the mirror raises its own SelectionChanged) so a change refreshes once.
        if (!_initialized || _syncing)
        {
            return;
        }

        var selected = GetComboSelection(sender as ComboBox);
        if (string.IsNullOrWhiteSpace(selected))
        {
            return;
        }

        _regionFilter = selected;

        _syncing = true;
        SyncComboSelection(DesktopRegionFilter, MobileRegionFilter, selected, sender as ComboBox);
        _syncing = false;

        ApplyFiltersAndRefreshMap(resetViewport: false);
    }

    private void SegmentFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_initialized || _syncing)
        {
            return;
        }

        var selected = GetComboSelection(sender as ComboBox);
        if (string.IsNullOrWhiteSpace(selected))
        {
            return;
        }

        _segmentFilter = selected;

        _syncing = true;
        SyncComboSelection(DesktopSegmentFilter, MobileSegmentFilter, selected, sender as ComboBox);
        _syncing = false;

        ApplyFiltersAndRefreshMap(resetViewport: false);
    }

    private static string GetComboSelection(ComboBox? combo) =>
        combo?.SelectedItem as string ?? string.Empty;

    private static void SyncComboSelection(ComboBox? first, ComboBox? second, string value, ComboBox? sender)
    {
        if (first is not null && first != sender)
        {
            SetComboSelection(first, value);
        }

        if (second is not null && second != sender)
        {
            SetComboSelection(second, value);
        }
    }

    private static void SetComboSelection(ComboBox combo, string value)
    {
        if (!Equals(combo.SelectedItem as string, value))
        {
            combo.SelectedItem = value;
        }
    }

    private void ApplyFiltersAndRefreshMap(bool resetViewport)
    {
        var filtered = _allContacts
            .Where(x => _regionFilter == AllRegions || x.Region.Equals(_regionFilter, StringComparison.OrdinalIgnoreCase))
            .Where(x => _segmentFilter == AllSegments || x.Segment.Equals(_segmentFilter, StringComparison.OrdinalIgnoreCase))
            .Where(x => string.IsNullOrWhiteSpace(_searchQuery)
                || x.Name.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase)
                || x.Company.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase)
                || x.City.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase))
            .ToList();

        FilteredContacts.Clear();
        foreach (var contact in filtered)
        {
            FilteredContacts.Add(contact);
        }

        UpdateMapForControl(DesktopMapControl, resetViewport);
        UpdateMapForControl(MobileMapControl, resetViewport);

        NotifyMetricsChanged();
    }

    private void UpdateMapForControl(Mapsui.UI.WinUI.MapControl? mapControl, bool resetViewport)
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
            var baseLayer = OpenStreetMap.CreateTileLayer();
            baseLayer.Name = "BaseMap";
            map.Layers.Insert(0, baseLayer);
        }

        map.Layers.Remove(layer => layer.Name == "ContactsLayer");

        var features = FilteredContacts
            .Select(CreateFeature)
            .ToList();

        var memoryLayer = new MemoryLayer("ContactsLayer")
        {
            Features = features,
            Style = null
        };

        map.Layers.Add(memoryLayer);

        if (resetViewport && memoryLayer.Extent is not null)
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
            Outline = new Pen { Color = Color.FromString("#FFFFFF"), Width = 2 }
        });
        feature.Styles.Add(new LabelStyle { LabelColumn = "Name" });

        return feature;
    }

    private static Mapsui.Map CreateBaseMap()
    {
        var map = new Mapsui.Map();
        var baseLayer = OpenStreetMap.CreateTileLayer();
        baseLayer.Name = "BaseMap";
        map.Layers.Add(baseLayer);
        return map;
    }

    private void ResetMap_Click(object sender, RoutedEventArgs e)
    {
        ApplyFiltersAndRefreshMap(resetViewport: true);
    }

    private void NotifyMetricsChanged()
    {
        OnPropertyChanged(nameof(TotalFilteredLabel));
        OnPropertyChanged(nameof(RegionsLabel));
        OnPropertyChanged(nameof(SegmentsLabel));
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void NavigateToDashboard_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(MainPage));
    }

    private void NavigateToPipeline_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(PipelinePage));
    }

    private void NavigateToLeads_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(LeadsPage));
    }

    public sealed record ContactLocation(
        string Name,
        string Company,
        string City,
        string Region,
        string Segment,
        double Latitude,
        double Longitude);
}
