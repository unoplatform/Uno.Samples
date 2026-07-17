using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.UI.ViewManagement;
using Windows.Foundation;

namespace FitBeginnerApp.Presentation;

/// <summary>
/// A circular weekly-goal progress ring: a muted track with an accent arc that sweeps to
/// <see cref="Completed"/> / <see cref="Goal"/> on load, and the count read out in the centre.
/// The arc geometry is rebuilt as <see cref="Progress"/> (0..1) animates, so the sweep is a real
/// value animation rather than a fixed picture.
/// </summary>
public sealed partial class WeeklyRing : UserControl
{
    private const double Box = 176;      // control size
    private const double Thickness = 16; // stroke width
    private static readonly double Radius = (160 - Thickness) / 2; // matches the 160 track ellipse
    private static readonly double Center = Box / 2;

    public WeeklyRing()
    {
        this.InitializeComponent();
        Loaded += OnLoaded;
    }

    public int Completed
    {
        get => (int)GetValue(CompletedProperty);
        set => SetValue(CompletedProperty, value);
    }

    public static readonly DependencyProperty CompletedProperty =
        DependencyProperty.Register(nameof(Completed), typeof(int), typeof(WeeklyRing),
            new PropertyMetadata(0, OnDataChanged));

    public int Goal
    {
        get => (int)GetValue(GoalProperty);
        set => SetValue(GoalProperty, value);
    }

    public static readonly DependencyProperty GoalProperty =
        DependencyProperty.Register(nameof(Goal), typeof(int), typeof(WeeklyRing),
            new PropertyMetadata(1, OnDataChanged));

    /// <summary>Animated 0..1 fraction the arc is drawn to.</summary>
    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public static readonly DependencyProperty ProgressProperty =
        DependencyProperty.Register(nameof(Progress), typeof(double), typeof(WeeklyRing),
            new PropertyMetadata(0.0, OnProgressChanged));

    private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ring = (WeeklyRing)d;
        ring.UpdateText();
        if (ring.IsLoaded)
        {
            ring.AnimateIn();
        }
    }

    private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((WeeklyRing)d).UpdateArc();

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateText();
        AnimateIn();
    }

    private void UpdateText()
    {
        if (CountText is not null)
        {
            CountText.Text = $"{Completed}/{Goal}";
        }
    }

    private double TargetFraction => Goal > 0 ? Math.Clamp((double)Completed / Goal, 0, 1) : 0;

    private void AnimateIn()
    {
        var target = TargetFraction;

        // Reduced motion: jump straight to the value, no sweep (validated wiki rule).
        if (!new UISettings().AnimationsEnabled)
        {
            Progress = target;
            return;
        }

        var anim = new DoubleAnimationUsingKeyFrames { EnableDependentAnimation = true };
        anim.KeyFrames.Add(new DiscreteDoubleKeyFrame
        {
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero),
            Value = 0,
        });
        anim.KeyFrames.Add(new SplineDoubleKeyFrame
        {
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(700)),
            Value = target,
            // House EaseSmooth curve (0.22,1 0.36,1).
            KeySpline = new KeySpline
            {
                ControlPoint1 = new Point(0.22, 1),
                ControlPoint2 = new Point(0.36, 1),
            },
        });

        Storyboard.SetTarget(anim, this);
        Storyboard.SetTargetProperty(anim, nameof(Progress));

        var sb = new Storyboard();
        sb.Children.Add(anim);
        sb.Begin();
    }

    private void UpdateArc()
    {
        if (ArcPath is null)
        {
            return;
        }

        var fraction = Math.Clamp(Progress, 0, 1);
        if (fraction <= 0)
        {
            // Release the native geometry backing the old arc before detaching it.
            (ArcPath.Data as IDisposable)?.Dispose();
            ArcPath.Data = null;
            return;
        }

        // Sweep clockwise from 12 o'clock.
        const double startAngle = -90;
        var sweep = 360 * fraction;
        if (sweep >= 359.999)
        {
            sweep = 359.999; // a full 360 arc is degenerate; keep it just under
        }

        var figure = new PathFigure
        {
            IsClosed = false,
            StartPoint = PointOnCircle(startAngle),
        };
        figure.Segments.Add(new ArcSegment
        {
            Point = PointOnCircle(startAngle + sweep),
            Size = new Size(Radius, Radius),
            SweepDirection = SweepDirection.Clockwise,
            IsLargeArc = sweep > 180,
        });

        // Dispose the geometry from the previous update before replacing it (UpdateArc runs on every
        // progress tick during the entrance animation, so a fresh geometry is built each time). The new
        // geometry is owned by ArcPath.Data — assign it directly rather than via a throwaway local.
        (ArcPath.Data as IDisposable)?.Dispose();
        ArcPath.Data = new PathGeometry { Figures = { figure } };
    }

    private static Point PointOnCircle(double angleDegrees)
    {
        var a = angleDegrees * Math.PI / 180.0;
        return new Point(Center + Radius * Math.Cos(a), Center + Radius * Math.Sin(a));
    }
}
