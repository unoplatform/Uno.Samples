using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;
using EnterpriseDashboard.Observatory.Animation;
using EnterpriseDashboard.Observatory.Helpers;

namespace EnterpriseDashboard.Observatory.Charts;

public sealed class GaugeChartControl : ChartViewport
{
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value), typeof(double), typeof(GaugeChartControl),
        new PropertyMetadata(0d, (d, _) => ((GaugeChartControl)d).EnsureBuilt()));

    public double Value { get => (double)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

    private Line? _needle;
    private double _needleCx, _needleCy;
    private RotateTransform? _needleRotate;

    protected override void BuildScene()
    {
        Surface.Children.Clear();
        _needle = null;
        _needleRotate = null;

        var grey = (SolidColorBrush)Application.Current.Resources["ObsGreyBrush"];
        var white = (SolidColorBrush)Application.Current.Resources["ObsWhiteBrush"];
        var posB = (SolidColorBrush)Application.Current.Resources["ObsPositiveBrush"];
        var negB = (SolidColorBrush)Application.Current.Resources["ObsNegativeBrush"];
        var accB = (SolidColorBrush)Application.Current.Resources["ObsAccentBrush"];

        double cx = SurfaceWidth / 2;
        double cy = SurfaceHeight * 0.78;
        double r = Math.Min(SurfaceWidth / 2 - 12, SurfaceHeight * 0.65);
        _needleCx = cx;
        _needleCy = cy;

        // Semantic zones: poor (red) < 50 · fair (amber) 50–80 · good (green) 80+
        Surface.Children.Add(MakeArc(cx, cy, r, 180, 270, negB, 4));
        Surface.Children.Add(MakeArc(cx, cy, r, 270, 324, accB, 4));
        Surface.Children.Add(MakeArc(cx, cy, r, 324, 360, posB, 4));

        for (int i = 0; i <= 10; i++)
        {
            double t = i / 10.0;
            double a = (180 + 180 * t) * Math.PI / 180;
            double r0 = r + 4;
            double r1 = r + (i % 5 == 0 ? 10 : 6);
            Surface.Children.Add(new Line
            {
                X1 = cx + r0 * Math.Cos(a),
                Y1 = cy + r0 * Math.Sin(a),
                X2 = cx + r1 * Math.Cos(a),
                Y2 = cy + r1 * Math.Sin(a),
                Stroke = grey,
                StrokeThickness = 1
            });
        }

        // Needle drawn at the 180° rest position; rotated via RotateTransform around hub.
        // 0% = -180° rotation (pointing left), 100% = 0° (pointing right).
        _needleRotate = new RotateTransform { CenterX = cx, CenterY = cy, Angle = -180 };
        _needle = new Line
        {
            X1 = cx, Y1 = cy,
            X2 = cx + (r - 8), Y2 = cy,
            Stroke = white, StrokeThickness = 2,
            RenderTransform = _needleRotate
        };
        Surface.Children.Add(_needle);

        var hub = new Ellipse { Width = 8, Height = 8, Fill = white };
        Canvas.SetLeft(hub, cx - 4);
        Canvas.SetTop(hub, cy - 4);
        Surface.Children.Add(hub);

        var zoneBrush = Value >= 80 ? posB : Value >= 50 ? accB : negB;
        var status = new TextBlock { Text = "NOMINAL", Style = (Style)Application.Current.Resources["ObsMonoTag"], Foreground = zoneBrush };
        status.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        Canvas.SetLeft(status, cx - status.DesiredSize.Width / 2);
        Canvas.SetTop(status, cy + 14);
        Surface.Children.Add(status);
    }

    // Tension & Release: needle swings past target then settles back.
    protected override Storyboard? CreateLoopStoryboard()
    {
        if (_needle == null || _needleRotate == null) return base.CreateLoopStoryboard();
        Surface.Opacity = 1;

        double frac = Math.Clamp(Value / 100, 0, 1);
        double restAngle = -180 + 180 * frac;
        double overshoot = -180 + 180 * Math.Min(1, frac + 0.10);

        var anim = new DoubleAnimationUsingKeyFrames();
        anim.KeyFrames.Add(new DiscreteDoubleKeyFrame { KeyTime = TimeSpan.Zero, Value = -180 });
        anim.KeyFrames.Add(new SplineDoubleKeyFrame
        {
            KeyTime = TimeSpan.FromMilliseconds(800),
            Value = overshoot,
            KeySpline = new KeySpline { ControlPoint1 = new Point(0.22, 0.61), ControlPoint2 = new Point(0.36, 1.0) }
        });
        anim.KeyFrames.Add(new SplineDoubleKeyFrame
        {
            KeyTime = TimeSpan.FromMilliseconds(1400),
            Value = restAngle,
            KeySpline = new KeySpline { ControlPoint1 = new Point(0.68, 0), ControlPoint2 = new Point(0.32, 1.0) }
        });
        anim.KeyFrames.Add(new DiscreteDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(5.0), Value = restAngle });
        anim.KeyFrames.Add(new SplineDoubleKeyFrame
        {
            KeyTime = TimeSpan.FromSeconds(5.5),
            Value = -180,
            KeySpline = new KeySpline { ControlPoint1 = new Point(0.4, 0), ControlPoint2 = new Point(1, 1) }
        });
        Storyboard.SetTarget(anim, _needle);
        Storyboard.SetTargetProperty(anim, "(UIElement.RenderTransform).(RotateTransform.Angle)");

        var sb = new Storyboard { RepeatBehavior = RepeatBehavior.Forever };
        sb.Children.Add(anim);
        return sb;
    }

    private static Microsoft.UI.Xaml.Shapes.Path MakeArc(double cx, double cy, double r, double a0, double a1, Brush stroke, double thickness)
    {
        double rad0 = a0 * Math.PI / 180;
        double rad1 = a1 * Math.PI / 180;
        var p0 = new Point(cx + r * Math.Cos(rad0), cy + r * Math.Sin(rad0));
        var p1 = new Point(cx + r * Math.Cos(rad1), cy + r * Math.Sin(rad1));
        bool large = Math.Abs(a1 - a0) > 180;

        var fig = new PathFigure { StartPoint = p0, IsClosed = false };
        fig.Segments.Add(new ArcSegment
        {
            Point = p1, Size = new Size(r, r),
            SweepDirection = SweepDirection.Clockwise, IsLargeArc = large
        });
        var geo = new PathGeometry();
        geo.Figures.Add(fig);
        return new Microsoft.UI.Xaml.Shapes.Path { Data = geo, Stroke = stroke, StrokeThickness = thickness };
    }
}
