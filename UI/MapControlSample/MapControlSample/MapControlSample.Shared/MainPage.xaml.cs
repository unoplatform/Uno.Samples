using Mapsui;
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

            MapControl.PointerWheelChanged += MapControl_PointerWheelChanged;
            MapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
            MapControl.Map.Widgets.Add(new ZoomInOutWidget { MarginX = 10, MarginY = 20 });
            MapControl.PointerMoved += MapControl_PointerMoved;
            var centerOfLondonOntario = new MPoint(-81.2497, 42.9837);

            var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(centerOfLondonOntario.X, centerOfLondonOntario.Y).ToMPoint();

            MapControl.Map.Home = n => n.NavigateTo(sphericalMercatorCoordinate, MapControl.Map.Resolutions[13]);
        }

        private void MapControl_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            _currentPoint = e.GetCurrentPoint(this);
        }

        private void ZoomSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var mousePosition = new MPoint(_currentPoint.Position.X, _currentPoint.Position.Y);

            if (MapControl.Map == null)
            {
                return;
            }
            if (e.NewValue > e.OldValue)
            {
                MapControl.Navigator?.ZoomIn(mousePosition, 100, MouseWheelAnimation.Easing);
            }
            else
            {
                MapControl.Navigator?.ZoomOut(mousePosition, 100, MouseWheelAnimation.Easing);
            }
        }

        private void MapControl_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (MapControl.Map?.ZoomLock ?? true)
            {
                return;
            }

            if (!Viewport.HasSize())
            {
                return;
            }

            if (_currentPoint is null)
            {
                _currentPoint = e.GetCurrentPoint(this);
            }

            var mousePosition = new MPoint(_currentPoint.Position.X, _currentPoint.Position.Y);

            var resolution = MouseWheelAnimation.GetResolution(_currentPoint.Properties.MouseWheelDelta, _viewport, MapControl.Map);

            if (MapControl.Map == null)
            {
                return;
            }

            resolution = MapControl.Map.Limiter.LimitResolution(resolution, Viewport.Width, Viewport.Height, MapControl.Map.Resolutions, MapControl.Map.Extent);
            MapControl.Navigator?.ZoomTo(resolution, mousePosition, 100, MouseWheelAnimation.Easing);

            e.Handled = true;
        }

        public MouseWheelAnimation MouseWheelAnimation { get; } = new MouseWheelAnimation { Duration = 0 };

        private Microsoft.UI.Input.PointerPoint _currentPoint;

        /// <summary>
        /// Viewport holding information about visible part of the map. Viewport can never be null.
        /// </summary>
        public IReadOnlyViewport Viewport => _viewport;

        private readonly LimitedViewport _viewport = new LimitedViewport();
    }
}