namespace Meridian.Presentation;

/// <summary>
/// Root navigation host model. The Shell is a bootstrap <c>ContentControl</c>; the framework
/// resolves the default nested route ("Dashboard") and hosts that page in <c>ShellContent</c>.
/// Empty by design — Meridian has no login gate or pre-route decision.
/// </summary>
public partial record ShellModel();
