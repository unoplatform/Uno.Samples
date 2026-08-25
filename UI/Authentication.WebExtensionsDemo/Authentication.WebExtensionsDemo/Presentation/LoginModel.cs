using Authentication.WebExtensionsDemo.AuthFlow;

namespace Authentication.WebExtensionsDemo.Presentation;

public partial record LoginModel(IDispatcher Dispatcher, INavigator Navigator, AuthFlowService Flow)
{
    public string Title { get; } = "Sign in";

    public async ValueTask Login(CancellationToken token)
    {
        var success = await Flow.SignInAsync(Dispatcher, token);
        if (success)
        {
            await Navigator.NavigateViewModelAsync<MainModel>(this, qualifier: Qualifiers.ClearBackStack);
        }
    }
}
