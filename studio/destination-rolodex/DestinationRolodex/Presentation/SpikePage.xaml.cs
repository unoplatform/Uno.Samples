using System.Diagnostics;
#if DEBUG
using DestinationRolodex.Harness;
#endif
using DestinationRolodex.Models;
using DestinationRolodex.Rendering;
using DestinationRolodex.Services;
using Microsoft.UI.Xaml.Controls.Primitives;
using SkiaSharp;
using Uno.WinUI.Graphics2DSK;

namespace DestinationRolodex.Presentation;

// xaml-lint: allow codebehind - DEBUG-only shader diagnostic page; it drives a Skia surface directly.
public sealed partial class SpikePage : Page
{
    private PhotoTransitionSurface? _surface;
    private DestinationMediaCache? _cache;
    private IReadOnlyList<Destination> _catalog = Array.Empty<Destination>();
    private DecodedPhoto? _a, _b;
    private Destination? _da, _db;
    private DispatcherTimer? _timer;
    private Stopwatch? _clock;

    public SpikePage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += (_, _) =>
        {
            _timer?.Stop();
            _surface?.Release();
            _cache?.Dispose();
        };
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (!SKCanvasElement.IsSupportedOnCurrentPlatform())
        {
            Status.Text = "SKCanvasElement.IsSupportedOnCurrentPlatform() = false on this target.";
            return;
        }

        _surface = new PhotoTransitionSurface();
        SurfaceHost.Child = _surface;
        Status.Text = _surface.Renderer.HasRuntimeEffect ? "Runtime effect compiled (SkSL)." : $"SkSL compile failed: {_surface.Renderer.CompileError}";

        _cache = new DestinationMediaCache();
        _catalog = await DestinationRepository.LoadPackagedAsync();
        await LoadPairAsync();
#if DEBUG
        var h = App.HarnessOptions;
        if (h.Mode == "liquid") ModeSwitch.IsOn = true;
        if (h.Seek is { } s) ProgressSlider.Value = s * 1000;
#if !__WASM__
        if (h.CapturePath is { } path)
        {
            await Task.Delay(h.SettleMs);
            await CaptureHelper.CaptureAsync(this, path);
            Environment.Exit(0);
        }
#endif
#endif
    }

    private async Task LoadPairAsync()
    {
        if (_cache is null)
        {
            return;
        }

        var ids = PairSwitch.IsOn ? ("jp", "it") : ("ph", "jp");
        _da = _catalog.First(d => d.Id == ids.Item1);
        _db = _catalog.First(d => d.Id == ids.Item2);
        _a = await _cache.GetAsync(_da);
        _b = await _cache.GetAsync(_db);
        Present();
    }

    private void Present()
    {
        if (_surface is null || _a is null || _b is null || _da is null || _db is null)
        {
            return;
        }

        var frame = new PhotoFrame(_a, _b, _da.FocalPoint, _db.FocalPoint,
            (float)(ProgressSlider.Value / 1000d), (float)(StrengthSlider.Value / 100d),
            DirectionSwitch.IsOn ? -1 : 1,
            ModeSwitch.IsOn ? PhotoTransitionKind.LiquidWarp : PhotoTransitionKind.MorphBlend,
            CameraPose.Identity, SKColor.Parse(_da.FallbackColor));
        _surface.Present(frame);
        var ms = _surface.Renderer.RecentRenderMilliseconds;
        if (ms.Count > 0)
        {
            Timing.Text = $"RenderOverride last {ms[^1]:0.00} ms, frames {_surface.Renderer.FramesRendered}";
        }
    }

    private void OnProgressChanged(object sender, RangeBaseValueChangedEventArgs e) => Present();

    private async void OnSettingChanged(object sender, RoutedEventArgs e)
    {
        if (sender == PairSwitch)
        {
            await LoadPairAsync();
        }
        else
        {
            Present();
        }
    }

    private void OnPlay(object sender, RoutedEventArgs e)
    {
        _timer?.Stop();
        _clock = Stopwatch.StartNew();
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(8) };
        _timer.Tick += (_, _) =>
        {
            var p = Math.Clamp(_clock!.Elapsed.TotalMilliseconds / 1200d, 0, 1);
            ProgressSlider.Value = p * 1000;
            if (p >= 1)
            {
                _timer.Stop();
            }
        };
        _timer.Start();
    }
}
