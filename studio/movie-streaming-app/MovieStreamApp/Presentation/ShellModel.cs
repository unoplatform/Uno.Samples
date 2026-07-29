using System.Threading.Tasks;
using Windows.Storage;

namespace MovieStreamApp.Presentation;

/// <summary>
/// Backs <see cref="Shell"/> (the navigation root that hosts the extended splash screen). On startup
/// it navigates to the first route — the onboarding carousel on the very first launch, the app shell
/// ("Main") on every launch after — which the framework injects into the splash screen's content area,
/// revealing it once loading completes. A plain class (no reactive members).
/// </summary>
public class ShellModel
{
    private const string OnboardingSeenKey = "HasSeenOnboarding";

    private readonly INavigator _navigator;

    public ShellModel(INavigator navigator)
    {
        _navigator = navigator;
        _ = Start();
    }

    private async Task Start() =>
        await _navigator.NavigateRouteAsync(this, route: IsFirstRun() ? "Onboarding" : "Main");

    // True once, on the very first launch: reads-and-sets a persisted flag so the onboarding carousel
    // shows only then (its Get Started / Skip go on to "Main"). Best-effort — any settings failure
    // falls through to "not first run" so startup can never block on the gate.
    private static bool IsFirstRun()
    {
        try
        {
            var values = ApplicationData.Current.LocalSettings.Values;
            if (values.ContainsKey(OnboardingSeenKey))
            {
                return false;
            }

            values[OnboardingSeenKey] = true;
            return true;
        }
        catch
        {
            return false;
        }
    }
}
