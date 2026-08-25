using Authentication.MsalExtensionsDemo.Authentication;
using Authentication.MsalExtensionsDemo.Common;

namespace Authentication.MsalExtensionsDemo.Presentation;

/// <summary>
/// View state for <see cref="PlatformSetupView"/>: the static per-platform requirements.
/// </summary>
/// <remarks>
/// The guide bakes the client ID into the values it quotes - the Android redirect URI is
/// <c>msal{ClientId}://auth</c> - so recording mode rebuilds it from the masked ID rather than
/// scrubbing the finished text.
/// </remarks>
public sealed class PlatformSetupViewModel : ObservableObject
{
    private readonly MsalFlowService _flow;
    private readonly SecretRedactor _redactor;

    public PlatformSetupViewModel(MsalFlowService flow, SecretRedactor redactor)
    {
        _flow = flow;
        _redactor = redactor;

        Platforms = Build();

        _redactor.Changed += (_, _) =>
        {
            Platforms = Build();
            Raise(nameof(Platforms));
        };
    }

    public IReadOnlyList<SetupStep> CommonSteps => PlatformGuide.Common;

    public IReadOnlyList<PlatformGuideEntry> Platforms { get; private set; }

    private IReadOnlyList<PlatformGuideEntry> Build() =>
        PlatformGuide.Build(_redactor.Apply(_flow.ClientId));
}
