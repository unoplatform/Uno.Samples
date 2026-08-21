using Authentication.MsalExtensionsDemo.Authentication;

namespace Authentication.MsalExtensionsDemo.Presentation;

/// <summary>
/// Shell view model for <see cref="MainPage"/>: one child view model per section. Resolved by
/// Uno.Extensions Navigation, which injects the shared <see cref="MsalFlowService"/> singleton
/// and the window's <see cref="IDispatcher"/>.
/// </summary>
public sealed class MainModel
{
    public MainModel(MsalFlowService flow, IDispatcher dispatcher)
    {
        SignIn = new SignInViewModel(flow, dispatcher);
        Graph = new GraphViewModel(flow);
        Setup = new PlatformSetupViewModel(flow);
    }

    public SignInViewModel SignIn { get; }

    public GraphViewModel Graph { get; }

    public PlatformSetupViewModel Setup { get; }
}
