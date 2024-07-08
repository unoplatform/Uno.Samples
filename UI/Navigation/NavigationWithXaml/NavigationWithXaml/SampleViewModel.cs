namespace NavigationWithXaml.Presentation;
public class SampleViewModel
{
    public SampleViewModel(INavigator navigator)
    {
        _navigator = navigator;
    }

    private readonly INavigator _navigator;
}
