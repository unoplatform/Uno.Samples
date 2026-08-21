using Authentication.MsalExtensionsDemo.Authentication;

namespace Authentication.MsalExtensionsDemo.Presentation;

/// <summary>
/// View state for <see cref="PlatformSetupView"/>: the static per-platform requirements.
/// </summary>
public sealed class PlatformSetupViewModel
{
    public PlatformSetupViewModel(MsalFlowService flow)
    {
        Platforms = PlatformGuide.Build(flow.ClientId);
    }

    public IReadOnlyList<SetupStep> CommonSteps => PlatformGuide.Common;

    public IReadOnlyList<PlatformGuideEntry> Platforms { get; }
}
