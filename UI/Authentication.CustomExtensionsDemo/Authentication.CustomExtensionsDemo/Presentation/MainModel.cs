using Authentication.CustomExtensionsDemo.Authentication;
using Authentication.CustomExtensionsDemo.Common;
using Authentication.CustomExtensionsDemo.Client;

namespace Authentication.CustomExtensionsDemo.Presentation;

/// <summary>
/// Shell view model for <see cref="MainPage"/>: one child view model per section. Resolved by
/// Uno.Extensions Navigation, which injects the shared <see cref="CustomFlowService"/> singleton,
/// the Kiota client and the window's <see cref="IDispatcher"/>.
/// </summary>
public sealed class MainModel
{
    public MainModel(CustomFlowService flow, DemoServerClient api, IDispatcher dispatcher, SecretRedactor redaction)
    {
        Redaction = redaction;

        SignIn = new SignInViewModel(flow, dispatcher, redaction);
        Api = new ApiViewModel(flow, api, redaction);
        Setup = new PlatformSetupViewModel();
    }

    /// <summary>Recording mode, toggled from the header and shared by every section.</summary>
    public SecretRedactor Redaction { get; }

    public SignInViewModel SignIn { get; }

    public ApiViewModel Api { get; }

    public PlatformSetupViewModel Setup { get; }
}
