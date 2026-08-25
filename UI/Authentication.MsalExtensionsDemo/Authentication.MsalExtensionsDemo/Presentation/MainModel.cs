using Authentication.MsalExtensionsDemo.Authentication;
using Authentication.MsalExtensionsDemo.Common;

namespace Authentication.MsalExtensionsDemo.Presentation;

/// <summary>
/// Shell view model for <see cref="MainPage"/>: one child view model per section. Resolved by
/// Uno.Extensions Navigation, which injects the shared <see cref="MsalFlowService"/> singleton
/// and the window's <see cref="IDispatcher"/>.
/// </summary>
public sealed class MainModel
{
    public MainModel(MsalFlowService flow, IDispatcher dispatcher, SecretRedactor redaction)
    {
        Redaction = redaction;

        SignIn = new SignInViewModel(flow, dispatcher, redaction);
        Graph = new GraphViewModel(flow, redaction);
        Setup = new PlatformSetupViewModel(flow, redaction);
    }

    /// <summary>Recording mode, toggled from the header and shared by every section.</summary>
    public SecretRedactor Redaction { get; }

    public SignInViewModel SignIn { get; }

    public GraphViewModel Graph { get; }

    public PlatformSetupViewModel Setup { get; }
}
