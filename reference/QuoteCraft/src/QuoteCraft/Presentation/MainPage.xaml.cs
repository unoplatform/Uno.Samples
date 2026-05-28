using Uno.Toolkit.UI;
using QuoteCraft.Helpers;
using QuoteCraft.Services;

namespace QuoteCraft.Presentation;

public sealed partial class MainPage : Page
{
    private ConnectivityHelper? _connectivity;
    private bool _isSideRailCollapsed;

    /// <summary>Access the underlying MVUX model from the generated ViewModel DataContext.</summary>
    private MainModel? Model => MvuxHelper.GetModel<MainModel>(DataContext);

    public MainPage()
    {
        this.InitializeComponent();
        this.Loaded += MainPage_Loaded;
        this.Unloaded += MainPage_Unloaded;
    }

    private void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateThemeToggleVisuals();

        // Wire up offline banner via MainModel-supplied ConnectivityHelper
        var model = Model;
        if (model is not null)
        {
            _connectivity = model.Connectivity;
            _connectivity.ConnectivityChanged += OnConnectivityChanged;
            _connectivity.StartMonitoring();
        }
    }

    private void MainPage_Unloaded(object sender, RoutedEventArgs e)
    {
        if (_connectivity is not null)
        {
            _connectivity.ConnectivityChanged -= OnConnectivityChanged;
            _connectivity = null;
        }
    }

    private void OnConnectivityChanged(bool isOnline)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            OfflineBanner.Visibility = isOnline ? Visibility.Collapsed : Visibility.Visible;
        });
    }

    private void ThemeToggle_Click(object sender, RoutedEventArgs e)
    {
        var isDark = SystemThemeHelper.IsRootInDarkMode(this.XamlRoot);
        SystemThemeHelper.SetRootTheme(this.XamlRoot, !isDark);
        UpdateThemeToggleVisuals();
    }

    private void UpdateThemeToggleVisuals()
    {
        var isDark = SystemThemeHelper.IsRootInDarkMode(this.XamlRoot);
        ThemeIcon.Glyph = isDark ? "\uE706" : "\uE708";
        ThemeLabel.Text = isDark ? "Light Mode" : "Dark Mode";
    }

    // --- Notifications overlay ---

    private async void Notifications_Click(object sender, RoutedEventArgs e)
    {
        NotificationsOverlay.Visibility = Visibility.Visible;
        await LoadNotificationsAsync();
    }

    private void NotifBack_Click(object sender, RoutedEventArgs e)
    {
        NotificationsOverlay.Visibility = Visibility.Collapsed;
    }

    private async void MarkAllRead_Click(object sender, RoutedEventArgs e)
    {
        var model = Model;
        if (model is null) return;
        await model.MarkAllNotificationsReadAsync(CancellationToken.None);
        await LoadNotificationsAsync();
    }

    private async Task LoadNotificationsAsync()
    {
        var model = Model;
        if (model is null) return;

        var notifications = await model.GetAllNotificationsAsync();
        if (notifications.Count == 0)
        {
            NotificationsList.Visibility = Visibility.Collapsed;
            NotifEmptyState.Visibility = Visibility.Visible;
        }
        else
        {
            NotificationsList.ItemsSource = notifications;
            NotificationsList.Visibility = Visibility.Visible;
            NotifEmptyState.Visibility = Visibility.Collapsed;
        }
    }

    // --- Side rail collapse ---

    private void CollapseToggle_Click(object sender, RoutedEventArgs e)
    {
        _isSideRailCollapsed = !_isSideRailCollapsed;

        if (_isSideRailCollapsed)
        {
            // Collapse: icons only
            SideRail.Width = 56;
            BrandLabel.Visibility = Visibility.Collapsed;
            NotifLabel.Visibility = Visibility.Collapsed;
            ThemeLabel.Visibility = Visibility.Collapsed;
            CollapseLabel.Visibility = Visibility.Collapsed;
            CollapseIcon.Glyph = "\uE76C"; // ChevronRight

            // Hide labels on tab bar items via visual tree
            HideTabBarItemLabels(true);
        }
        else
        {
            // Expand: icons + labels
            SideRail.Width = 156;
            BrandLabel.Visibility = Visibility.Visible;
            NotifLabel.Visibility = Visibility.Visible;
            ThemeLabel.Visibility = Visibility.Visible;
            CollapseLabel.Visibility = Visibility.Visible;
            CollapseIcon.Glyph = "\uE76B"; // ChevronLeft

            HideTabBarItemLabels(false);
        }
    }

    private void HideTabBarItemLabels(bool hide)
    {
        // Find all LabelText elements in the side rail's TabBar items
        var tabBar = FindDescendant<TabBar>(SideRail);
        if (tabBar is null) return;

        foreach (var item in tabBar.Items.OfType<TabBarItem>())
        {
            var label = FindDescendantByName(item, "LabelText") as TextBlock;
            if (label is not null)
            {
                label.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }

    private static T? FindDescendant<T>(DependencyObject parent) where T : class
    {
        var count = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(parent, i);
            if (child is T match) return match;
            var result = FindDescendant<T>(child);
            if (result is not null) return result;
        }
        return null;
    }

    private static DependencyObject? FindDescendantByName(DependencyObject parent, string name)
    {
        var count = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(parent, i);
            if (child is FrameworkElement fe && fe.Name == name) return fe;
            var result = FindDescendantByName(child, name);
            if (result is not null) return result;
        }
        return null;
    }
}
