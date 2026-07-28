using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Windows.Graphics.Effects;
using Windows.Graphics.Effects.Interop;

namespace MovieStreamApp.Media;

/// <summary>
/// Attached property that paints a controllable Gaussian backdrop blur behind an element via the
/// Composition effect graph (CompositionBackdropBrush -> GaussianBlurEffect -> SpriteVisual). Unlike
/// AcrylicBrush — whose blur radius is a fixed ~30px with no public knob — the <see cref="AmountProperty"/>
/// here is tunable, so the "Liquid Glass" distortion can be made as strong as wanted while a separate
/// thin tint keeps the surface transparent. Portable on the Skia renderer (every target in this app).
/// Apply it to a CHILDLESS element (e.g. a background Border); layer the tint/edge on a sibling above it.
/// </summary>
public static class BackdropBlur
{
    public static readonly DependencyProperty AmountProperty =
        DependencyProperty.RegisterAttached(
            "Amount", typeof(double), typeof(BackdropBlur), new PropertyMetadata(0d, OnChanged));

    public static double GetAmount(DependencyObject obj) => (double)obj.GetValue(AmountProperty);
    public static void SetAmount(DependencyObject obj, double value) => obj.SetValue(AmountProperty, value);

    // Rounds the blur sprite to match the host's capsule corner (the SpriteVisual isn't clipped by the
    // Border's own CornerRadius, so it needs its own rounded clip).
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.RegisterAttached(
            "CornerRadius", typeof(double), typeof(BackdropBlur), new PropertyMetadata(0d));

    public static double GetCornerRadius(DependencyObject obj) => (double)obj.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(DependencyObject obj, double value) => obj.SetValue(CornerRadiusProperty, value);

    private sealed class State
    {
        public SpriteVisual? Sprite;
        public CompositionRoundedRectangleGeometry? Geometry;
    }

    private static readonly ConditionalWeakTable<FrameworkElement, State> _states = new();

    private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement fe)
        {
            return;
        }

        fe.Loaded -= OnLoaded;
        fe.Loaded += OnLoaded;
        fe.SizeChanged -= OnSizeChanged;
        fe.SizeChanged += OnSizeChanged;

        if (fe.IsLoaded)
        {
            Apply(fe);
        }
    }

    private static void OnLoaded(object sender, RoutedEventArgs e) => Apply((FrameworkElement)sender);

    private static void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var fe = (FrameworkElement)sender;
        if (_states.TryGetValue(fe, out var state) && state.Sprite is not null)
        {
            var size = new Vector2((float)e.NewSize.Width, (float)e.NewSize.Height);
            state.Sprite.Size = size;
            if (state.Geometry is not null)
            {
                state.Geometry.Size = size;
            }
        }
        else
        {
            Apply(fe);
        }
    }

    private static void Apply(FrameworkElement fe)
    {
        var amount = (float)GetAmount(fe);
        if (amount <= 0 || fe.ActualWidth <= 0 || fe.ActualHeight <= 0)
        {
            return;
        }

        var compositor = ElementCompositionPreview.GetElementVisual(fe).Compositor;

        var backdrop = compositor.CreateBackdropBrush();
        var blur = new GaussianBlurEffect
        {
            BlurAmount = amount,
            Source = new CompositionEffectSourceParameter("source"),
        };
        var effectBrush = compositor.CreateEffectFactory(blur).CreateBrush();
        effectBrush.SetSourceParameter("source", backdrop);

        var size = new Vector2((float)fe.ActualWidth, (float)fe.ActualHeight);
        var sprite = compositor.CreateSpriteVisual();
        sprite.Brush = effectBrush;
        sprite.Size = size;

        CompositionRoundedRectangleGeometry? geo = null;
        var radius = (float)GetCornerRadius(fe);
        if (radius > 0)
        {
            geo = compositor.CreateRoundedRectangleGeometry();
            geo.Size = size;
            geo.CornerRadius = new Vector2(radius);
            sprite.Clip = compositor.CreateGeometricClip(geo);
        }

        _states.AddOrUpdate(fe, new State { Sprite = sprite, Geometry = geo });
        ElementCompositionPreview.SetElementChildVisual(fe, sprite);
    }
}

/// <summary>
/// Minimal hand-rolled Win2D Gaussian blur effect. Uno ships the real GaussianBlurEffect but keeps its
/// Win2D wrapper internal, so this implements the D2D interop directly (CLSID = D2D1 Gaussian Blur) with
/// a public, settable <see cref="BlurAmount"/> (the blur standard deviation).
/// </summary>
internal sealed class GaussianBlurEffect : IGraphicsEffect, IGraphicsEffectSource, IGraphicsEffectD2D1Interop
{
    private static readonly Guid D2D1GaussianBlur = new("1FEB6D69-2FE6-4AC9-8C58-1D7F93E7A6A5");

    public string Name { get; set; } = "GaussianBlur";
    public IGraphicsEffectSource? Source { get; set; }
    public float BlurAmount { get; set; } = 3f;

    public Guid GetEffectId() => D2D1GaussianBlur;

    public void GetNamedPropertyMapping(string name, out uint index, out GraphicsEffectPropertyMapping mapping)
    {
        if (name == nameof(BlurAmount))
        {
            index = 0;
            mapping = GraphicsEffectPropertyMapping.Direct;
        }
        else
        {
            index = 0xFF;
            mapping = (GraphicsEffectPropertyMapping)0xFF;
        }
    }

    public uint GetPropertyCount() => 1;

    public object GetProperty(uint index) => index == 0 ? BlurAmount : null!;

    public IGraphicsEffectSource? GetSource(uint index) => Source;

    public uint GetSourceCount() => 1;
}
