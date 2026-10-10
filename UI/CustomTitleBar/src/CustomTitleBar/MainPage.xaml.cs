using CustomTitleBar.Views;

namespace CustomTitleBar;

public sealed partial class MainPage : Page
{
    private static readonly string[] Suggestions =
    {
        "Trail map - Alder Ridge", "Soil sample log", "Heron sighting, 6 Oct",
        "Lichen field guide", "Weather station notes", "Camera trap schedule",
    };

    private bool _syncing;

    public MainPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (App.MainWindow is { } window)
        {
            window.SetTitleBar(AppTitleBar);
            AppTitleBar.LayoutUpdated += (_, _) => UpdatePassthroughRegions(window);

            // AppWindow.Resize takes physical pixels, so scale the 1280x860 DIP size
            double scale = XamlRoot?.RasterizationScale ?? 1;
            window.AppWindow.Resize(new Windows.Graphics.SizeInt32((int)(1280 * scale), (int)(860 * scale)));
        }

        if (NavView.SelectedItem is null)
        {
            NavView.SelectedItem = NavView.MenuItems[0];
        }
    }

    // Interactive title bar elements must be punched out of the caption region to receive pointer input
    private void UpdatePassthroughRegions(Window window)
    {
        if (XamlRoot is not { } xamlRoot)
        {
            return;
        }

        double scale = xamlRoot.RasterizationScale;
        List<Windows.Graphics.RectInt32> rects = new();
        foreach (FrameworkElement element in new FrameworkElement[] { BackButton, PaneToggleButton, SearchBox, NewNoteButton, Avatar })
        {
            if (element.Visibility == Visibility.Visible && element.ActualWidth > 0)
            {
                var bounds = element.TransformToVisual(null).TransformBounds(new Windows.Foundation.Rect(0, 0, element.ActualWidth, element.ActualHeight));
                rects.Add(new Windows.Graphics.RectInt32((int)(bounds.X * scale), (int)(bounds.Y * scale), (int)(bounds.Width * scale), (int)(bounds.Height * scale)));
            }
        }

        var source = Microsoft.UI.Input.InputNonClientPointerSource.GetForWindowId(window.AppWindow.Id);
        source.SetRegionRects(Microsoft.UI.Input.NonClientRegionKind.Passthrough, rects.ToArray());
    }

    private void OnBackClick(object sender, RoutedEventArgs e)
    {
        if (ContentFrame.CanGoBack)
        {
            ContentFrame.GoBack();
            SyncSelection();
        }
    }

    private void OnPaneToggleClick(object sender, RoutedEventArgs e) => NavView.IsPaneOpen = !NavView.IsPaneOpen;

    private void OnNavSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (_syncing || args.SelectedItem is not NavigationViewItem { Tag: string tag })
        {
            return;
        }

        Type page = tag switch
        {
            "Notes" => typeof(NotesPage),
            "Tags" => typeof(TagsPage),
            "Settings" => typeof(SettingsPage),
            _ => typeof(HomePage),
        };

        if (ContentFrame.CurrentSourcePageType != page)
        {
            ContentFrame.Navigate(page);
        }
    }

    private void SyncSelection()
    {
        string tag = ContentFrame.CurrentSourcePageType?.Name switch
        {
            nameof(NotesPage) => "Notes",
            nameof(TagsPage) => "Tags",
            nameof(SettingsPage) => "Settings",
            _ => "Home",
        };

        _syncing = true;
        NavView.SelectedItem = NavView.MenuItems.Concat(NavView.FooterMenuItems)
            .OfType<NavigationViewItem>().First(i => (string)i.Tag == tag);
        _syncing = false;
    }

    private void OnNewNoteClick(object sender, RoutedEventArgs e) => NavView.SelectedItem = NavView.MenuItems[1];

    private void OnSearchTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            sender.ItemsSource = Suggestions
                .Where(s => s.Contains(sender.Text, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }

    private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args) =>
        sender.Text = (string)args.SelectedItem;

    private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) =>
        NavView.SelectedItem = NavView.MenuItems[1];
}
