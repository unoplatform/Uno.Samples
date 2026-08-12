using Authentication.MsalDemo.Authentication;

namespace Authentication.MsalDemo.Views;

/// <summary>
/// View state for <see cref="PlatformSetupView"/>: the static per-platform requirements.
/// </summary>
internal sealed class PlatformSetupViewModel
{
    public IReadOnlyList<SetupStep> CommonSteps => PlatformGuide.Common;

    public IReadOnlyList<PlatformGuideEntry> Platforms => PlatformGuide.All;
}
