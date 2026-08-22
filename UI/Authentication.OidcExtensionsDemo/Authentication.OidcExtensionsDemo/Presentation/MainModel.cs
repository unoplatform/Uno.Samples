using Authentication.OidcExtensionsDemo.Authentication;

namespace Authentication.OidcExtensionsDemo.Presentation;

/// <summary>
/// Shell view model for <see cref="MainPage"/>. Resolved by Uno.Extensions Navigation, which
/// injects the shared <see cref="OidcFlowService"/> singleton and the window's
/// <see cref="IDispatcher"/>.
/// </summary>
public sealed class MainModel
{
    public MainModel(OidcFlowService flow, IDispatcher dispatcher)
    {
        SignIn = new SignInViewModel(flow, dispatcher);
    }

    public SignInViewModel SignIn { get; }
}
