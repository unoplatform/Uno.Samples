using System.Collections.ObjectModel;
using System.Diagnostics;
using Caffe.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Caffe.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public ObservableCollection<EspressoItem> EspressoItems { get; } =
    [
        new("Espresso", 30, "Pure, concentrated, bold"),
        new("Doppio", 60, "Double the intensity"),
        new("Ristretto", 20, "Short, sweet, powerful"),
        new("Lungo", 50, "Long pull, smooth finish")
    ];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    [NotifyPropertyChangedFor(nameof(BrewButtonText))]
    [NotifyPropertyChangedFor(nameof(GrindAbbreviation))]
    private EspressoItem? _selectedEspresso;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BrewingParametersText))]
    private int _temperature = 93;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(GrindLabel))]
    [NotifyPropertyChangedFor(nameof(GrindAbbreviation))]
    [NotifyPropertyChangedFor(nameof(BrewingParametersText))]
    private GrindLevel _grindLevel = GrindLevel.Fine;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BrewingParametersText))]
    private int _extractionTime = 27;

    [ObservableProperty]
    private bool _isBrewing;

    [ObservableProperty]
    private double _brewProgress;

    public bool HasSelection => SelectedEspresso is not null;

    public bool CanBrew => HasSelection && !IsBrewing;

    public string BrewButtonText => SelectedEspresso is null
        ? "Select your espresso"
        : $"Brew {SelectedEspresso.Name}";

    public string GrindLabel => GrindLevel.ToLabel();
    public string GrindAbbreviation => GrindLevel.ToAbbreviation();

    public string BrewingParametersText =>
        $"{Temperature}°C · {GrindLabel} · {ExtractionTime}s";

    [RelayCommand(CanExecute = nameof(CanBrew))]
    private async Task BrewAsync()
    {
        if (SelectedEspresso is null) return;

        IsBrewing = true;
        BrewProgress = 0;

        var tcs = new TaskCompletionSource();
        var duration = TimeSpan.FromMilliseconds(2500);
        var stopwatch = Stopwatch.StartNew();

        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        timer.Tick += (s, e) =>
        {
            var elapsed = stopwatch.Elapsed;
            if (elapsed >= duration)
            {
                timer.Stop();
                stopwatch.Stop();
                BrewProgress = 1.0;
                tcs.TrySetResult();
            }
            else
            {
                BrewProgress = elapsed.TotalMilliseconds / duration.TotalMilliseconds;
            }
        };
        timer.Start();

        await tcs.Task;
        await Task.Delay(200);

        IsBrewing = false;
        BrewProgress = 0;
    }

    partial void OnSelectedEspressoChanged(EspressoItem? value)
    {
        BrewCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsBrewingChanged(bool value)
    {
        BrewCommand.NotifyCanExecuteChanged();
    }
}
