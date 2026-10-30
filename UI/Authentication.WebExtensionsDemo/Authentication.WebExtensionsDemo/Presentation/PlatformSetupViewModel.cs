using Authentication.WebExtensionsDemo.Authentication;

namespace Authentication.WebExtensionsDemo.Presentation;

/// <summary>
/// View state for <see cref="PlatformSetupView"/>: the static per-platform requirements.
/// </summary>
public sealed class PlatformSetupViewModel
{
    public IReadOnlyList<SetupStep> CommonSteps => PlatformGuide.Common;

    public IReadOnlyList<PlatformGuideEntry> Platforms { get; } = PlatformGuide.Build();
}
