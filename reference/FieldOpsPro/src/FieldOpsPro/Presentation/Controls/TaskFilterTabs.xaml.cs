using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace FieldOpsPro.Presentation.Controls;

public sealed partial class TaskFilterTabs : UserControl
{
    public event EventHandler<string>? FilterChanged;
    public event EventHandler? NewTaskRequested;

    private static Brush B(string key) => Utils.ColorUtils.GetBrush(key);

    public TaskFilterTabs()
    {
        this.InitializeComponent();
    }

    public static readonly DependencyProperty UrgentCountProperty =
        DependencyProperty.Register(nameof(UrgentCount), typeof(int), typeof(TaskFilterTabs),
            new PropertyMetadata(3));

    public static readonly DependencyProperty UnassignedCountProperty =
        DependencyProperty.Register(nameof(UnassignedCount), typeof(int), typeof(TaskFilterTabs),
            new PropertyMetadata(5));

    public static readonly DependencyProperty SelectedFilterProperty =
        DependencyProperty.Register(nameof(SelectedFilter), typeof(string), typeof(TaskFilterTabs),
            new PropertyMetadata("All"));

    public int UrgentCount
    {
        get => (int)GetValue(UrgentCountProperty);
        set => SetValue(UrgentCountProperty, value);
    }

    public int UnassignedCount
    {
        get => (int)GetValue(UnassignedCountProperty);
        set => SetValue(UnassignedCountProperty, value);
    }

    /// <summary>Currently-selected filter tag. Bound declaratively so tab styling updates without
    /// imperative code.</summary>
    public string SelectedFilter
    {
        get => (string)GetValue(SelectedFilterProperty);
        set => SetValue(SelectedFilterProperty, value);
    }

    // ---- x:Bind helpers ----

    /// <summary>Selected tab gets the accent background; others stay transparent.</summary>
    public Brush TabBackground(string selected, string tag)
        => string.Equals(selected, tag, StringComparison.Ordinal)
            ? B("AccentPrimaryBrush")
            : new SolidColorBrush(Microsoft.UI.Colors.Transparent);

    /// <summary>Selected tab gets the on-accent foreground; others use the muted accent.</summary>
    public Brush TabForeground(string selected, string tag)
        => string.Equals(selected, tag, StringComparison.Ordinal)
            ? B("TextOnAccentBrush")
            : B("AccentMediumBrush");

    // ---- Events ----

    private void OnTabClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string filter)
        {
            SelectedFilter = filter;
            FilterChanged?.Invoke(this, filter);
        }
    }

    private void OnNewTaskClick(object sender, RoutedEventArgs e)
    {
        NewTaskRequested?.Invoke(this, EventArgs.Empty);
    }
}
