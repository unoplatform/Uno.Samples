using System;
using System.ComponentModel;
using Mapsui;
using Mapsui.Animations;
using Mapsui.Extensions;
using Mapsui.Projections;
using Mapsui.Tiling;
using Mapsui.UI;
using Mapsui.Widgets.Zoom;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MapControlSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            zoomSlider.ValueChanged += ZoomSlider_ValueChanged;

            MapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer()); 
            MapControl.Map.Widgets.Add(new ZoomInOutWidget { MarginX = 10, MarginY = 20 });
            MapControl.PointerMoved += MapControl_PointerMoved;
            var centerOfLondonOntario = new MPoint(-81.2497, 42.9837);

            var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(centerOfLondonOntario.X, centerOfLondonOntario.Y).ToMPoint();

            MapControl.Map.Home = n => n.CenterOnAndZoomTo(sphericalMercatorCoordinate, n.Resolutions[13]);
            lastViewPort = MapControl.Map.Navigator.Viewport;
            MapControl.Map.Navigator.ViewportChanged += Navigator_ViewPortChanged;
        }

        private void Navigator_ViewPortChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.updating)
                return;
            
            this.DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    this.updating = true;

                    var resolutions = MapControl.Map.Navigator.Resolutions;
                    // find the closest resolution
                    var zoomLevel = 0;
                    for (var i = 0; i < resolutions.Count; i++)
                    {
                        if (resolutions[i] < MapControl.Map.Navigator.Viewport.Resolution)
                        {
                            break;
                        }

                        zoomLevel = i;
                    }

                    zoomSlider.Value = zoomLevel;
                }
                finally
                {
                    this.updating = false;
                }
            });
        }

        private void MapControl_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            _currentPoint = e.GetCurrentPoint(this);
        }

        private void ZoomSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (this.updating)
                return;

            try
            {
                if(_currentPoint is null)
                {
                    return;
                }
                var mousePosition = new MPoint(_currentPoint.Position.X, _currentPoint.Position.Y);

                if (MapControl.Map == null)
                {
                    return;
                }
                
                MapControl.Map.Navigator.ZoomToLevel(Convert.ToInt32(zoomSlider.Value));
            }
            finally
            {
                this.updating = false;
            }
        }


        public MouseWheelAnimation MouseWheelAnimation { get; } = new MouseWheelAnimation { Duration = 0 };

        private Microsoft.UI.Input.PointerPoint _currentPoint;
        private Viewport lastViewPort;
        private bool updating;
    }
}