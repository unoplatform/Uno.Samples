using CardViewMigration.Controls;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace CardViewMigration;

public partial class CardViewCodePage : Page
{
    public CardViewCodePage()
    {
        InitializeComponent();

        Button backButton = new();
        backButton.Click += (s, e) => Frame.GoBack();
        backButton.Style = (Style)App.Current.Resources["NavigationBackButtonNormalStyle"];
        ToolTipService.SetToolTip(backButton, "Back");

        StackPanel layout = new StackPanel
        {
            Padding = new Thickness(20),
            Spacing = 20,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            
            Children =
            {
                backButton,
                new CardView
                {
                    BorderBrush = new SolidColorBrush(Colors.DarkGray),
                    CardTitle = "Slavko Vlasic",
                    CardDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla elit dolor, convallis non interdum.",
                    IconBackgroundBrush = new SolidColorBrush(Colors.SlateGray),
                    IconImageSource = new BitmapImage(new Uri("ms-appx:///Assets/user.png"))
                },
                new CardView
                {
                    BorderBrush = new SolidColorBrush(Colors.DarkGray),
                    CardTitle = "Carolina Pena",
                    CardDescription = "Phasellus eu convallis mi. In tempus augue eu dignissim fermentum. Morbi ut lacus vitae eros lacinia.",
                    IconBackgroundBrush = new SolidColorBrush(Colors.SlateGray),
                    IconImageSource = new BitmapImage(new Uri("ms-appx:///Assets/user.png"))
                },
                new CardView
                {
                    BorderBrush = new SolidColorBrush(Colors.DarkGray),
                    CardTitle = "Wade Blanks",
                    CardDescription = "Aliquam sagittis, odio lacinia fermentum dictum, mi erat scelerisque erat, quis aliquet arcu.",
                    IconBackgroundBrush = new SolidColorBrush(Colors.SlateGray),
                    IconImageSource = new BitmapImage(new Uri("ms-appx:///Assets/user.png"))
                },
                new CardView
                {
                    BorderBrush = new SolidColorBrush(Colors.DarkGray),
                    CardTitle = "Colette Quint",
                    CardDescription = "In pellentesque odio eget augue elementum lobortis. Sed augue massa, rhoncus eu nisi vitae, egestas.",
                    IconBackgroundBrush = new SolidColorBrush(Colors.SlateGray),
                    IconImageSource = new BitmapImage(new Uri("ms-appx:///Assets/user.png"))
                },
            }
        };

        ScrollViewer scroll = new ScrollViewer
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Content = layout
        };

        Content = scroll;
    }
}
