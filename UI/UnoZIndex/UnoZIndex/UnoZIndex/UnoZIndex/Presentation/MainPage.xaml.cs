using Uno.Toolkit.UI;

namespace UnoZIndex.Presentation
{
    public sealed partial class MainPage : Page
    {
        ShadowContainer foregroundCard;

        public MainPage()
        {
            this.InitializeComponent();
            foregroundCard = Card4;
        }

        private void Card_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

            var card = sender as ShadowContainer;
            Canvas.SetZIndex(foregroundCard, 0);
            if (card is not null)
            {
                var zIndex = Canvas.GetZIndex(card);
                Canvas.SetZIndex(card, 4);
                foregroundCard = card;
            }

        }
    }
}