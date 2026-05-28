using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using SkiaSharp;
using Uno.WinUI.Graphics2DSK;
using Windows.Foundation;
using EnterpriseDashboard.Observatory.Animation;
using EnterpriseDashboard.Observatory.Models;

namespace EnterpriseDashboard.Observatory.Charts;

public class ObservatoryHeatmapChart : SKCanvasElement, IAnimatableChart
{
    private static readonly string[] DayLabels = { "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN" };

    public static readonly DependencyProperty CellsProperty = DependencyProperty.Register(
        nameof(Cells), typeof(HeatmapCell[]), typeof(ObservatoryHeatmapChart),
        new PropertyMetadata(null, (d, _) => ((ObservatoryHeatmapChart)d).Invalidate()));

    public HeatmapCell[]? Cells { get => (HeatmapCell[]?)GetValue(CellsProperty); set => SetValue(CellsProperty, value); }

    private DispatcherTimer? _timer;
    private DateTime _started;
    private double _elapsed;
    private bool _isPlaying;
    public bool IsPlaying => _isPlaying;

    // Reveal-once: the diagonal cascade is fully in by ~1.4s (max cell delay + ramp);
    // after that we stop the timer and hold the filled frame.
    private const double RevealSeconds = 1.6;

    public ObservatoryHeatmapChart()
    {
        // Play is driven by the page's staggered cascade; only re-render on theme change here.
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
                _elapsed = RevealSeconds;   // hold the final, fully revealed frame
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
    // black — fine on the dark card, a black box on the light card. Clear to the active
    // theme's card colour so the chart background always matches its card.
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
        if (Cells == null || area.Width < 1 || area.Height < 1) return;

        float w = (float)area.Width;
        float h = (float)area.Height;
        const int rows = 7, cols = 24;
        float labelLeft = 24f, labelTop = 14f;
        float gridW = w - labelLeft - 4f;
        float gridH = h - labelTop - 4f;
        float cellW = gridW / cols;
        float cellH = gridH / rows;
        float gap = 1f;

        bool isLight = ActualTheme == ElementTheme.Light;

        using var font = new SKFont(SKTypeface.FromFamilyName("Courier New"), 8f);
        byte labelLum = isLight ? (byte)0x70 : (byte)0x55;
        using var labelPaint = new SKPaint { Color = new SKColor(labelLum, labelLum, labelLum), IsAntialias = true };

        for (int d = 0; d < rows; d++)
        {
            float cy = labelTop + d * cellH + cellH / 2 + 3;
            canvas.DrawText(DayLabels[d], 0, cy, SKTextAlign.Left, font, labelPaint);
        }
        for (int hr = 0; hr < cols; hr += 6)
        {
            float cx = labelLeft + hr * cellW + cellW / 2;
            canvas.DrawText(hr.ToString("00"), cx, 10, SKTextAlign.Center, font, labelPaint);
        }

        using var cellPaint = new SKPaint { IsAntialias = false, Style = SKPaintStyle.Fill };
        foreach (var cell in Cells)
        {
            float x = labelLeft + cell.Col * cellW + gap / 2;
            float y = labelTop + cell.Row * cellH + gap / 2;
            float cw = cellW - gap;
            float ch = cellH - gap;

            // Per-cell wave delay (diagonal cascade)
            double delay = 0.12 + (cell.Row + cell.Col) * 0.032;
            double localT = Math.Clamp((_elapsed - delay) / 0.35, 0, 1);
            double scaleY = EasingCurves.SpringT(localT);
            float effectiveH = (float)(ch * scaleY);
            float yOffset = ch - effectiveH;

            cellPaint.Color = MapValueToGrey(cell.Value, (float)localT, isLight);
            canvas.DrawRect(new SKRect(x, y + yOffset, x + cw, y + ch), cellPaint);
        }
    }

    private static SKColor MapValueToGrey(double v, float opacity, bool isLight)
    {
        // Brightness-as-magnitude. Dark theme: high value → light mark.
        // Light theme: ramp inverted so high value → dark ink on the ice-grey field.
        byte lum = isLight
            ? (v > 80 ? (byte)0x1E : v > 60 ? (byte)0x44 : v > 40 ? (byte)0x70 : (byte)0xA0)
            : (v > 80 ? (byte)0xEE : v > 60 ? (byte)0xBB : v > 40 ? (byte)0x88 : (byte)0x55);
        byte a = (byte)Math.Clamp(opacity * 255, 0, 255);
        return new SKColor(lum, lum, lum, a);
    }
}
