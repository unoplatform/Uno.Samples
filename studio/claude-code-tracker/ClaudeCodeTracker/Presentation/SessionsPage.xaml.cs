using Uno.Toolkit.UI;

namespace ClaudeCodeTracker.Presentation;

public sealed partial class SessionsPage : Page
{
    private IReadOnlyList<SessionEntry> _allSessions = SampleData.Sessions;
    private string _modelFilter = "All";

    public SessionsPage()
    {
        this.InitializeComponent();

        // Hot Design fallback only; at runtime Navigation injects the SessionsModel from the
        // ViewMap, so gate this on design mode to avoid shadowing the injected instance (lesson 21).
        if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
        {
            this.DataContext = new SessionsModel();
        }
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is SessionsModel model)
        {
            _allSessions = model.Sessions;
        }

        // Default to the "All" chip the first time the list renders.
        ModelFilter.SelectedItem ??= _modelFilter;
        ApplyFilter();
    }

    private void ModelFilter_ItemChecked(object sender, ChipItemEventArgs e)
    {
        _modelFilter = ModelFilter.SelectedItem as string ?? "All";
        ApplyFilter();
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilter();

    // Search text and the model chip compose: both predicates must match.
    private void ApplyFilter()
    {
        var query = SearchBox.Text?.Trim() ?? string.Empty;

        IEnumerable<SessionEntry> result = _allSessions;
        if (!string.Equals(_modelFilter, "All", StringComparison.OrdinalIgnoreCase))
        {
            result = result.Where(s => s.ModelDisplayName.Contains(_modelFilter, StringComparison.OrdinalIgnoreCase));
        }
        if (!string.IsNullOrEmpty(query))
        {
            result = result.Where(s => s.ProjectName.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        var list = result.ToList();
        SessionList.ItemsSource = list;
        EmptyState.Visibility = list.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }
}
