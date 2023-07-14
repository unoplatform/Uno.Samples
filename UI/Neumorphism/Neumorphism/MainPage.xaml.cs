using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Neumorphism
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void KeyTapped(object sender, TappedRoutedEventArgs e)
        {
            imgKey.Source = new BitmapImage(new Uri("ms-appx:///UnoApp1/Assets/Icons/Key-Close.png"));
            grdKeyText.Text = "Unlocking...";
            OpenKeyEffect.Begin();
        }

        public void OpenKeyEffect_Completed(object sender, object e)
        {
            imgKey.Source = new BitmapImage(new Uri("ms-appx:///UnoApp1/Assets/Icons/Key-Open.png"));
            grdKeyText.Text = "Unlocked";
        }
    }
}