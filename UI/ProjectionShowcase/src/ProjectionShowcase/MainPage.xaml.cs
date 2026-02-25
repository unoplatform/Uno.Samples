using Microsoft.UI.Xaml.Media.Media3D;

namespace ProjectionShowcase;

public sealed partial class MainPage : Page
{
    private readonly DispatcherTimer _timer;
    private double _elapsed;

    private Border[] _cards;
    private PlaneProjection[] _cardProjs;
    private PlaneProjection[] _tileProjs;
    private PlaneProjection[] _ringProjs;
    private Matrix3DProjection[] _waveProjs;

    public MainPage()
    {
        this.InitializeComponent();

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        _timer.Tick += OnTick;

        this.Loaded += OnLoaded;
        this.Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _cards = new[] { Card0, Card1, Card2, Card3, Card4 };
        _cardProjs = new[] { CardProj0, CardProj1, CardProj2, CardProj3, CardProj4 };
        _tileProjs = new[] { TileProj0, TileProj1, TileProj2, TileProj3, TileProj4, TileProj5 };
        _ringProjs = new[] { RingProj0, RingProj1, RingProj2, RingProj3, RingProj4, RingProj5, RingProj6, RingProj7 };
        _waveProjs = new[] { WaveProj0, WaveProj1, WaveProj2, WaveProj3, WaveProj4, WaveProj5, WaveProj6, WaveProj7, WaveProj8, WaveProj9 };

        _elapsed = 0;
        _timer.Start();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _timer.Stop();
    }

    private void OnTick(object sender, object e)
    {
        _elapsed += 0.016;
        UpdateCarousel(_elapsed);
        UpdateFloatingTiles(_elapsed);
        UpdateFlipCard(_elapsed);
        UpdateTunnel(_elapsed);
        UpdateWave(_elapsed);
    }

    /// <summary>
    /// Section 1: Five gradient cards orbiting in a ring.
    /// Each card is offset by 72 degrees. RotationY makes the card face
    /// outward, GlobalOffsetX/Z position it on the circular path,
    /// and Canvas.ZIndex ensures correct front-to-back ordering.
    /// </summary>
    private void UpdateCarousel(double t)
    {
        const double speed = 40.0;   // degrees per second
        const double radiusX = 200.0;
        const double radiusZ = 150.0;

        for (int i = 0; i < 5; i++)
        {
            double angleDeg = (speed * t + i * 72.0) % 360.0;
            double angleRad = angleDeg * Math.PI / 180.0;

            _cardProjs[i].RotationY = angleDeg;
            _cardProjs[i].GlobalOffsetX = radiusX * Math.Sin(angleRad);
            _cardProjs[i].GlobalOffsetZ = radiusZ * Math.Cos(angleRad);

            // Cards closer to the viewer (higher cos) render on top
            Canvas.SetZIndex(_cards[i], (int)(100 * Math.Cos(angleRad)));
        }
    }

    /// <summary>
    /// Section 2: Six colored tiles oscillating on different rotation
    /// axes with sine/cosine at varying speeds and phases.
    /// LocalOffsetZ adds a depth-breathing effect.
    /// </summary>
    private void UpdateFloatingTiles(double t)
    {
        for (int i = 0; i < 6; i++)
        {
            double phase = i * Math.PI / 3.0;
            double speed = 1.0 + i * 0.3;

            _tileProjs[i].RotationX = 25.0 * Math.Sin(t * speed + phase);
            _tileProjs[i].RotationY = 20.0 * Math.Cos(t * speed * 0.7 + phase);
            _tileProjs[i].RotationZ = 12.0 * Math.Sin(t * speed * 0.5 + phase * 2.0);
            _tileProjs[i].LocalOffsetZ = 40.0 * Math.Sin(t * 0.8 + phase);
        }
    }

    /// <summary>
    /// Section 3: Continuous flip between front and back faces.
    /// RotationY drives the flip; visibility swaps at 90 and 270 degrees
    /// so only the viewer-facing side is shown. The back face is offset
    /// by 180 degrees so its text reads correctly when revealed.
    /// </summary>
    private void UpdateFlipCard(double t)
    {
        double angle = (t * 60.0) % 360.0; // 60 deg/s → full flip every 6s

        FrontProj.RotationY = angle;
        BackProj.RotationY = angle + 180.0;

        bool showFront = angle < 90.0 || angle > 270.0;
        FrontFace.Visibility = showFront ? Visibility.Visible : Visibility.Collapsed;
        BackFace.Visibility = showFront ? Visibility.Collapsed : Visibility.Visible;
    }

    /// <summary>
    /// Section 4: Eight concentric rings spinning at increasing speeds
    /// in alternating directions. LocalOffsetZ pushes deeper rings
    /// further back, and a subtle sine oscillation adds a breathing pulse.
    /// </summary>
    private void UpdateTunnel(double t)
    {
        for (int i = 0; i < 8; i++)
        {
            double direction = (i % 2 == 0) ? 1.0 : -1.0;
            double speed = 30.0 + i * 15.0;

            _ringProjs[i].RotationZ = (t * speed * direction) % 360.0;
            _ringProjs[i].LocalOffsetZ = -i * 50.0 - 20.0 * Math.Sin(t * 2.0 + i * 0.5);
        }
    }

    /// <summary>
    /// Section 5: Ten tiles in a row, each receiving a custom Y-rotation
    /// Matrix3D with perspective (M34). A sine wave with per-tile phase
    /// offset creates a traveling ripple effect.
    /// </summary>
    private void UpdateWave(double t)
    {
        for (int i = 0; i < 10; i++)
        {
            double phase = i * 0.6;
            double angleDeg = 30.0 * Math.Sin(t * 2.5 + phase);
            double angleRad = angleDeg * Math.PI / 180.0;

            double cos = Math.Cos(angleRad);
            double sin = Math.Sin(angleRad);

            // Y-rotation matrix with perspective via M34
            _waveProjs[i].ProjectionMatrix = new Matrix3D(
                cos, 0, -sin, 0,
                0, 1, 0, 0,
                sin, 0, cos, -0.002,
                0, 0, 0, 1
            );
        }
    }
}
