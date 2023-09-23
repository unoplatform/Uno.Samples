using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Mapsui;
using Mapsui.Animations;
using Mapsui.Extensions;
using Mapsui.Projections;
using Mapsui.Tiling;
using Mapsui.UI;
using Mapsui.Widgets.Zoom;
using Microsoft.UI.Input;
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
            MapControl.Map.Navigator.ViewportChanged += Navigator_ViewPortChanged;
        }

        private void Navigator_ViewPortChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this._updating)
                return;
            
            this._updating = true;

            this.DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    var resolutions = MapControl.Map.Navigator.Resolutions;
                    // find the closest resolution
                    var zoomLevel = 0;
                    for (var i = 0; i < resolutions.Count; i++)
                    {
                        if (resolutions[i] <= MapControl.Map.Navigator.Viewport.Resolution)
                        {
                            break;
                        }

                        zoomLevel = i;
                    }

                    zoomSlider.Value = zoomLevel;
                }
                finally
                {
                    this._updating = false;
                }
            });
        }

        private void MapControl_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            _currentPoint = e.GetCurrentPoint(this);
        }

        private async void ZoomSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (this._updating)
                return;

            try
            {
                _updating = true;

                var mousePosition = _currentPoint is null ? 
                    new MPoint(MapControl.ActualWidth / 2, MapControl.ActualHeight / 2) : 
                    new MPoint(_currentPoint.Position.X, _currentPoint.Position.Y);

                if (MapControl.Map == null)
                {
                    return;
                }

                var level = Convert.ToInt32(zoomSlider.Value) - 1;
                var resolution = MapControl.Map.Navigator.Resolutions[level];
                MapControl.Map.Navigator.ZoomTo(resolution, mousePosition, 100, MouseWheelAnimation.Easing);
            }
            finally
            {
                await Task.Delay(120); // animation duration + 20ms
                _updating = false;
            }
        }


        public MouseWheelAnimation MouseWheelAnimation { get; } = new MouseWheelAnimation { Duration = 0 };

        private Microsoft.UI.Input.PointerPoint _currentPoint;
        private bool _updating;
    }
}