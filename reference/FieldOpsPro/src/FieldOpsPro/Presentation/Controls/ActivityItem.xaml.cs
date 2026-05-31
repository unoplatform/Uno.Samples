using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FieldOpsPro.Models;

namespace FieldOpsPro.Presentation.Controls;

public sealed partial class ActivityItem : UserControl
{
    public ActivityItem()
    {
        this.InitializeComponent();
    }

    public static readonly DependencyProperty ActivityProperty =
        DependencyProperty.Register(nameof(Activity), typeof(Activity), typeof(ActivityItem),
            new PropertyMetadata(null));

    public static readonly DependencyProperty IsLastItemProperty =
        DependencyProperty.Register(nameof(IsLastItem), typeof(bool), typeof(ActivityItem),
            new PropertyMetadata(false));

    public Activity? Activity
    {
        get => (Activity?)GetValue(ActivityProperty);
        set => SetValue(ActivityProperty, value);
    }

    public bool IsLastItem
    {
        get => (bool)GetValue(IsLastItemProperty);
        set => SetValue(IsLastItemProperty, value);
    }

    /// <summary>Action text run: " " + message. Invoked from x:Bind in XAML.</summary>
    public string ActionLabel(Activity? activity)
        => activity is null ? string.Empty : " " + activity.Message;

    /// <summary>Timeline line is hidden on the last item. Invoked from x:Bind.</summary>
    public Visibility TimelineVisibility(bool isLast)
        => isLast ? Visibility.Collapsed : Visibility.Visible;
}
