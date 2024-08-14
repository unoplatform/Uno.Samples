using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Dispatching;
using SkiaSharp;
using SkiaSharp.Skottie;
using SkiaSharp.Views.Windows;

namespace SkottieSample;

public sealed partial class MainPage : Page
{
    private Stopwatch _watch = new Stopwatch();
    private Animation _animation;
    private DispatcherQueueTimer _timer;

    public MainPage()
    {
        this.InitializeComponent();

        _ = Load();
    }

    private async Task Load()
    {
        var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/LottieLogo1.json"));
        using var stream = await file.OpenReadAsync();

        using var fileStream = new SKManagedStream(stream.AsStream());

        if (SkiaSharp.Skottie.Animation.TryCreate(fileStream, out _animation))
        {
            _animation.Seek(0, null);

            Console.WriteLine($"SkottieSample(): Version:{_animation.Version} Duration:{_animation.Duration} Fps:{_animation.Fps} InPoint:{_animation.InPoint} OutPoint:{_animation.OutPoint}");
        }
        else
        {
            Console.WriteLine($"SkottieSample(): failed to load animation");
        }

        _timer = DispatcherQueue.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(Math.Max(1 / 60.0, 1 / _animation.Fps));
        _timer.Tick += (s, e) =>
        {
            // TODO: Work out why canvas can't be resolved for Mobile and Wasm targets
            // canvas.Invalidate();
            if (this.skiaCanvas.Children.FirstOrDefault() is SKXamlCanvas canvas)
            {
                canvas.Invalidate();
            }
            else
            {
                Console.WriteLine($"Canvas is not available");
                _timer.Stop();
            }
        };

        _timer.Start();
        _watch.Start();

    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        // the the canvas and properties
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        if (_animation != null)
        {
            _animation.SeekFrameTime((float)_watch.Elapsed.TotalSeconds, null);

            if (_watch.Elapsed > _animation.Duration)
            {
                _watch.Restart();
            }

            _animation.Render(canvas, new SKRect(0, 0, _animation.Size.Width, _animation.Size.Height));
        }
    }
}
