using Authentication.WebExtensionsDemo.Authentication;

namespace Authentication.WebExtensionsDemo.Presentation;

/// <summary>
/// Shell view model for <see cref="MainPage"/>. Resolved by Uno.Extensions Navigation, which
/// injects the shared <see cref="WebFlowService"/> singleton and the window's
/// <see cref="IDispatcher"/>.
/// </summary>
public sealed class MainModel
{
    public MainModel(WebFlowService flow, IDispatcher dispatcher)
    {
        SignIn = new SignInViewModel(flow, dispatcher);
    }

    public SignInViewModel SignIn { get; }
}
