using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace FieldOpsPro.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    // Mobile bottom-nav buttons drive NavigationView.SelectedItem so navigation flows
    // through Uno's NavigationView region integration — same canonical path as the
    // desktop pane. Avoids the back-stack accumulation that broke the previous
    // NavigateRouteAsync-from-code-behind approach.
    private void OnMobileNavClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not string route || string.IsNullOrEmpty(route))
        {
            return;
        }

        NavigationViewItem? target = route switch
        {
            "Dashboard" => NavDashboard,
            "Map" => NavMap,
            "Tasks" => NavTasks,
            "Profile" => NavProfile,
            _ => null,
        };

        if (target is not null)
        {
            DesktopNav.SelectedItem = target;
        }

        SetMobileActive(route);
    }

    private void SetMobileActive(string route)
    {
        var accent = Utils.ColorUtils.GetBrush("AccentPrimaryBrush");
        var muted = Utils.ColorUtils.GetBrush("TextMutedBrush");

        SetPair(HomeIcon, HomeLabel, route == "Dashboard", accent, muted);
        SetPair(MapIcon, MapLabel, route == "Map", accent, muted);
        SetPair(TasksIcon, TasksLabel, route == "Tasks", accent, muted);
        SetPair(ProfileIcon, ProfileLabel, route == "Profile", accent, muted);
    }

    private static void SetPair(FontIcon icon, TextBlock label, bool active, Brush accent, Brush muted)
    {
        icon.Foreground = active ? accent : muted;
        label.Foreground = active ? accent : muted;
    }
}
