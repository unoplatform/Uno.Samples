using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CombiningMVUXAndMVVM.Presentation;

[ReactiveBindable(false)]
public partial class MainViewModel : ObservableObject
{
    private INavigator _navigator;
    public MainViewModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        INavigator navigator)
    {
        _navigator = navigator;
        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";
    }

    public string? Title { get; }

    [ObservableProperty]
    private string name = string.Empty;

    [RelayCommand]
    public async Task GoToSecond()
    {
        await _navigator.NavigateViewModelAsync<SecondModel>(this, data: new Entity(Name!));
    }
}
