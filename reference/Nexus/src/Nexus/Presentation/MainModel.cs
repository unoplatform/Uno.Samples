namespace Nexus.Presentation;

/// <summary>
/// Host model for the tab shell (MainPage). Holds the chrome (header/tabs/footer) and the
/// content region into which the five section pages load by route name.
/// Empty by design — no model-level state; the chrome's clock + connection-dot pulse are
/// view-local concerns and live in code-behind.
/// </summary>
public partial record MainModel();
