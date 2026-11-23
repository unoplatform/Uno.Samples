using DestinationRolodex.Controls;
using DestinationRolodex.Models;
using DestinationRolodex.Presentation;
using DestinationRolodex.Services;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.System;

namespace DestinationRolodex;

// xaml-lint: allow codebehind - demo glue: replay/seek are debug operations invoked on the card (no XAML command surface),
// the credits dialog is built from the catalog, and the DEBUG harness pins start states for captures.
public sealed partial class MainPage : Page
{
    private readonly DispatcherTimer _tuningDebounce = new() { Interval = TimeSpan.FromMilliseconds(320) };
    private bool _timelineUserActive;

    public DestinationDemoViewModel ViewModel { get; } = new();

    public MainPage()
    {
        InitializeComponent();
        ViewModel.ReplayRequested += async (_, _) => await ReplayAsync();
        ViewModel.SeekRequested += async (_, p) => await Card.SeekTransitionAsync(p);
        _tuningDebounce.Tick += (_, _) => { _tuningDebounce.Stop(); ViewModel.Replay(); };

        Card.TransitionStarted += (_, e) => ViewModel.ReportTransitionStart(e.From.Country, e.To.Country);
        Card.TransitionProgress += (_, e) => ViewModel.ReportProgress(e.Progress);
        Card.TransitionCompleted += (_, _) => ViewModel.ReportRendererReady(Card.RendererStatus, Card.RendererDetail);
        Card.RendererFallback += (_, message) => ViewModel.ReportFallback(message);
        Card.MediaFailed += (_, message) => ViewModel.ReportFallback($"Photo failed: {message}");
        ViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(DestinationDemoViewModel.IsFallback))
            {
                StatusDot.Fill = (Brush)Resources[ViewModel.IsFallback ? "DemoStatusFallbackBrushFixed" : "DemoStatusDotBrushFixed"];
            }
        };

        Loaded += OnLoaded;
        KeyDown += OnPageKeyDown;

        // Sliders react to the user only: programmatic updates (progress reporting, initial binding) never replay or seek.
        foreach (var slider in new[] { DurationSlider, StrengthSlider })
        {
            slider.AddHandler(PointerReleasedEvent, new PointerEventHandler((_, _) => _tuningDebounce.Start()), true);
            slider.AddHandler(PointerCaptureLostEvent, new PointerEventHandler((_, _) => _tuningDebounce.Start()), true);
            slider.AddHandler(KeyUpEvent, new KeyEventHandler((_, _) => _tuningDebounce.Start()), true);
        }

        TimelineSlider.AddHandler(PointerPressedEvent, new PointerEventHandler((_, _) => _timelineUserActive = true), true);
        TimelineSlider.AddHandler(PointerReleasedEvent, new PointerEventHandler((_, _) => _timelineUserActive = false), true);
        TimelineSlider.AddHandler(PointerCaptureLostEvent, new PointerEventHandler((_, _) => _timelineUserActive = false), true);
        TimelineSlider.AddHandler(KeyUpEvent, new KeyEventHandler((_, _) => ViewModel.Seek(TimelineSlider.Value / 1000d)), true);
        TimelineSlider.ValueChanged += (_, e) => { if (_timelineUserActive) ViewModel.Seek(e.NewValue / 1000d); };
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {

        var catalog = await DestinationRepository.LoadPackagedAsync();
#if DEBUG
        var h = App.HarnessOptions;
        if (h.Empty) catalog = Array.Empty<Destination>();
        else if (h.Single) catalog = catalog.Take(1).ToList();
        if (h.Viewport is { } vp) { PageRoot.Width = vp.Width; PageRoot.Height = vp.Height; PageRoot.HorizontalAlignment = HorizontalAlignment.Left; PageRoot.VerticalAlignment = VerticalAlignment.Top; }
        if (h.ReducedMotion) ViewModel.IsReducedMotion = true;
        if (h.Mode == "liquid") ViewModel.Choose(PhotoTransitionKind.LiquidWarp, replay: false);
        if (h.Crossfade) ViewModel.IsDiagnosticCrossfade = true;
        if (h.SecondCard) ViewModel.ShowSecondCard = true;
#endif
        ViewModel.Destinations = catalog;
        ViewModel.ReportRendererReady(Card.RendererStatus, Card.RendererDetail);
        var first = catalog.Count > 0 ? catalog[0].Country : string.Empty;
        var second = catalog.Count > 1 ? catalog[1].Country : string.Empty;
        ViewModel.ReportTransitionStart(first, second);
        ViewModel.RendererStatus = ViewModel.IsReducedMotion ? "Reduced motion enabled" : Card.RendererStatus == PhotoRendererStatus.Shader ? "Skia runtime effect ready" : ViewModel.RendererStatus;
#if DEBUG
        await RunHarnessAsync(h);
#endif
    }

    private async Task ReplayAsync()
    {
        ViewModel.IsReplaying = true;
        try
        {
            var ok = await Card.ReplayTransitionAsync();
            if (!ok && !ViewModel.IsFallback)
            {
                ViewModel.ReportFallback("Replay unavailable.");
            }
        }
        finally
        {
            ViewModel.IsReplaying = false;
        }
    }

    private void OnMorphChecked(object sender, RoutedEventArgs e)
    {
        if (ViewModel.TransitionKind != PhotoTransitionKind.MorphBlend || ViewModel.IsDiagnosticCrossfade) ViewModel.Choose(PhotoTransitionKind.MorphBlend);
    }

    private void OnLiquidChecked(object sender, RoutedEventArgs e)
    {
        if (ViewModel.TransitionKind != PhotoTransitionKind.LiquidWarp || ViewModel.IsDiagnosticCrossfade) ViewModel.Choose(PhotoTransitionKind.LiquidWarp);
    }

    private void OnCrossfadeChecked(object sender, RoutedEventArgs e)
    {
        if (!ViewModel.IsDiagnosticCrossfade) ViewModel.ChooseDiagnosticCrossfade();
    }

    private void OnPageKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.OriginalSource is Slider or TextBox or RadioButton or CountryPicker)
        {
            return;
        }

        switch (e.Key)
        {
            case VirtualKey.R: ViewModel.Replay(); e.Handled = true; break;
            case VirtualKey.Number1: ViewModel.Choose(PhotoTransitionKind.MorphBlend); e.Handled = true; break;
            case VirtualKey.Number2: ViewModel.Choose(PhotoTransitionKind.LiquidWarp); e.Handled = true; break;
        }
    }

#if DEBUG
    private async Task RunHarnessAsync(Harness.DebugHarness h)
    {
        if (!h.IsActive)
        {
            return;
        }

        await Task.Delay(400);
        if (h.Select is { } id)
        {
            var index = ViewModel.Destinations.ToList().FindIndex(d => d.Id == id);
            if (index >= 0) Card.GoTo(index);
        }

        if (h.AutoPlay) Card.Play();
        if (h.Look) Card.ViewMode = CardViewMode.Look;
        if (h.Position is { } q) Card.HoldPositionForDiagnostics(q);
        if (h.Seek is { } p) await Card.SeekTransitionAsync(p);
        if (h.Replay) await Card.ReplayTransitionAsync();

        if (h.Steps > 0)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (var i = 0; i < h.Steps; i++)
            {
                Card.Next();
                if (i % 50 == 0) await Task.Delay(1);
            }

            await Task.Delay(Math.Max(h.SettleMs, 3000));
            var cache = Card.GetCacheStatistics();
            var frames = Card.GetFrameStatistics();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            var json = $"{{ \"steps\": {h.Steps}, \"elapsedMs\": {sw.ElapsedMilliseconds}, \"selectedIndex\": {Card.SelectedIndex}, \"cacheCount\": {cache.Count}, \"cacheBytes\": {cache.Bytes}, \"retired\": {cache.Retired}, \"managedBytes\": {GC.GetTotalMemory(true)}, \"workingSetBytes\": {Environment.WorkingSet}, \"loopFrames\": {frames.Frames}, \"loopP50Ms\": {frames.P50Ms:0.##}, \"loopP95Ms\": {frames.P95Ms:0.##}, \"loopMaxMs\": {frames.MaxMs:0.##}, \"renderP50Ms\": {frames.RenderP50Ms:0.###}, \"renderP95Ms\": {frames.RenderP95Ms:0.###}, \"renderMaxMs\": {frames.RenderMaxMs:0.###} }}";
            await File.WriteAllTextAsync((h.CapturePath ?? "steps") + ".json", json);
        }

        if (h.PerfPath is { } perf)
        {
            Card.Play();
            await Task.Delay(12000);
            var frames = Card.GetFrameStatistics();
            var json = $"{{ \"seconds\": 12, \"loopFrames\": {frames.Frames}, \"loopP50Ms\": {frames.P50Ms:0.##}, \"loopP95Ms\": {frames.P95Ms:0.##}, \"loopMaxMs\": {frames.MaxMs:0.##}, \"renderP50Ms\": {frames.RenderP50Ms:0.###}, \"renderP95Ms\": {frames.RenderP95Ms:0.###}, \"renderMaxMs\": {frames.RenderMaxMs:0.###}, \"rendererStatus\": \"{Card.RendererStatus}\", \"workingSetBytes\": {Environment.WorkingSet}, \"managedBytes\": {GC.GetTotalMemory(false)} }}";
            await File.WriteAllTextAsync(perf, json);
        }

#if !__WASM__
        if (h.CapturePath is { } path)
        {
            await Task.Delay(h.SettleMs);
            await Harness.CaptureHelper.CaptureAsync(PageRoot, path);
        }
#endif

        Environment.Exit(0);
    }
#endif
}
