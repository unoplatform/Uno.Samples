using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EnterpriseDashboard.Observatory.Controls;

public sealed partial class SectionDivider : UserControl
{
    public SectionDivider()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty NumberProperty = DependencyProperty.Register(
        nameof(Number), typeof(string), typeof(SectionDivider),
        new PropertyMetadata(string.Empty, (d, e) => ((SectionDivider)d).NumberText.Text = (string)e.NewValue));

    public string Number
    {
        get => (string)GetValue(NumberProperty);
        set => SetValue(NumberProperty, value);
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(SectionDivider),
        new PropertyMetadata(string.Empty, (d, e) => ((SectionDivider)d).TitleText.Text = (string)e.NewValue));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        nameof(Description), typeof(string), typeof(SectionDivider),
        new PropertyMetadata(string.Empty, (d, e) => ((SectionDivider)d).DescText.Text = (string)e.NewValue));

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
}
