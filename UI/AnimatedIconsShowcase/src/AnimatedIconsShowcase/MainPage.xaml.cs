using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

namespace AnimatedIconsShowcase;

public sealed partial class MainPage : Page
{
    // Long enough for one marker segment to finish before the next state is applied.
    private static readonly TimeSpan PulseStep = TimeSpan.FromMilliseconds(450);

    public MainPage()
    {
        this.InitializeComponent();
    }

    private Border[] PointerStateTiles => [BackIconTile, FindIconTile, SettingsIconTile, GlobalNavigationIconTile];

    private Border[] OnOffStateTiles => [AcceptIconTile, ChevronIconTile];

    private void OnPointerIconEntered(object sender, PointerRoutedEventArgs e)
        => AnimatedIcon.SetState((DependencyObject)sender, "PointerOver");

    private void OnPointerIconExited(object sender, PointerRoutedEventArgs e)
        => AnimatedIcon.SetState((DependencyObject)sender, "Normal");

    private void OnPointerIconPressed(object sender, PointerRoutedEventArgs e)
        => AnimatedIcon.SetState((DependencyObject)sender, "Pressed");

    private void OnPointerIconReleased(object sender, PointerRoutedEventArgs e)
        => AnimatedIcon.SetState((DependencyObject)sender, "PointerOver");

    private async void OnPulseIcons(object sender, RoutedEventArgs e)
    {
        PulseIconsButton.IsEnabled = false;

        foreach (var state in new[] { "PointerOver", "Pressed", "Normal" })
        {
            foreach (var tile in PointerStateTiles)
            {
                AnimatedIcon.SetState(tile, state);
            }

            await Task.Delay(PulseStep);
        }

        PulseIconsButton.IsEnabled = true;
    }

    private void OnOnOffStateToggled(object sender, RoutedEventArgs e)
    {
        var state = ((ToggleSwitch)sender).IsOn ? "NormalOn" : "NormalOff";
        foreach (var tile in OnOffStateTiles)
        {
            AnimatedIcon.SetState(tile, state);
        }
    }

    private void OnPaneOpenToggled(object sender, RoutedEventArgs e)
        => DemoNavigationView.IsPaneOpen = ((ToggleSwitch)sender).IsOn;

    private void OnPlay(object sender, RoutedEventArgs e)
    {
        _ = Player.PlayAsync(0, 1, true);
        PlayerStatusText.Text = "Playing 0 → 1, looped";
    }

    private void OnPause(object sender, RoutedEventArgs e)
    {
        Player.Pause();
        PlayerStatusText.Text = "Paused";
    }

    private void OnResume(object sender, RoutedEventArgs e)
    {
        Player.Resume();
        PlayerStatusText.Text = "Resumed";
    }

    private void OnStop(object sender, RoutedEventArgs e)
    {
        Player.Stop();
        PlayerStatusText.Text = "Stopped";
    }

    private void OnProgressChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        Player.SetProgress(e.NewValue);
        PlayerStatusText.Text = $"Progress {e.NewValue:F2}";
    }
}
