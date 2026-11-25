using Microsoft.UI.Xaml.Automation;

namespace QuoteCraft.Controls;

public sealed partial class StatusPill : UserControl
{
    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(nameof(Status), typeof(object), typeof(StatusPill),
            new PropertyMetadata(null, (d, _) => ((StatusPill)d).Apply()));

    public static readonly DependencyProperty CountProperty =
        DependencyProperty.Register(nameof(Count), typeof(object), typeof(StatusPill),
            new PropertyMetadata(null, (d, _) => ((StatusPill)d).Apply()));

    public StatusPill()
    {
        InitializeComponent();
        Loaded += (_, _) => Apply();
    }

    /// <summary>The quote status (a <see cref="QuoteStatus"/> or its name).</summary>
    public object? Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    /// <summary>When set, the pill counts quotes in the status ("DRAFT: 3") instead of naming one quote's status.</summary>
    public object? Count
    {
        get => GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }

    private void Apply()
    {
        var text = Status?.ToString();
        if (string.IsNullOrEmpty(text) || !Enum.TryParse<QuoteStatus>(text, ignoreCase: true, out var status))
        {
            status = QuoteStatus.Draft;
        }

        var name = status.ToString();
        if (Count is { } count)
        {
            Label.Text = $"{name.ToUpperInvariant()}: {count}";
            AutomationProperties.SetName(this, $"{name}: {count}");
        }
        else
        {
            Label.Text = name.ToUpperInvariant();
            AutomationProperties.SetName(this, $"Status: {name}");
        }
        VisualStateManager.GoToState(this, status.ToString(), false);
    }
}
