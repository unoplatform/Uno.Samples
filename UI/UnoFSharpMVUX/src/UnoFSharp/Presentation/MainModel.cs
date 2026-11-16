namespace UnoFSharp.Presentation;

public partial record MainModel
{
    private INavigator _navigator;

    public MainModel(
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

    public async Task GoToFSharpTest()
    {
        await _navigator.NavigateViewModelAsync<FSharpTestModel>(this);
    }

    public async Task GoToInteractive()
    {
        await _navigator.NavigateViewModelAsync<InteractiveModel>(this);
    }

}
