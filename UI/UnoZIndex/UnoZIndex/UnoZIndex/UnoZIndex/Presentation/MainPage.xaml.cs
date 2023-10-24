using Uno.UI.Toolkit;

namespace UnoZIndex.Presentation
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void ElevatedView_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var card = sender as ElevatedView;
            if(card is not null)
            {
                var zIndex = Canvas.GetZIndex(card);
                Canvas.SetZIndex(card, -1*zIndex);

            }
        }
    }
}