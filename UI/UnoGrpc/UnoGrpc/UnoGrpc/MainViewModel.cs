using System;
using System.Collections.Generic;
using System.Text;
using UnoGrpc.Services;

namespace UnoGrpc;

public partial class MainViewModel(WeatherService weatherService) : ObservableObject
{
    private readonly WeatherService _weatherService = weatherService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WeatherIn))]
    public partial string City { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Temperature { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Wind { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Description { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsLoading { get; set; } = false;

    [ObservableProperty]
    public partial bool MockData { get; set; } = false;

    [ObservableProperty]
    public partial bool IsEmpty { get; set; } = false;

    public string WeatherIn => $"Weather in {City}";

    [RelayCommand]
    private async Task GetWeather()
    {
        if (string.IsNullOrEmpty(City))
        {
            (App.Current as App)?.Dispatcher?.TryEnqueue(() => IsEmpty = true);
            return;
        }

        (App.Current as App)?.Dispatcher?.TryEnqueue(() =>
        {
            IsLoading = true;
            IsEmpty = false;
        });

        var weatherReply = await _weatherService.GetWeatherAsync(City, MockData);

        (App.Current as App)?.Dispatcher?.TryEnqueue(() =>
        {
            Temperature = weatherReply.Temperature;
            Wind = weatherReply.Wind;
            Description = weatherReply.Description;
            IsLoading = false;
        });
    }
}
