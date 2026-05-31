using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;

namespace QuoteCraft.Controls;

/// <summary>
/// A lightweight sparkline chart that renders a polyline from an array of doubles.
/// </summary>
public sealed class SparklineControl : Grid
{
    private readonly Polyline _line = new()
    {
        StrokeThickness = 2,
        StrokeLineJoin = PenLineJoin.Round,
    };

    private readonly Polygon _fill = new()
    {
        Opacity = 0.15,
    };

    private DispatcherTimer? _debounceTimer;

    public static readonly DependencyProperty ValuesProperty =
        DependencyProperty.Register(nameof(Values), typeof(double[]), typeof(SparklineControl),
            new PropertyMetadata(null, OnValuesChanged));

    public static readonly DependencyProperty StrokeColorProperty =
        DependencyProperty.Register(nameof(StrokeColor), typeof(Brush), typeof(SparklineControl),
            new PropertyMetadata(null, OnStrokeChanged));

    public double[] Values
    {
        get => (double[])GetValue(ValuesProperty);
        set => SetValue(ValuesProperty, value);
    }

    public Brush StrokeColor
    {
        get => (Brush)GetValue(StrokeColorProperty);
        set => SetValue(StrokeColorProperty, value);
    }

    public SparklineControl()
    {
        Children.Add(_fill);
        Children.Add(_line);
        SizeChanged += (_, _) => DebouncedRedraw();
    }

    private void DebouncedRedraw()
    {
        _debounceTimer?.Stop();
        _debounceTimer ??= new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
        _debounceTimer.Tick += (_, _) =>
        {
            _debounceTimer.Stop();
            Redraw();
        };
        _debounceTimer.Start();
    }

    private static void OnValuesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((SparklineControl)d).Redraw();

    private static void OnStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (SparklineControl)d;
        ctrl._line.Stroke = e.NewValue as Brush;
        ctrl._fill.Fill = e.NewValue as Brush;
    }

    private void Redraw()
    {
        _line.Points.Clear();
        _fill.Points.Clear();

        var data = Values;
        if (data is null || data.Length < 2 || ActualWidth <= 0 || ActualHeight <= 0)
            return;

        var w = ActualWidth;
        var h = ActualHeight;
        var min = data.Min();
        var max = data.Max();
        var range = max - min;
        if (range == 0) range = 1;

        var padding = 4.0;
        var plotH = h - padding * 2;
        var stepX = w / (data.Length - 1);

        var linePoints = new PointCollection();
        var fillPoints = new PointCollection();

        for (int i = 0; i < data.Length; i++)
        {
            var x = i * stepX;
            var y = padding + plotH - ((data[i] - min) / range) * plotH;
            linePoints.Add(new Windows.Foundation.Point(x, y));
            fillPoints.Add(new Windows.Foundation.Point(x, y));
        }

        fillPoints.Add(new Windows.Foundation.Point(w, h));
        fillPoints.Add(new Windows.Foundation.Point(0, h));

        _line.Points = linePoints;
        _fill.Points = fillPoints;
    }
}
