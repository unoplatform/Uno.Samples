using System.Runtime.CompilerServices;

namespace QuoteCraft.Presentation;


/// <summary>
/// The shell around the four destinations: the tab bar and side rail, the theme menu, the offline banner and the
/// unread-notification badge. The page's visual states come from <see cref="ShellState"/>.
/// </summary>
public partial record MainModel
{
    private readonly INavigator _navigator;
    private readonly IThemeService _theme;
    private readonly INotificationService _notifications;
    private readonly ConnectivityHelper _connectivity;
    private readonly ThemeChoiceNotifier _themeChoice;

    public MainModel(
        INavigator navigator,
        IThemeService theme,
        ThemeChoiceNotifier themeChoice,
        INotificationService notifications,
        ConnectivityHelper connectivity)
    {
        _navigator = navigator;
        _theme = theme;
        _themeChoice = themeChoice;
        _notifications = notifications;
        _connectivity = connectivity;

        // Only polls when a Supabase project is configured (D3).
        _connectivity.StartMonitoring();
    }

    public IState<bool> IsRailCollapsed => State.Value(this, () => false);

    /// <summary>The theme chosen in the rail's menu or in Settings (the icon follows the window through ThemeResource).</summary>
    public IFeed<AppTheme> Theme => Feed.AsyncEnumerable(WatchThemeAsync);

    /// <summary>The rail's Theme button names the choice, so it reads the same on screen and to a screen reader.</summary>
    public IFeed<string> ThemeLabel => Theme.Select(t => t switch
    {
        AppTheme.Light => "Light theme",
        AppTheme.Dark => "Dark theme",
        _ => "System theme",
    });

    public IFeed<bool> IsSystemTheme => Theme.Select(t => t == AppTheme.System);
    public IFeed<bool> IsLightTheme => Theme.Select(t => t == AppTheme.Light);
    public IFeed<bool> IsDarkTheme => Theme.Select(t => t == AppTheme.Dark);

    public IFeed<bool> IsOnline => Feed.AsyncEnumerable(WatchConnectivityAsync);

    public IFeed<int> UnreadNotificationCount => Feed.AsyncEnumerable(WatchUnreadCountAsync);

    /// <summary>The bottom tab's badge: shown while anything is unread, with the count (99+ above 99).</summary>
    public IFeed<bool> HasUnreadNotifications => UnreadNotificationCount.Select(count => count > 0);

    public IFeed<string> UnreadBadge => UnreadNotificationCount.Select(count => count > 99 ? "99+" : count.ToString(System.Globalization.CultureInfo.CurrentCulture));

    /// <summary>
    /// The notifications drawer (route <c>!Notifications</c>, D15): from the rail's bell from 800 px, and from the
    /// bottom tab bar's Alerts item below, where the rail is hidden.
    /// </summary>
    public async ValueTask OpenNotifications(CancellationToken ct) =>
        await _navigator.NavigateRouteAsync(this, "Notifications", qualifier: Qualifiers.Dialog, cancellation: ct);

    /// <summary>The page's visual states (MainPage.xaml): the rail expanded or collapsed, the offline banner.</summary>
    public IFeed<string> ShellState => Feed.Combine(IsRailCollapsed, IsOnline)
        .Select(s => string.Join(',', s.Item1 ? "RailCollapsed" : "RailExpanded", s.Item2 ? "Online" : "Offline"));

    public async ValueTask ToggleRail(CancellationToken ct) => await IsRailCollapsed.UpdateAsync(c => !c, ct);

    /// <summary>The rail's theme menu: System (the default), Light or Dark, saved by the theme service (D6).</summary>
    public async ValueTask SetTheme(string theme)
    {
        if (Enum.TryParse<AppTheme>(theme, out var choice))
        {
            await _theme.SetThemeAsync(choice);
            _themeChoice.Raise();
        }
    }

    /// <summary>
    /// A tap on a tab. A selected tab does not navigate again, so after the quote editor (opened inside the Quotes
    /// tab's frame) the Quotes tab did nothing; this opens the tab's page whether or not it is selected. The model's
    /// navigator is the shell's: a plain route replaced the whole page (rail included) with the tab's page, and "./"
    /// alone stopped at the tab bar, which was already on the route. "./-/" reaches the content region and clears the
    /// tab frame's back stack, so the editor closes and the list shows.
    /// </summary>
    public async ValueTask OpenTab(string route, CancellationToken ct) =>
        await _navigator.NavigateRouteAsync(this, route, qualifier: Qualifiers.Nested + Qualifiers.ClearBackStack, cancellation: ct);

    private async IAsyncEnumerable<AppTheme> WatchThemeAsync([EnumeratorCancellation] CancellationToken ct)
    {
        var changed = new ChangeSignal();
        Action handler = changed.Raise;
        _themeChoice.Changed += handler;
        try
        {
            while (true)
            {
                // The saved choice, which every IThemeService instance shares.
                yield return _theme.Theme;
                await changed.WaitAsync(ct);
            }
        }
        finally
        {
            _themeChoice.Changed -= handler;
        }
    }

    private async IAsyncEnumerable<bool> WatchConnectivityAsync([EnumeratorCancellation] CancellationToken ct)
    {
        var changed = new ChangeSignal();
        Action<bool> handler = _ => changed.Raise();
        _connectivity.ConnectivityChanged += handler;
        try
        {
            while (true)
            {
                yield return _connectivity.IsOnline;
                await changed.WaitAsync(ct);
            }
        }
        finally
        {
            _connectivity.ConnectivityChanged -= handler;
        }
    }

    private async IAsyncEnumerable<int> WatchUnreadCountAsync([EnumeratorCancellation] CancellationToken ct)
    {
        var changed = new ChangeSignal();
        Action<int> handler = _ => changed.Raise();
        _notifications.UnreadCountChanged += handler;
        try
        {
            yield return await _notifications.GetUnreadCountAsync();
            while (true)
            {
                await changed.WaitAsync(ct);
                yield return _notifications.UnreadCount;
            }
        }
        finally
        {
            _notifications.UnreadCountChanged -= handler;
        }
    }
}
