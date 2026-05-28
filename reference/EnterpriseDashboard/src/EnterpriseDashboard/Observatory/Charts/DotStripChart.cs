using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using SkiaSharp;
using Uno.WinUI.Graphics2DSK;
using Windows.Foundation;
using EnterpriseDashboard.Observatory.Animation;

namespace EnterpriseDashboard.Observatory.Charts;

public class DotStripChart : SKCanvasElement, IAnimatableChart
{
    public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
        nameof(Points), typeof(double[]), typeof(DotStripChart),
        new PropertyMetadata(null, (d, _) => ((DotStripChart)d).Invalidate()));

    public double[]? Points { get => (double[]?)GetValue(PointsProperty); set => SetValue(PointsProperty, value); }

    private DispatcherTimer? _timer;
    private DateTime _started;
    private double _elapsed;
    private bool _isPlaying;
    public bool IsPlaying => _isPlaying;

    // Reveal-once: dots settle by ~3.0s (max delay 2.5s + 0.5s ramp) and the mean
    // line fades in after 2.5s; hold the final frame after that.
    private const double RevealSeconds = 3.2;

    public DotStripChart()
    {
        // Play is driven by the page's staggered cascade; re-render on theme change.
        ActualThemeChanged += (_, _) => Invalidate();
    }

    public void Play()
    {
        _started = DateTime.Now;
        _elapsed = 0;
        _isPlaying = true;
        _timer?.Stop();
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        _timer.Tick += (_, _) =>
        {
            _elapsed = (DateTime.Now - _started).TotalSeconds;
            if (_elapsed >= RevealSeconds)
            {
                _elapsed = RevealSeconds;   // hold the final, fully settled frame
                _timer!.Stop();
                _isPlaying = false;
            }
            Invalidate();
        };
        _timer.Start();
        Invalidate();
    }

    public void Reset()
    {
        _timer?.Stop();
        _isPlaying = false;
        _elapsed = 0;
        Invalidate();
    }

    // SKCanvasElement composites onto an opaque backing, so a transparent clear shows as
    // black — invisible on the dark card, a black box on the light card. Clear to the
    // active theme's card colour so the chart background always matches its card.
    private static SKColor CardBackground()
    {
        if (Application.Current.Resources["ObsCardBrush"] is SolidColorBrush b)
        {
            var c = b.Color;
            return new SKColor(c.R, c.G, c.B, c.A);
        }
        return SKColors.Transparent;
    }

    protected override void RenderOverride(SKCanvas canvas, Size area)
    {
        canvas.Clear(CardBackground());
        if (Points == null || area.Width < 1 || area.Height < 1) return;

        float w = (float)area.Width;
        float h = (float)area.Height;
        float padL = 28f, padR = 12f, padT = 12f, padB = 32f;
        float plotW = w - padL - padR;
        float plotH = h - padT - padB;
        float baseline = padT + plotH * 0.85f;

        bool isLight = ActualTheme == ElementTheme.Light;
        byte labelLum = isLight ? (byte)0x70 : (byte)0x55;
        byte axisLum = isLight ? (byte)0xC2 : (byte)0x33;

        using var font = new SKFont(SKTypeface.FromFamilyName("Courier New"), 8f);
        using var greyPaint = new SKPaint { Color = new SKColor(labelLum, labelLum, labelLum), IsAntialias = true };
        using var axisPaint = new SKPaint { Color = new SKColor(axisLum, axisLum, axisLum), StrokeWidth = 0.5f };

        // X axis
        canvas.DrawLine(padL, baseline, padL + plotW, baseline, axisPaint);
        for (int t = 0; t <= 4; t++)
        {
            float x = padL + plotW * t / 4f;
            canvas.DrawText((t * 25).ToString(), x, baseline + 14, SKTextAlign.Center, font, greyPaint);
        }

        const float dotR = 3.5f;
        const float binSize = 5f;
        var bins = new Dictionary<int, int>();

        for (int i = 0; i < Points.Length; i++)
        {
            double v = Points[i];
            int bin = (int)Math.Round(v / binSize);
            int row = bins.TryGetValue(bin, out var c) ? c : 0;
            bins[bin] = row + 1;

            float x = padL + plotW * (float)v / 100f;
            float yTarget = baseline - row * (dotR * 2.2f) - dotR;

            // Delay each dot with deterministic pseudo-random
            double delay = (300 + ((Math.Round(v * 7) + i * 29) % 2200)) / 1000.0;
            double localT = Math.Clamp((_elapsed - delay) / 0.5, 0, 1);
            double eased = EasingCurves.OutT(localT);

            float yFrom = padT - 5;
            float y = (float)(yFrom + (yTarget - yFrom) * eased);

            // Brightness encoding: central (within ~1σ) reads strongest. Dark theme
            // maps that to a bright mark; light theme to dark ink on the ice-grey field.
            bool central = Math.Abs(v - 50) < 16;
            byte lum = isLight
                ? (central ? (byte)0x2A : (byte)0x88)
                : (central ? (byte)0xBB : (byte)0x55);
            byte alpha = (byte)Math.Clamp(localT * 220, 0, 220);

            using var dotPaint = new SKPaint
            {
                Color = new SKColor(lum, lum, lum, alpha),
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };
            canvas.DrawCircle(x, y, dotR, dotPaint);
        }

        // Mean line
        if (_elapsed > 2.5)
        {
            byte alpha = (byte)Math.Clamp((_elapsed - 2.5) * 200, 0, 100);
            byte meanLum = isLight ? (byte)0x22 : (byte)0xDD;
            using var meanPaint = new SKPaint
            {
                Color = new SKColor(meanLum, meanLum, meanLum, alpha),
                StrokeWidth = 1, PathEffect = SKPathEffect.CreateDash(new[] { 4f, 4f }, 0)
            };
            float xMean = padL + plotW * 0.5f;
            canvas.DrawLine(xMean, padT + 4, xMean, baseline, meanPaint);
        }
    }
}
