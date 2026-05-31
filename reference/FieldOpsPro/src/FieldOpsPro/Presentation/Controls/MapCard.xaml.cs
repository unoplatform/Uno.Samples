using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using FieldOpsPro.Models;
using FieldOpsPro.Models.Enums;

namespace FieldOpsPro.Presentation.Controls;

public sealed partial class MapCard : UserControl
{
    private WritableLayer? _agentMarkersLayer;

    // Montreal area coordinates
    private const double MontrealLat = 45.5017;
    private const double MontrealLng = -73.5673;
    // Resolution limits to prevent white space / excessive zoom out
    private const double MinResolution = 0.5;      // closest possible
    private const double MaxResolution = 10000000; // farthest allowed (prevent white area)

    public MapCard()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        InitializeMap();
    }

    public static readonly DependencyProperty AgentsProperty =
        DependencyProperty.Register(nameof(Agents), typeof(IImmutableList<Agent>), typeof(MapCard),
            new PropertyMetadata(null, OnAgentsChanged));

    public IImmutableList<Agent>? Agents
    {
        get => (IImmutableList<Agent>?)GetValue(AgentsProperty);
        set => SetValue(AgentsProperty, value);
    }

    private static void OnAgentsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MapCard card && e.NewValue is IImmutableList<Agent> agents)
        {
            card.UpdateAgentMarkers(agents);
        }
    }

    private void InitializeMap()
    {
        try
        {
            // Use OpenStreetMap tiles
            var tileLayer = OpenStreetMap.CreateTileLayer();
            MapControl.Map.Layers.Add(tileLayer);

            // Create markers layer for agents
            _agentMarkersLayer = new WritableLayer
            {
                Name = "Agents",
                Style = null // We'll style individual features
            };
            MapControl.Map.Layers.Add(_agentMarkersLayer);

            // Set initial viewport to Montreal area
            var center = SphericalMercator.FromLonLat(MontrealLng, MontrealLat);
            MapControl.Map.Navigator.CenterOnAndZoomTo(new MPoint(center.x, center.y), 8000);

            // Add mock pins if no agents are available
            if (Agents == null || !Agents.Any())
            {
                AddMockPins();
            }
            // Update markers if we already have agents
            else
            {
                UpdateAgentMarkers(Agents);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Map initialization error: {ex.Message}");
        }
    }

    private void AddMockPins()
    {
        if (_agentMarkersLayer == null) return;

        try
        {
            _agentMarkersLayer.Clear();

            // Create mock agent locations around Montreal
            var mockLocations = new[]
            {
                // Downtown Montreal - On Route
                new { Lat = 45.5017, Lng = -73.5673, Status = AgentStatus.OnRoute, Name = "Agent 1" },
                // Old Montreal - On Site
                new { Lat = 45.5088, Lng = -73.5543, Status = AgentStatus.OnSite, Name = "Agent 2" },
                // Plateau Mont-Royal - Available
                new { Lat = 45.5200, Lng = -73.5800, Status = AgentStatus.Available, Name = "Agent 3" },
                // Mile End - On Route
                new { Lat = 45.5250, Lng = -73.6000, Status = AgentStatus.OnRoute, Name = "Agent 4" },
                // Westmount - On Site
                new { Lat = 45.4833, Lng = -73.5967, Status = AgentStatus.OnSite, Name = "Agent 5" },
                // Outremont - Available
                new { Lat = 45.5167, Lng = -73.6167, Status = AgentStatus.Available, Name = "Agent 6" },
                // Ville-Marie - On Route
                new { Lat = 45.4950, Lng = -73.5700, Status = AgentStatus.OnRoute, Name = "Agent 7" },
                // Rosemont - On Site
                new { Lat = 45.5430, Lng = -73.5900, Status = AgentStatus.OnSite, Name = "Agent 8" }
            };

            foreach (var location in mockLocations)
            {
                var point = SphericalMercator.FromLonLat(location.Lng, location.Lat);

                var feature = new PointFeature(new MPoint(point.x, point.y))
                {
                    ["AgentId"] = location.Name,
                    ["AgentName"] = location.Name,
                    ["Status"] = location.Status.ToString()
                };

                feature.Styles.Add(CreateMarkerStyle(location.Status));
                _agentMarkersLayer.Add(feature);
            }

            _agentMarkersLayer.DataHasChanged();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error adding mock pins: {ex.Message}");
        }
    }

    public void UpdateAgentMarkers(IEnumerable<Agent> agents)
    {
        if (_agentMarkersLayer == null) return;

        try
        {
            _agentMarkersLayer.Clear();

            foreach (var agent in agents.Where(a => a.Status != AgentStatus.Offline && a.Location != null))
            {
                var point = SphericalMercator.FromLonLat(
                    agent.Location.Longitude,
                    agent.Location.Latitude);

                var feature = new PointFeature(new MPoint(point.x, point.y))
                {
                    ["AgentId"] = agent.Id,
                    ["AgentName"] = agent.Name,
                    ["Status"] = agent.Status.ToString()
                };

                feature.Styles.Add(CreateMarkerStyle(agent.Status));
                _agentMarkersLayer.Add(feature);
            }

            _agentMarkersLayer.DataHasChanged();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating markers: {ex.Message}");
        }
    }

    private static IStyle CreateMarkerStyle(AgentStatus status)
    {
        // Marker colors come from the FieldOps design tokens. Mapsui has its own Color
        // type, so we resolve the theme Color and convert it.
        var color = status switch
        {
            AgentStatus.OnRoute => MarkerColor("AccentSecondary"),   // Silver/Gray for On Route
            AgentStatus.OnSite => MarkerColor("AccentPrimary"),      // White for On Site
            AgentStatus.Available => MarkerColor("AccentTertiary"),  // Gray for Available
            _ => MarkerColor("AccentTertiary")
        };

        return new SymbolStyle
        {
            SymbolScale = 0.8,
            Fill = new Mapsui.Styles.Brush(color),
            Outline = new Pen(MarkerColor("AccentPrimary"), 2)
        };
    }

    private static Mapsui.Styles.Color MarkerColor(string colorKey)
    {
        var c = Utils.ColorUtils.GetColor(colorKey);
        return new Mapsui.Styles.Color(c.R, c.G, c.B, c.A);
    }

    private void OnFocusButtonClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_agentMarkersLayer == null || !_agentMarkersLayer.GetFeatures().Any())
            {
                // If no markers, just center on Montreal
                var center = SphericalMercator.FromLonLat(MontrealLng, MontrealLat);
                MapControl.Map.Navigator.CenterOnAndZoomTo(new MPoint(center.x, center.y), 8000);
                return;
            }

            // Calculate the bounding box of all markers
            var features = _agentMarkersLayer.GetFeatures()
                .OfType<PointFeature>()
                .ToList();
            if (!features.Any()) return;

            var minX = features.Min(f => f.Point.X);
            var maxX = features.Max(f => f.Point.X);
            var minY = features.Min(f => f.Point.Y);
            var maxY = features.Max(f => f.Point.Y);

            // Calculate center point
            var centerX = (minX + maxX) / 2;
            var centerY = (minY + maxY) / 2;

            // Calculate the bounding box with viewport dimensions
            var width = maxX - minX;
            var height = maxY - minY;

            // Get the map control's actual dimensions
            var mapWidth = MapControl.ActualWidth;
            var mapHeight = MapControl.ActualHeight;

            if (mapWidth > 0 && mapHeight > 0)
            {
                // Calculate resolution needed for both dimensions (world units per pixel)
                var resolutionX = width / mapWidth;
                var resolutionY = height / mapHeight;

                // Use the larger resolution to ensure all markers fit
                var resolution = Math.Max(resolutionX, resolutionY);

                // Add small padding (10% total) so markers aren't on the very edge
                resolution *= 1.1;

                // Clamp resolution to prevent excessive zooming in or out
                if (resolution < MinResolution) resolution = MinResolution;
                if (resolution > MaxResolution) resolution = MaxResolution;

                // Center and zoom to the computed resolution (tightest fit)
                MapControl.Map.Navigator.CenterOnAndZoomTo(new MPoint(centerX, centerY), resolution);
            }
            else
            {
                // Fallback if dimensions not available
                var maxExtent = Math.Max(width, height);
                var resolution = maxExtent > 0 ? maxExtent * 1.1 : 8000;
                MapControl.Map.Navigator.CenterOnAndZoomTo(new MPoint(centerX, centerY), resolution);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error focusing on markers: {ex.Message}");
        }
    }
}
