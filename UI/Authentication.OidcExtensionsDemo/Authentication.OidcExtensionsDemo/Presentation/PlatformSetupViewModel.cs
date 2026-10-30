using Authentication.OidcExtensionsDemo.Authentication;
using Authentication.OidcExtensionsDemo.Common;

namespace Authentication.OidcExtensionsDemo.Presentation;

/// <summary>
/// View state for <see cref="PlatformSetupView"/>: the static per-platform requirements.
/// </summary>
/// <remarks>
/// The guide quotes the configured redirect URI, so recording mode rebuilds it from the masked
/// value rather than scrubbing the finished text.
/// </remarks>
public sealed class PlatformSetupViewModel : ObservableObject
{
    private readonly OidcFlowService _flow;
    private readonly SecretRedactor _redactor;

    public PlatformSetupViewModel(OidcFlowService flow, SecretRedactor redactor)
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
        PlatformGuide.Build(_redactor.Apply(_flow.ConfiguredRedirectUri));
}
