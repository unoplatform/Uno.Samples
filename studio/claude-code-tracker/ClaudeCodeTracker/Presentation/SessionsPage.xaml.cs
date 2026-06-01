using ClaudeCodeTracker.Presentation.MockData;

namespace ClaudeCodeTracker.Presentation;

public sealed partial class SessionsPage : Page
{
    private readonly SessionsData _data;
    private readonly IReadOnlyList<SessionEntry> _allSessions;

    public SessionsPage()
    {
        this.InitializeComponent();
        _data = SessionsPageMockData.Data;
        _allSessions = _data.Sessions;
        Root.DataContext = _data;
        SessionList.ItemsSource = _allSessions;
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var query = SearchBox.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(query))
        {
            SessionList.ItemsSource = _allSessions;
        }
        else
        {
            var filtered = _allSessions
                .Where(s => s.ProjectName.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
            SessionList.ItemsSource = filtered;
        }
    }
}
