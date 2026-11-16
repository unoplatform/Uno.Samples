using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace CardViewMigration.Controls;

public sealed partial class CardView : ContentControl
{
    public static readonly DependencyProperty CardTitleProperty = DependencyProperty.Register(nameof(CardTitle), typeof(string), typeof(CardView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty CardDescriptionProperty = DependencyProperty.Register(nameof(CardDescription), typeof(string), typeof(CardView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty CardBrushProperty = DependencyProperty.Register(nameof(CardBrush), typeof(Brush), typeof(CardView), new PropertyMetadata(new SolidColorBrush(Colors.White)));
    public static readonly DependencyProperty IconImageSourceProperty = DependencyProperty.Register(nameof(IconImageSource), typeof(ImageSource), typeof(CardView), new PropertyMetadata(default(ImageSource)));
    public static readonly DependencyProperty IconBackgroundBrushProperty = DependencyProperty.Register(nameof(IconBackgroundBrush), typeof(Brush), typeof(CardView), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

    public CardView()
    {
        InitializeComponent();
#if !HAS_UNO_WINUI
        CardBorder.Translation += new System.Numerics.Vector3(0, 0, 32);
#endif
    }
    
    public string CardTitle
    {
        get => (string)GetValue(CardTitleProperty);
        set => SetValue(CardTitleProperty, value);
    }

    public string CardDescription
    {
        get => (string)GetValue(CardDescriptionProperty);
        set => SetValue(CardDescriptionProperty, value);
    }

    public Brush CardBrush
    {
        get => (Brush)GetValue(CardBrushProperty);
        set => SetValue(CardBrushProperty, value);
    }

    public ImageSource IconImageSource
    {
        get => (ImageSource)GetValue(IconImageSourceProperty);
        set => SetValue(IconImageSourceProperty, value);
    }

    public Brush IconBackgroundBrush
    {
        get => (Brush)GetValue(IconBackgroundBrushProperty);
        set => SetValue(IconBackgroundBrushProperty, value);
    }
}
