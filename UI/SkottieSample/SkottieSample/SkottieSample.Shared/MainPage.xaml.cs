using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SkiaSharp;
using SkiaSharp.Skottie;
using SkiaSharp.Views.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SkottieSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Stopwatch _watch = new Stopwatch();
        private Animation _animation;
        private DispatcherQueueTimer _timer;

        public MainPage()
        {
            this.InitializeComponent();

            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Title = "Skottie WebAssembly";

            Load();
        }

        private async Task Load()
        {
            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/LottieLogo1.json"));
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
            _timer.Tick += (s, e) => {
                // TODO: Work out why canvas can't be resolved for Mobile and Wasm targets
                // canvas.Invalidate();
                (this.skiaCanvas.Children.First() as SKXamlCanvas).Invalidate();
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
}
