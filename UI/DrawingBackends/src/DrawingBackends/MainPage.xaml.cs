using System.Numerics;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.UI;

namespace DrawingBackends;

public sealed partial class MainPage : Page
{
    private readonly System.Diagnostics.Stopwatch _clock = System.Diagnostics.Stopwatch.StartNew();
    private long _frames;
    private long _framesInWindow;
    private TimeSpan _windowStart;

    public MainPage()
    {
        this.InitializeComponent();
        BackendBadge.Text = BackendInfo.Badge;
        CompositionTarget.Rendering += OnRendering;
        Loaded += (_, _) => Rebuild();
        SizeChanged += (_, _) => Rebuild();
    }

    private void OnRendering(object? sender, object e)
    {
        _frames++;
        _framesInWindow++;
        var elapsed = _clock.Elapsed - _windowStart;
        if (elapsed.TotalMilliseconds >= 500)
        {
            FpsText.Text = (_framesInWindow / elapsed.TotalSeconds).ToString("0");
            FrameText.Text = _frames.ToString("N0");
            _framesInWindow = 0;
            _windowStart = _clock.Elapsed;
        }
    }

    private void OnCountChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        CountText.Text = ((int)e.NewValue).ToString();
        if (IsLoaded)
        {
            Rebuild();
        }
    }

    private void OnAnimateToggled(object sender, RoutedEventArgs e) => Rebuild();

    private void Rebuild()
    {
        if (ActualWidth < 1 || ActualHeight < 1)
        {
            return;
        }

        Scene.Children.Clear();
        var count = (int)CountSlider.Value;
        CountText.Text = count.ToString();
        var animate = AnimateToggle.IsOn;
        Random rng = new(70);
        var w = ActualWidth;
        var h = ActualHeight;

        for (var i = 0; i < count; i++)
        {
            var size = 28 + rng.NextDouble() * rng.NextDouble() * 150;
            var hue = (i * 360.0 / 24 + rng.NextDouble() * 30) % 360;
            var a = FromHsv(hue, 0.75, 1.0, 0xE0);
            var b = FromHsv((hue + 50) % 360, 0.85, 0.85, 0x40);

            var brush = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
            brush.GradientStops.Add(new GradientStop { Color = a, Offset = 0 });
            brush.GradientStops.Add(new GradientStop { Color = b, Offset = 1 });

            UIElement shape;
            if (i % 3 == 0)
            {
                shape = new Ellipse { Width = size, Height = size, Fill = brush };
            }
            else
            {
                shape = new Border
                {
                    Width = size,
                    Height = size * (0.6 + rng.NextDouble() * 0.6),
                    CornerRadius = new CornerRadius(size * 0.28),
                    Background = brush,
                    BorderBrush = new SolidColorBrush(Color.FromArgb(0x50, 255, 255, 255)),
                    BorderThickness = new Thickness(1),
                };
            }

            Canvas.SetLeft(shape, rng.NextDouble() * (w - size));
            Canvas.SetTop(shape, rng.NextDouble() * (h - size));
            Scene.Children.Add(shape);

            var visual = ElementCompositionPreview.GetElementVisual(shape);
            visual.CenterPoint = new Vector3((float)size / 2, (float)size / 2, 0);
            visual.RotationAngleInDegrees = (float)(rng.NextDouble() * 360);
            var spinSeconds = 6 + rng.NextDouble() * 14;
            var pulseSeconds = 2 + rng.NextDouble() * 4;
            var peak = 1.2f + (float)rng.NextDouble() * 0.5f;
            var dir = rng.Next(2) == 0 ? 1 : -1;
            if (!animate)
            {
                continue;
            }

            var compositor = visual.Compositor;
            var start = visual.RotationAngleInDegrees;
            var spin = compositor.CreateScalarKeyFrameAnimation();
            spin.InsertKeyFrame(0f, start);
            spin.InsertKeyFrame(1f, start + 360 * dir, compositor.CreateLinearEasingFunction());
            spin.Duration = TimeSpan.FromSeconds(spinSeconds);
            spin.IterationBehavior = AnimationIterationBehavior.Forever;
            visual.StartAnimation("RotationAngleInDegrees", spin);

            var ease = compositor.CreateCubicBezierEasingFunction(new Vector2(0.4f, 0f), new Vector2(0.2f, 1f));
            var pulse = compositor.CreateVector3KeyFrameAnimation();
            pulse.InsertKeyFrame(0f, Vector3.One);
            pulse.InsertKeyFrame(0.5f, new Vector3(peak, peak, 1), ease);
            pulse.InsertKeyFrame(1f, Vector3.One, ease);
            pulse.Duration = TimeSpan.FromSeconds(pulseSeconds);
            pulse.IterationBehavior = AnimationIterationBehavior.Forever;
            visual.StartAnimation("Scale", pulse);
        }
    }

    private static Color FromHsv(double h, double s, double v, byte alpha)
    {
        var c = v * s;
        var x = c * (1 - Math.Abs(h / 60 % 2 - 1));
        var m = v - c;
        var (r, g, b) = (int)(h / 60) switch
        {
            0 => (c, x, 0d),
            1 => (x, c, 0d),
            2 => (0d, c, x),
            3 => (0d, x, c),
            4 => (x, 0d, c),
            _ => (c, 0d, x),
        };
        return Color.FromArgb(alpha, (byte)((r + m) * 255), (byte)((g + m) * 255), (byte)((b + m) * 255));
    }
}
