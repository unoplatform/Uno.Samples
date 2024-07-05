namespace NavigationWithXaml.Presentation;

public partial class MainViewModel : ObservableObject
{
    private INavigator _navigator;

    [ObservableProperty]
    private string? name;

    public MainViewModel(
        INavigator navigator)
    {
        _navigator = navigator;
        Title = "Main";
    }
    public string? Title { get; }

    public ICommand GoToSecond { get; }

    

}
