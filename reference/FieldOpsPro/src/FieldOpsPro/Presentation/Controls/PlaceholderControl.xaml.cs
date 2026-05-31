using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FieldOpsPro.Presentation.Controls;

public sealed partial class PlaceholderControl : UserControl
{
    public PlaceholderControl()
    {
        this.InitializeComponent();
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(PlaceholderControl),
            new PropertyMetadata("Section"));

    public static readonly DependencyProperty IconGlyphProperty =
        DependencyProperty.Register(nameof(IconGlyph), typeof(string), typeof(PlaceholderControl),
            new PropertyMetadata(""));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string IconGlyph
    {
        get => (string)GetValue(IconGlyphProperty);
        set => SetValue(IconGlyphProperty, value);
    }
}
