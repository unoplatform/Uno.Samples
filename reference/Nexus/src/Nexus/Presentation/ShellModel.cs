namespace Nexus.Presentation;

/// <summary>
/// Root navigation host model. The Shell is a bootstrap <c>ContentControl</c>; the framework
/// resolves the default nested route ("Main") and hosts that page in <c>ShellContent</c>.
/// Empty by design — Nexus has no login gate, so there is no decision logic here.
/// </summary>
public partial record ShellModel();
