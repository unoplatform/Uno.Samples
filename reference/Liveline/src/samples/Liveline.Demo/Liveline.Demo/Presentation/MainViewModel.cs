using System;
using System.Collections.Generic;
using Liveline.Models;
using Microsoft.UI.Dispatching;

namespace Liveline.Demo.Presentation;

/// <summary>
/// Drives the Liveline chart demo: random-walk data feed + chrome state
/// (grid/badge/fill/dot/pause, accent color, dark/light). The chart's data
/// pipeline stays imperative (timer-driven push) because that's the scenario
/// the control is designed to showcase.
/// </summary>
public partial class MainViewModel : ObservableObject, IDisposable
{
    private const int MaxPoints = 60;

    private readonly DispatcherQueueTimer _timer;
    private readonly DispatcherQueueTimer _loadingTimer;
    private readonly Queue<LivelinePoint> _buffer = new(MaxPoints);
    private readonly Random _rng = new();
    private double _currentValue = 100.0;

    [ObservableProperty]
    private LivelinePoint[] _data = Array.Empty<LivelinePoint>();

    [ObservableProperty]
    private double _value;

    [ObservableProperty]
    private LivelineTheme _theme;

    [ObservableProperty]
    private bool _isLoading = true;

    [ObservableProperty]
    private bool _isPaused;

    [ObservableProperty]
    private bool _showGrid = true;

    [ObservableProperty]
    private bool _showBadge = true;

    [ObservableProperty]
    private bool _showFill = true;

    [ObservableProperty]
    private bool _showDot = true;

    [ObservableProperty]
    private bool _isDark = true;

    [ObservableProperty]
    private string _accentColor = "#4CAF50";

    public MomentumDirection Momentum => ShowDot ? MomentumDirection.Auto : MomentumDirection.Off;

    public MainViewModel(ILogger<MainViewModel> logger)
    {
        _theme = new LivelineTheme { Color = AccentColor, IsDark = IsDark };

        var dispatcher = DispatcherQueue.GetForCurrentThread();
        _loadingTimer = dispatcher.CreateTimer();
        _loadingTimer.Interval = TimeSpan.FromSeconds(3);
        _loadingTimer.Tick += OnLoadingComplete;
        _loadingTimer.Start();

        _timer = dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(150);
        _timer.Tick += OnTimerTick;

        logger.LogInformation("Liveline demo started.");
    }

    [RelayCommand]
    private void SelectColor(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
            return;
        AccentColor = color;
    }

    partial void OnAccentColorChanged(string value)
    {
        Theme = new LivelineTheme { Color = value, IsDark = IsDark };
    }

    partial void OnIsDarkChanged(bool value)
    {
        Theme = new LivelineTheme { Color = AccentColor, IsDark = value };
    }

    partial void OnShowDotChanged(bool value)
    {
        OnPropertyChanged(nameof(Momentum));
    }

    private void OnLoadingComplete(DispatcherQueueTimer sender, object args)
    {
        _loadingTimer.Stop();

        var now = DateTimeOffset.Now;
        for (int i = MaxPoints; i > 0; i--)
        {
            _currentValue += (_rng.NextDouble() - 0.5) * 4;
            _buffer.Enqueue(new LivelinePoint(now.AddMilliseconds(-i * 100), _currentValue));
        }

        IsLoading = false;
        PushData();

        _timer.Start();
    }

    private void OnTimerTick(DispatcherQueueTimer sender, object args)
    {
        // Volatile random walk: occasional large spikes and dips.
        double step = (_rng.NextDouble() - 0.5) * 8;
        if (_rng.NextDouble() < 0.08)
            step *= 6;
        _currentValue += step;

        _buffer.Enqueue(new LivelinePoint(DateTimeOffset.Now, _currentValue));
        while (_buffer.Count > MaxPoints)
            _buffer.Dequeue();

        PushData();
    }

    private void PushData()
    {
        Data = _buffer.ToArray();
        Value = _currentValue;
    }

    public void Dispose()
    {
        _timer.Stop();
        _loadingTimer.Stop();
    }
}
